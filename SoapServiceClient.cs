using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using ServiceReference1;


namespace Mekashron;

public class SoapService
{
    public async Task<LoginResponse> GetSoap(string userName, string password, string ip) //
    {
        var client = new ICUTechClient();
        try
        {
            Console.WriteLine("Попытка входа...");
            LoginResponse response = await client.LoginAsync(userName, password, ip);


            if (response == null || response.@return == null)
            {
                throw new InvalidOperationException("Сервис вернул неверный формат ответа.");
            }
            Console.WriteLine($"Для response = {userName}");
            Console.WriteLine($"Сообщение: {response.@return.ToString()}");

            return response;
        }
        catch (FaultException ex)
        {
            Console.WriteLine($"Ошибка SOAP/WCF: {ex.Message}");
            throw new Exception("Ошибка сервиса WCF", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
            throw;
        }
        finally
        {
            await client.CloseAsync();
        }

    }
    public async Task<string> GetResponseSoapManual(string userName, string password, string ip)
    {
        string _url = "http://isapi.mekashron.com/icu-tech/icutech-test.dll/soap/IICUTech";
        string _soapEnvelope = @$"
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
            xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
            xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""
            xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/"">
            <soap:Body soap:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
            <NS1:Login xmlns:NS1=""urn:ICUTech.Intf-IICUTech"">
            <UserName xsi:type=""xsd:string"">{userName}</UserName>
            <Password xsi:type=""xsd:string"">{password}</Password>
            <IP xsi:type=""xsd:string"">{ip}</IP>
            </NS1:Login>
            </soap:Body>
            </soap:Envelope>";
        WebRequest _request = WebRequest.Create(_url);
        _request.Method = "POST";
        _request.ContentType = "text/xml; charset=utf-8";

        _request.Headers.Add("SOAPAction", "urn:ICUTech.Intf-IICUTech#Login");


        byte[] byteArray = Encoding.UTF8.GetBytes(_soapEnvelope);
        _request.ContentLength = byteArray.Length;

        using (Stream _streamWriter = _request.GetRequestStream())
        {
            _streamWriter.WriteAsync(byteArray, 0, byteArray.Length);
        }

        string _result;
        try
        {
            using (WebResponse _response = await _request.GetResponseAsync())
            {
                using (StreamReader _streamReader = new StreamReader(_response.GetResponseStream()))
                {
                    _result = await _streamReader.ReadToEndAsync(); 
                    var doc = XDocument.Parse(_result);
                    var resultString = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "return")?.Value;

                    return resultString;
                }
            }

        }
        catch (WebException e)
        {
            if (e.Response != null)
            {
                using (var errorDataStream = e.Response.GetResponseStream()) 
                using (var reader = new StreamReader(errorDataStream))
                {
                    return "ERROR_FROM_SERVER: " + await reader.ReadToEndAsync();
                }
            }
            else
            {
                Console.WriteLine($"Произошла сетевая ошибка без ответа сервера: {e.Message}");
                _result = $"Произошла сетевая ошибка без ответа сервера: {e.Message}";
            }
        }
        return _result; 
    }
}