using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OAuthSample.Utils;
using System.Web.UI.WebControls;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using OAuthSample.Models;

namespace OAuthSample.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private HttpClient httpClient = new HttpClient();

        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public async Task<ActionResult> Validate()
        {
            ITokenAcquirer tokenAcquirer = TokenAcquirerFactory.GetDefaultInstance().GetTokenAcquirer();
            string[] scope = new[] { "user.read" };
            var access_token = await tokenAcquirer.GetTokenForUserAsync(scope);

            var irToken = await Authorize(access_token.IdToken, access_token.TenantId);
            AuthenticationProvider.Token = irToken;
            ViewBag.Message = $"Token issued by app server: {irToken}";

            return View();
        }

        private async Task<string> Authorize(string token, string tenantId)
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var authUri = $"https://epbyminw9084.epam.com:8093/api/authenticate/sso/client/c00195e0-df31-4825-b439-98472a32ff7b/tenant/{tenantId}";
            HttpContent content = new StringContent($"\"{token}\"", Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, authUri);
            request.Content = content;

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return responseContent;
        }

        [Authorize]
        public async Task<ActionResult> GetFile()
        {
            var fileId = 5500;
            var fileUri = $"https://epbyminw9084.epam.com:8093/api/files/{fileId}";
            var request = new HttpRequestMessage(HttpMethod.Get, fileUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("AccessToken", AuthenticationProvider.Token);
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var fileData = JsonConvert.DeserializeObject<FileResponse>(jsonString);

                return View("GetFile", fileData);
            }
            else
            {
                ViewBag.Error = $"Failed to retrieve file: {response.StatusCode}";
                return View("GetFile");
            }
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Name = ClaimsPrincipal.Current.FindFirst("name").Value;
            ViewBag.AuthorizationRequest = string.Empty;

            // The object ID claim will only be emitted for work or school accounts at this time.
            Claim oid = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
            ViewBag.ObjectId = oid == null ? string.Empty : oid.Value;

            // The 'preferred_username' claim can be used for showing the user's primary way of identifying themselves
            ViewBag.Username = ClaimsPrincipal.Current.FindFirst("preferred_username").Value;

            // The subject or nameidentifier claim can be used to uniquely identify the user
            ViewBag.Subject = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return View();
        }
    }
}