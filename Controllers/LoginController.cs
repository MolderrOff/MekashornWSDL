using System;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceReference1;


//Front  запускается по пути http://localhost:5173

namespace Mekashron; 

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    public readonly ICUTechClient _soapClient;
    private readonly SoapService _soapService;
    public LoginController(ICUTechClient soapClient, SoapService soapService)
    {
        _soapClient = soapClient;
        _soapService = soapService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(ModelState);
        }
        try
        {        
            LoginResponse response = await _soapClient.LoginAsync(model.UserName, model.Password, HttpContext.Connection.RemoteIpAddress?.ToString());
            
            if (response != null)
            {
                return Ok(response.@return);
            }
            else
            {
                return Unauthorized(new { Message = "Ошибка аутентификации" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Ошибка сервиса: {ex.Message}" });
        }
    }
    [HttpPost("login2")]
    public async Task<IActionResult> Login2([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var response = await _soapService.GetSoap(model.UserName, model.Password, model.Ip);
            return Ok(new
            {
                UserName = model.UserName,
                ReturnMessage = response.@return 
            });
        }
        catch (Exception ex) 
        {
            return StatusCode(500, new { Message = $"Ошибка при обработке запроса: {ex.Message}" }); 
        }
       
    }
    [HttpPost("login3")]
    public async Task<IActionResult> Login3([FromBody] LoginModel model)
    {
        try
        {
            var response = await _soapService.GetResponseSoapManual(model.UserName, model.Password, model.Ip);
            var userdata = JsonSerializer.Deserialize<dynamic>(response);           

            return Ok(userdata);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });  
        }
       
    }
}
