using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using System.Configuration;

namespace OAuthSample.Controllers
{
    public class AuthController : Controller
    {

        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext()
                           .Authentication
                           .Challenge(
                                new AuthenticationProperties { RedirectUri = "/" },
                                OpenIdConnectAuthenticationDefaults.AuthenticationType
                           );
            }
        }

        public void SignOut()
        {
            HttpContext.GetOwinContext()
                       .Authentication
                       .SignOut(
                            new AuthenticationProperties { RedirectUri = ConfigurationManager.AppSettings["ida:SignedOutCallbackPath"] },
                            OpenIdConnectAuthenticationDefaults.AuthenticationType,
                            CookieAuthenticationDefaults.AuthenticationType
                       );
        }
    }
}
