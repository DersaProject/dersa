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
            Get("/", p => View["index.cshtml", null]);

            Get("/Node/List/{id}", p => NodeControllerAdapter.List(p.id));
        }

    }
}
