using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;

namespace Dersa
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801
    public class DersaApplication : System.Web.HttpApplication
    {
        private void DeleteViewFile(string view_name)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "Views/Temp/" + Response.Headers["view_name"];
            Response.Headers.Remove("view_name");
            if (System.IO.File.Exists(fileName))
            {
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch { }
            }
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_EndRequest() 
        {
            if(Response.Headers["view_name"] != null)
            {
                DeleteViewFile(Response.Headers["view_name"]);
            }
        }

        protected void Application_Error()
        {
            if (Response.Headers["view_name"] != null)
            {
                DeleteViewFile(Response.Headers["view_name"]);
            }
        }

    }
}