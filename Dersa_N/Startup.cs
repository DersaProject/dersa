using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Nancy.ViewEngines.Razor;
using Nancy;
using Nancy.Configuration;
using DIOS.Common;
using System.Configuration;
using Nancy.Bootstrapper;
using Nancy.Responses;
using Nancy.TinyIoc;
using System.Security.Claims;
using Nancy.Authentication.Forms;

[assembly: OwinStartup(typeof(Dersa_N.Startup))]

namespace Dersa_N
{
    public class Startup
    {
        private bool UserIsAuthenticated()
        {
            return !string.IsNullOrEmpty(UserDatabase.userName);
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@"..\www"),
            });

            app.UseStaticFiles("/../www");

            app.UseNancy();

            new SqlManagerConfigProvider(AppConfiguration.Config, new DIOS.Common.UserIsAuthenticatedMethod(UserIsAuthenticated));
        }
    }

    public class AppConfiguration
    {
        private static Configuration _config;
        public static Configuration Config
        {
            get
            {
                return _config;
            }
        }

        static AppConfiguration()
        {
            _config = ConfigurationManager.OpenExeConfiguration("");
        }
    }
    public class RazorConfig : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "System.Data";
            yield return "System.Web.Optimization";
            yield return "Newtonsoft.Json";
            yield return "System.Reflection";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "System.Data";
            yield return "System.Web.Optimization";
            yield return "Newtonsoft.Json";
            yield return "System.Reflection";
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

        private void S()
        {
            foreach (System.Reflection.PropertyInfo x in this.GetType().GetProperties())
            {
                //x.PropertyType
            }
        }
    }

    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "..\\";
            return path;
        }
    }

    public class NBootstrapper : DefaultNancyBootstrapper
    {
        public override void Configure(INancyEnvironment environment)
        {
            base.Configure(environment);
            environment.Tracing(false, true);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // We don't call "base" here to prevent auto-discovery of
            // types/dependencies
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            // Here we register our user mapper as a per-request singleton.
            // As this is now per-request we could inject a request scoped
            // database "context" or other request scoped services.
            container.Register<IUserMapper, UserDatabase>();
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            // At request startup we modify the request pipelines to
            // include forms authentication - passing in our now request
            // scoped user name mapper.
            //
            // The pipelines passed in here are specific to this request,
            // so we can add/remove/update items in them as we please.
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/login",
                    UserMapper = requestContainer.Resolve<IUserMapper>(),
                };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }

    }
}
