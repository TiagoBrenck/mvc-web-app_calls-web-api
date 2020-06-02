using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApp.Utils
{
    public static class Globals
    {
        public static string ClientId { get; } = ConfigurationManager.AppSettings["ida:ClientId"];

        /// <summary>
        /// The ClientSecret is a credential used to authenticate the application to Azure AD.  Azure AD supports password and certificate credentials.
        /// </summary>
        public static string ClientSecret { get; } = ConfigurationManager.AppSettings["ida:ClientSecret"];

        /// <summary>
        /// The Post Logout Redirect Uri is the URL where the user will be redirected after they sign out.
        /// </summary>
        public static string PostLogoutRedirectUri { get; } = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        /// <summary>
        /// The TenantId is the DirectoryId of the Azure AD tenant being used in the sample
        /// </summary>

        public static string RedirectUri { get; } = ConfigurationManager.AppSettings["ida:RedirectUri"];

        public static string TenantId { get; } = ConfigurationManager.AppSettings["ida:TenantId"];

        public static string Authority { get; } = ConfigurationManager.AppSettings["ida:AADInstance"] + TenantId + "/v2.0";

        public static string[] Scopes { get; } = new string[] { ConfigurationManager.AppSettings["ida:Scopes"] };

        public static string WebApiUrl { get; } = ConfigurationManager.AppSettings["ida:WebApiUrl"];
    }
}