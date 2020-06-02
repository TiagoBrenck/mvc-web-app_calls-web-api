using Microsoft.Identity.Client;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebApp.Utils;

namespace WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CallAPI()
        {
            try
            {
                string json = await CallWebApiHelper(Globals.WebApiUrl + "api/todo");
                var apiResult = JsonConvert.DeserializeObject<List<string>>(json);

                return View(apiResult);
            }
            catch (MsalUiRequiredException)
            {
                return RedirectToAction("SignIn", "Account", new { redirectUrl = "/Home/CallAPI" });
            }
        }

        private async Task<string> CallWebApiHelper(string apiUrl)
        {
            var app = MsalAppBuilder.BuildConfidentialClientApplication();

            var userAccount = await app.GetAccountAsync(ClaimsPrincipal.Current.GetMsalAccountId());

            if (userAccount == null)
            {
                // Dealing with guest users
                var accounts = await app.GetAccountsAsync();
                userAccount = accounts.Where(x => x.Username == ClaimsPrincipal.Current.GetLoginHint())
                    .FirstOrDefault();
            }

            var result = await app.AcquireTokenSilent(Globals.Scopes, userAccount)
                .ExecuteAsync()
                .ConfigureAwait(false);

            // Construct the query
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

            // Ensure a successful response
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }
}