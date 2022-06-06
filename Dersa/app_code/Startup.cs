using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using System.Configuration;
using System.Web.Configuration;

[assembly: OwinStartupAttribute(typeof(DersaApplication.Startup))]
namespace DersaApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            if (configuration.AppSettings.Settings["CookieAuth"] != null && configuration.AppSettings.Settings["CookieAuth"].Value.ToLower() == "true")
            {//если сюда не попадаем, значит должна использоваться виндусовая аутентификация
                DIOS.Common.Logger.LogStatic("use cookie auth");
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Account/Login")
                });
                app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            }
        }

    }
}
