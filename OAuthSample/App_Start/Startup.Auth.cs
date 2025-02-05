using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.OWIN;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.IdentityModel.Validators;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;

namespace OAuthSample
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            OwinTokenAcquirerFactory owinTokenAcquirerFactory = TokenAcquirerFactory.GetDefaultInstance<OwinTokenAcquirerFactory>();

            owinTokenAcquirerFactory.Services
                 .Configure<ConfidentialClientApplicationOptions>(options =>
                 {
                     options.RedirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
                     options.ClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
                     options.ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
                     options.TenantId = ConfigurationManager.AppSettings["ida:TenantId"];
                     options.Instance = ConfigurationManager.AppSettings["ida:Instance"];
                 })
                 .AddInMemoryTokenCaches();

            owinTokenAcquirerFactory.Build();

            app.AddMicrosoftIdentityWebApp(owinTokenAcquirerFactory,
                                           updateOptions: options => { });
        }
    }
}