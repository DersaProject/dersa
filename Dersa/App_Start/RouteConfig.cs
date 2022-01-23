using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dersa
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Project",
            //    url: "Project/{action}/{class_name}",
            //    defaults: new { controller = "Project", action = "Index", class_name = UrlParameter.Optional }
            //);
            routes.MapRoute(
                name: "ProjectByName",
                url: "Project/UserProject/{ProjectName}",
                defaults: new { controller = "Project", action = "UserProject", ProjectName = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ConfigByPath",
                url: "Query/{config_folder}/GetAction/{id}/{MethodName}",
                defaults: new { controller = "Query", action = "GetActionWithConfig", config_folder = UrlParameter.Optional, id = UrlParameter.Optional, MethodName = UrlParameter.Optional }
            );
        }
    }
}