using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using System.Web.Mvc;
using System.Web.Http;
using DIOS.Common;
using System.Configuration;
using System.Web.Configuration;

[assembly: OwinStartup(typeof(Dersa_A.Startup))]

namespace Dersa_A
{
    public class Startup
    {
        private bool UserIsAuthenticated()
        {
            return true;
        }

        public void Configuration(IAppBuilder app)
        {

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@"..\content"),
            });

            // Only serve files requested by name.
            app.UseStaticFiles("/../content");

            app.UseWebApi(config);
            
            Configuration wconfig = ConfigurationManager.OpenExeConfiguration("");
            new SqlManagerConfigProvider(wconfig, new DIOS.Common.UserIsAuthenticatedMethod(UserIsAuthenticated));

        }

    }
}
