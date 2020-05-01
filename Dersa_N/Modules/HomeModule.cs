using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Newtonsoft.Json;

namespace Dersa_N
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", parameters =>
            {
                return View["index.cshtml", null];
            });

            Func<dynamic, object> nodeListFunc = GetNodesList;
            Get("/Node/List/{id}", nodeListFunc);
        }

        private object GetNodesList(dynamic parameters)
        {
            object result = new NodeControllerAdapter().List(parameters.id.ToString());
            return JsonConvert.SerializeObject(result);
        }
    }
}
