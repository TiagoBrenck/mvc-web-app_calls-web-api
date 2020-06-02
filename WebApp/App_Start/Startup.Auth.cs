using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Identity.Client;
using System.Security.Claims;
using WebApp.Utils;
using Microsoft.IdentityModel.Tokens;

namespace WebApp
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = Globals.ClientId,
                    Authority = Globals.Authority,
                    PostLogoutRedirectUri = Globals.PostLogoutRedirectUri,
                    Notifications = new OpenIdConnectAuthenticationNotifications() 
                    { 
                        AuthorizationCodeReceived = OnAuthorizationCodeReceivedAsync,
                        AuthenticationFailed = OnAuthenticationFailed
                    },
                    TokenValidationParameters = new TokenValidationParameters 
                    { 
                        NameClaimType = "name"
                    }
                });
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            // Handle any unexpected errors during sign in
            context.OwinContext.Response.Redirect("/Error?message=" + context.Exception.Message);
            context.HandleResponse(); // Suppress the exception
            return Task.FromResult(0);
        }

        private async Task OnAuthorizationCodeReceivedAsync(AuthorizationCodeReceivedNotification notification)
        {
            notification.HandleCodeRedemption();

            IConfidentialClientApplication confidentialClient = MsalAppBuilder
                .BuildConfidentialClientApplication(new ClaimsPrincipal(notification.AuthenticationTicket.Identity));

            AuthenticationResult result = await confidentialClient
                .AcquireTokenByAuthorizationCode(Globals.Scopes, notification.Code).ExecuteAsync();

            notification.HandleCodeRedemption(null, result.IdToken);
        }

        private static string EnsureTrailingSlash(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            if (!value.EndsWith("/", StringComparison.Ordinal))
            {
                return value + "/";
            }

            return value;
        }
    }
}
