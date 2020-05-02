using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Dersa.Common;

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
            Post("/Node/SetProperties", p => NodeControllerAdapter.SetProperties(GetRequestData()));
            Get("/Node/PropertiesForm/{id}", p => NodeControllerAdapter.PropertiesForm(p.id));
            Get("/Node/Properties/{id}", p => NodeControllerAdapter.Properties(p.id));
            Get("/Node/PropertyForm/{id}/{prop_name}/{prop_type}", p => NodeControllerAdapter.PropertyForm(p.id, p.prop_name, p.prop_type));
            Post("/Node/SetTextProperty/{entity}/{prop_name}", p => NodeControllerAdapter.SetTextProperty(p.entity, p.prop_name, GetRequestData()));
            Get("/Node/MethodsForm/{id}", p => NodeControllerAdapter.MethodsForm(p.id));
            Get("/Node/ExecMethodForm/{id}/{method_name}", p => NodeControllerAdapter.ExecMethodForm(p.id, p.method_name));
            Get("/Node/DownloadString/{srcString}/{fileName}", p => DownloadObject(p.srcString, p.fileName));
            Get("/Node/DownloadMethodResult/{id}/{method_name}", p => DownloadObject(DersaUtil.ExecMethodResult(p.id, p.method_name), "emptyfile.txt"));
        }

        private string GetRequestData()
        {
            byte[] bts = new byte[this.Request.Body.Length];
            this.Request.Body.Read(bts, 0, bts.Length);
            return Encoding.Default.GetString(bts);
        }

        private object DownloadObject(object srcObject, string fileName)
        {
            byte[] bts = new byte[0];
            if (srcObject is string)
            {
                bts = System.Text.Encoding.Default.GetBytes((string)srcObject);
            }
            else if (srcObject is System.IO.FileInfo)
            {
                FileInfo fi = srcObject as System.IO.FileInfo;
                fileName = fi.Name;
                Stream S = fi.OpenRead();
                bts = new byte[S.Length];
                S.Read(bts, 0, bts.Length);
                S.Flush();
                S.Close();
                fi.Delete();
            }
            else if (srcObject is DynamicDictionaryValue)
            {
                DynamicDictionaryValue DV = srcObject as DynamicDictionaryValue;
                if (DV.HasValue)
                    bts = System.Text.Encoding.Default.GetBytes(DV.Value.ToString());
            }
            Stream outS = new MemoryStream(bts);
            //this.Response.Headers.Add("Content-Disposition", "attachment; filename=test.txt");
            //return this.Response.FromStream(outS, "application/force-download");
            var response = new Response();

            response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);
            response.ContentType = "application/force-download";
            response.Contents = stream => { stream.Write(bts, 0, bts.Length); };

            return response;
        }

    }
}
