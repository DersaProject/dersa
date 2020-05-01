using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Nancy.ViewEngines.Razor;
using Nancy;

[assembly: OwinStartup(typeof(Dersa_N.Startup))]

namespace Dersa_N
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
            //app.Run(context =>
            //{
            //    context.Response.ContentType = "text/html; charset=utf-8";
            //    return context.Response.WriteAsync("<h2>Привет мир!</h2>");
            //});
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

    //public class CustomRootPathProvider : IRootPathProvider
    //{
    //    public string GetRootPath()
    //    {
    //        string path = AppDomain.CurrentDomain.BaseDirectory + "..\\";
    //        return path;
    //    }
    //}
}
