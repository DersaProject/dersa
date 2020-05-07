using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Nancy.ViewEngines.Razor;
using Nancy;
using DIOS.Common;
using System.Configuration;

[assembly: OwinStartup(typeof(Dersa_N.Startup))]

namespace Dersa_N
{
    public class Program
    {
        public static void Run()
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<Startup>("http://localhost:8000"))
            {
                Console.WriteLine("Сервер запущен. Нажмите любую клавишу для завершения работы...");
                Console.ReadKey();
            }
        }
    }

    public class Startup
    {
        private bool UserIsAuthenticated()
        {
            return true;
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

            Configuration wconfig = ConfigurationManager.OpenExeConfiguration("");
            new SqlManagerConfigProvider(wconfig, new DIOS.Common.UserIsAuthenticatedMethod(UserIsAuthenticated));
        }
    }

    public class RazorConfig : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "System.Data";
            yield return "System.Web.Optimization";
            yield return "Newtonsoft.Json";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "System.Data";
            yield return "System.Web.Optimization";
            yield return "Newtonsoft.Json";
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
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
}
