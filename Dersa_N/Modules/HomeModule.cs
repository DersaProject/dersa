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
            Get("/Node/GetInsertSubmenu/{id}", p => NodeControllerAdapter.GetInsertSubmenu(p.id));
            Get("/Node/CanDnD/{src}/{dst}", p => NodeControllerAdapter.CanDnD(p.src, p.dst).ToString());
            Post("/Node/Remove/{id}/{diagram_id}/{options}", p => NodeControllerAdapter.Remove(p.id, p.diagram_id, p.options));
            Post("/Node/Rename/{id}/{name}", p => NodeControllerAdapter.Rename(p.id, p.name));
            Post("/Node/DnD/{src}/{dst}/{options}", p => NodeControllerAdapter.DnD(p.src, p.dst, p.options));
            Get("/Node/Description/{id}/{attr_name}", p => NodeControllerAdapter.Description(p.id, p.attr_name));
            Post("/Node/SetProperties", new Func<dynamic, object>(GetPostData));//p => NodeControllerAdapter.SetProperties(p.Request.Body));
            Get("/Node/PropertiesForm/{id}", p => NodeControllerAdapter.PropertiesForm(p.id));
            Get("/Node/Properties/{id}", p => NodeControllerAdapter.Properties(p.id));

        }

        private object GetPostData(dynamic x)
        {
            byte[] bts = new byte[this.Request.Body.Length];
            this.Request.Body.Read(bts, 0, bts.Length);
            string jsonString = Encoding.Default.GetString(bts);
            NodeControllerAdapter.SetProperties(jsonString);
            return null;
        }

    }
}
