using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace NancyApp
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            //Get("/", param => "I'm Nancy Self Host Application.");
            Get("/", parameters => {
                return View["index.cshtml", null];
            });
        }
    }
}
