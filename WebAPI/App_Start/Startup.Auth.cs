using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using System.Configuration;

namespace WebAPI
{
	public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = ConfigurationManager.AppSettings["ida:TenantId"],
                    TokenValidationParameters = new TokenValidationParameters 
                    { 
                        SaveSigninToken = true, 
                        ValidAudience = ConfigurationManager.AppSettings["ida:Audience"] 
                    }
                });
        }
    }
}