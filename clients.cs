using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using ServiceReference1;


namespace ServiceReference1;

using System.ServiceModel.Description;
using System.ServiceModel;
public partial class ICUTechClient : System.ServiceModel.ClientBase<ServiceReference1.IICUTech>, ServiceReference1.IICUTech
{
    static partial void ConfigureEndpoint(ServiceEndpoint serviceEndpoint, ClientCredentials clientCredentials)
    {       
        serviceEndpoint.Binding.SendTimeout = TimeSpan.FromMinutes(1);
       
        if (clientCredentials != null) 
        {
            clientCredentials.UserName.UserName = "myName";
            clientCredentials.UserName.Password = "12345678";
        }

    }
}