using System.Data;
using System.Linq;
using System.Web.Mvc;
using Dersa.Models;
using System.Web;
using System;
using System.Collections.Generic;


namespace Dersa.Controllers
{
    public class NodeController : Controller
    {

        public string GetInsertSubmenu(int id)
        {
            return (new NodeControllerAdapter()).GetInsertSubmenu(id);
        }

        public int CanDnD(string src, int dst)
        {
            return (new NodeControllerAdapter()).CanDnD(src, dst);
        }

        public void DownloadMethodResult(int id, string method_name)
        {
            object execResult = (new NodeControllerAdapter()).ExecMethodResult(id, method_name);
            string file_name = "emptyfile.txt";
            byte[] bts = new byte[0];
            if (execResult is string)
            {
                file_name = method_name + "_" + id.ToString() + ".txt";
                bts = System.Text.Encoding.Default.GetBytes(execResult.ToString());
            }
            else if (execResult is System.IO.FileInfo)
            {
                System.IO.FileInfo fi = execResult as System.IO.FileInfo;
                file_name = fi.Name;
                System.IO.Stream S = fi.OpenRead();
                bts = new byte[S.Length];
                S.Read(bts, 0, bts.Length);
                S.Flush();
                S.Close();
                fi.Delete();
            }
            try
            {

                Response.ContentType = "APPLICATION/OCTET-STREAM";
                string Header = "Attachment; Filename=" + file_name;
                Response.AppendHeader("Content-Disposition", Header);
                Response.OutputStream.Write(bts, 0, bts.Length);
                Response.End();
            }
            catch (Exception exc)
            {
                Response.OutputStream.Flush();
                Response.OutputStream.Close();
                Response.ContentType = "TEXT/HTML";
                Response.ClearHeaders();
                Response.Write(exc.Message);
            }

        }

        [HttpPost]
        public ActionResult UploadContent(int id, IEnumerable<HttpPostedFileBase> fileUpload, bool closeWindow = false)
        {
            foreach (var file in fileUpload)
            {
                if (file == null) continue;
                byte[] fileContent = new byte[file.InputStream.Length];
                file.InputStream.Read(fileContent, 0, fileContent.Length);
                string JsonContent = System.Text.Encoding.Default.GetString(fileContent);
                NodeControllerAdapter.SchemaEntity[] schemaContent = NodeControllerAdapter.GetSchema(JsonContent);
                for (int i = 0; i < schemaContent.Length; i++)
                {
                    NodeControllerAdapter.SchemaEntity schemaEntity = schemaContent[i];
                    string entId = CreateEntity(schemaEntity.StereotypeName, id.ToString(), schemaEntity.Name, schemaEntity.schemaAttributes);
                    NodeControllerAdapter.SchemaEntity[] childEntities = schemaEntity.childEntities;
                    for (int c = 0; c < childEntities.Length; c++)
                    {
                        NodeControllerAdapter.SchemaEntity childEntity = childEntities[c];
                        CreateEntity(childEntity.StereotypeName, entId, childEntity.Name, childEntity.schemaAttributes);
                    }
                }
            }
            if (closeWindow)
                return View("Close");
            else
                return RedirectToAction("UploadContent/" + id.ToString());

        }

        private string CreateEntity(string stereotypeName, string parentId, string entityName, NodeControllerAdapter.SchemaAttribute[] attrs)
        {
            string entityCreateResult = DnD(stereotypeName, parentId, 0);  //"[{\"id\":10000361,\"text\":\"Entity\",\"icon\":\"Entity\",\"name\":\"Entity\"}]"; 
            dynamic ES = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(entityCreateResult);
            string resultId = "";
            if (ES.Count > 0)
            {
                resultId = ES[0].id.ToString();
                Rename(resultId, entityName);
            }
            Common.DersaSqlManager DM = new Common.DersaSqlManager();
            if (resultId != "" && attrs.Length > 0)
            {
                for (int a = 0; a < attrs.Length; a++)
                {
                    NodeControllerAdapter.SetAttribute(DM, "ENTITY$SetAttribute", resultId, attrs[a].Name, attrs[a].Value);
                }
            }
            return resultId;

        }

        public ActionResult UploadContent(int id)
        {
            ViewBag.PackageId = id;
            return View();

        }

        public string Restore(int id)
        {
            return (new NodeControllerAdapter()).Restore(id);
        }

        public string Close()
        {
            return "close";
        }

        public string List(string id)
        {
            return (new NodeControllerAdapter()).List(id);
        }

        public string Remove(int id, string diagram_id = null, int options = 0)
        {
            return (new NodeControllerAdapter()).Remove(id, diagram_id, options);
        }

        public string Rename(string id, string name)
        {
            return (new NodeControllerAdapter()).Rename(id, name);
        }

        public string DnD(string src, string dst, int options)
        {
            return (new NodeControllerAdapter()).DnD(src, dst, options);
        }

        public string Description(string id, string attr_name)
        {
            return (new NodeControllerAdapter()).Description(id, attr_name);
        }

        [HttpPost]
        public string SetProperties(string json_params)
        {
            return (new NodeControllerAdapter()).SetProperties(json_params);
        }

        [HttpPost]

        public string SetTextProperty(int entity, string prop_name, string prop_value)
        {
            return NodeControllerAdapter.SetTextProperty(entity, prop_name, prop_value);

        }

        public string PropertiesForm(int id)
        {
            return (new NodeControllerAdapter()).PropertiesForm(id);
        }

        public string PropertyForm(int id, string prop_name)
        {
            return (new NodeControllerAdapter()).PropertyForm(id, prop_name);
        }

        public string MethodsForm(int id)
        {
            return (new NodeControllerAdapter()).MethodsForm(id);
        }

        public string ExecMethodForm(int id, string method_name)
        {
            return (new NodeControllerAdapter()).ExecMethodForm(id, method_name);
        }

        public string ChildStereotypes(int id)
        {
            return (new NodeControllerAdapter()).ChildStereotypes(id);
        }

        public string Properties(int id)
        {
            return (new NodeControllerAdapter()).Properties(id);
        }
    }
}
