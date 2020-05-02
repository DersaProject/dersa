using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Dersa.Common;
using Newtonsoft.Json;

namespace Dersa_N
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", p => {
                string userName = "localuser";
                ViewBag.Login = userName;
                try
                {
                    DersaSqlManager DM = new DersaSqlManager();
                    System.Data.DataTable T = DM.ExecuteMethod("DERSA_USER", "GetTextUserSetting", new object[] { userName, DersaUtil.GetPassword(userName), "toolbox JSON" });
                    string jsonData = (string)T.Rows[0][0];
                    JsonConvert.DeserializeObject(jsonData);
                    if (T != null && T.Rows.Count > 0)
                        ViewBag.ToolBoxData = jsonData;
                    else
                        ViewBag.ToolBoxData = "[]";
                }
                catch (System.Exception exc)
                {

                }
                return View["index.cshtml", null]; 
            });
            Get("/Node/UploadContent/{id}", p => { ViewBag.PackageId = p.id; return View["UploadContent.cshtml", null]; });
            Post("/Node/UploadContent", p => {
                string JsonContent = GetRequestFileAsString();
                SchemaEntity[] schemaContent = DersaUtil.GetSchema(JsonContent);
                for (int i = 0; i < schemaContent.Length; i++)
                {
                    SchemaEntity schemaEntity = schemaContent[i];
                    string entId = DersaUtil.CreateEntity(schemaEntity, this.Request.Form["id"], "localuser");
                }
                return View["Close"];
        });
            Get("/Node/List/{id}", p => NodeControllerAdapter.List(p.id));
            Get("/Node/GetInsertSubmenu/{id}", p => NodeControllerAdapter.GetInsertSubmenu(p.id));
            Get("/Node/CanDnD/{src}/{dst}", p => NodeControllerAdapter.CanDnD(p.src, p.dst).ToString());
            Post("/Node/Remove/{id}/{diagram_id}/{options}", p => NodeControllerAdapter.Remove(p.id, p.diagram_id, p.options));
            Post("/Node/Rename/{id}/{name}", p => NodeControllerAdapter.Rename(p.id, p.name));
            Post("/Node/DnD/{src}/{dst}/{options}", p => NodeControllerAdapter.DnD(p.src, p.dst, p.options));
            Get("/Node/Description/{id}/{attr_name}", p => NodeControllerAdapter.Description(p.id, p.attr_name));
            Post("/Node/SetProperties", p => NodeControllerAdapter.SetProperties(GetRequestBodyAsString()));
            Get("/Node/PropertiesForm/{id}", p => NodeControllerAdapter.PropertiesForm(p.id));
            Get("/Node/PropertiesForm?id={id}", p => NodeControllerAdapter.PropertiesForm(p.id));
            Get("/Node/Properties/{id}", p => NodeControllerAdapter.Properties(p.id));
            Get("/Node/PropertyForm/{id}/{prop_name}/{prop_type}", p => NodeControllerAdapter.PropertyForm(p.id, p.prop_name, p.prop_type));
            Post("/Node/SetTextProperty/{entity}/{prop_name}", p => NodeControllerAdapter.SetTextProperty(p.entity, p.prop_name, GetRequestBodyAsString()));
            Get("/Node/MethodsForm/{id}", p => NodeControllerAdapter.MethodsForm(p.id));
            Get("/Node/ExecMethodForm/{id}/{method_name}", p => NodeControllerAdapter.ExecMethodForm(p.id, p.method_name));
            Get("/Node/DownloadString/{srcString}/{fileName}", p => DownloadObject(p.srcString, p.fileName));
            Get("/Node/DownloadMethodResult/{id}/{method_name}", p => DownloadObject(DersaUtil.ExecMethodResult(p.id, p.method_name), p.method_name + "_" + p.id.ToString() + ".txt"));
            Get("/Query/GetAction/{MethodName}/{id}", p => QueryControllerAdapter.GetAction(p.MethodName, p.id));
            Get("/Query/GetAction/{MethodName}/{id}/{paramString}", p => QueryControllerAdapter.GetAction(p.MethodName, p.id, p.paramString));
            Get("/Account/JsSettings/{settingName}", p => AccountControllerAdapter.JsSettings(p.settingName));
            Get("/Account/JsSettings", p => AccountControllerAdapter.JsSettings(null));
            Post("/Account/SetUserSettings", p => AccountControllerAdapter.SetUserSettings(GetRequestBodyAsString()));
            Get("/Entity/GetPath/{id}/{for_system}", p => EntityControllerAdapter.GetPath(p.id, p.for_system));
            
        }

        private string GetRequestBodyAsString()
        {
            byte[] bts = new byte[this.Request.Body.Length];
            this.Request.Body.Read(bts, 0, bts.Length);
            return Encoding.UTF8.GetString(bts);
        }

        private string GetRequestFileAsString()
        {
            byte[] bts = new byte[this.Request.Files.ElementAt<HttpFile>(0).Value.Length];
            this.Request.Files.ElementAt<HttpFile>(0).Value.Read(bts, 0, bts.Length);
            return Encoding.Default.GetString(bts);
        }

        private object DownloadObject(object srcObject, string fileName)
        {
            byte[] bts = new byte[0];
            if (srcObject is string)
            {
                bts = System.Text.Encoding.UTF8.GetBytes((string)srcObject);
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
