using System.Data;
using System.Linq;
using System.Web.Mvc;
using Dersa.Models;
using Dersa.Common;
using Newtonsoft.Json;
using System.Collections;
using DIOS.Common;
using DIOS.Common.Interfaces;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Web;


namespace Dersa.Controllers
{
    public class ProjectController : Controller
    {

        public string CreateDir(string name)
        {
            return (new ProjectControllerAdapter()).CreateDir(name);
        }

        public ActionResult UploadFile()
        {
            return View();

        }

        public string CreateTextFile(string json_params)
        {
            return (new ProjectControllerAdapter()).CreateTextFile(json_params);
        }

        [HttpPost]
        public void UploadFile(IEnumerable<HttpPostedFileBase> fileUpload, int entityId)
        {
            foreach (var file in fileUpload)
            {
                if (file == null) continue;
                if (!file.ContentType.Contains("image/"))
                    throw new System.Exception("Файл не является изображением");
                byte[] fileContent = new byte[file.InputStream.Length];
                file.InputStream.Read(fileContent, 0, fileContent.Length);
                System.Tuple<string, string> fileNameInfo = ProjectControllerAdapter.GetPath(file.FileName); 
                System.IO.File.WriteAllBytes(fileNameInfo.Item1, fileContent);
                try
                {
                    ProjectControllerAdapter.SetImagePath(entityId, fileNameInfo.Item2);
                }
                catch(System.Exception exc)
                { }
            }
        }

        [HttpPost]
        public ActionResult UploadFileFromForm(IEnumerable<HttpPostedFileBase> fileUpload)
        {
            foreach (var file in fileUpload)
            {
                if (file == null) continue;
                byte[] fileContent = new byte[file.InputStream.Length];
                file.InputStream.Read(fileContent, 0, fileContent.Length);
                string path = ProjectControllerAdapter.GetPath(file.FileName).Item1;
                System.IO.File.WriteAllBytes(path, fileContent);
            }
            return View("Close");
        }

        public ActionResult Display(string class_name)
        {
            DersaUserSqlManager M = new DersaUserSqlManager();
            System.Data.DataTable T = M.ExecuteMethod("OBJ", "List", new object[] { class_name });
            try
            {
                ActionResult CV = View(class_name, T);
                CV.ExecuteResult(this.ControllerContext);
                return null;
            }
            catch
            {
                ViewBag.ClassName = class_name;
                ViewBag.KeyName = class_name.ToLower();
                return View("Table", T);
            }

        }

        public ActionResult Index()
        {
            DersaUserSqlManager M = new DersaUserSqlManager();
            System.Data.DataTable T = M.ExecSql("select class_name from OBJECT_TYPE");
            return View(T);
        }

        public string ObjectInfo(string class_name, int id)
        {
            DersaUserSqlManager M = new DersaUserSqlManager();
            System.Data.DataTable T = M.ExecuteMethod("OBJ", "Info", new object[] { class_name, id });
            System.Data.DataRow R = T.Rows.Count > 0 ? T.Rows[0] : null;
            var query =
                from System.Data.DataColumn C in T.Columns
                select new
                {
                    Name = C.ColumnName,
                    Value = R == null ? null : R[C],
                    ReadOnly = C.ColumnName == class_name.ToLower(),
                    WriteUnchanged = C.ColumnName == class_name.ToLower(),
                    Type = 1,
                    ControlType = "text"
                    //ChildFormAttrs = (int)R["Type"] == 1 ? null : new
                    //{
                    //    Height = 900,
                    //    Width = 600,
                    //    DisplayValue = (int)R["Type"] == 1 ? R["Value"] : "...",
                    //    InfoLink = (int)R["Type"] == 1 ? "" : "Node/PropertyForm?id=" + id.ToString() + "&prop_name=" + R["Name"].ToString()
                    //}
                };
            string result = JsonConvert.SerializeObject(query);
            return result;

        }

        public string ObjectUpdateOrInsert(string class_name, string json_params)
        {
            try
            {
                DersaUserSqlManager M = new DersaUserSqlManager();
                IParameterCollection Params = Util.DeserializeParams(json_params);
                bool doInsert = Params[class_name.ToLower()].Value.ToString() == "";
                string keyName = class_name.ToLower();
                if (!Params.Contains(keyName))
                    return "no key";
                System.Data.DataTable T = M.ExecuteMethod("OBJ", "Info", new object[] { class_name, Params[class_name.ToLower()].Value });
                if (T.Rows.Count < 1)
                {
                    if (!doInsert)
                        return "no object";
                    System.Data.DataRow newR = T.NewRow();
                    T.Rows.Add(newR);
                }
                ArrayList changedFields = new ArrayList();
                foreach (IParameter Param in Params)
                {
                    if (!doInsert || Param.Name != keyName)
                    {
                        T.Rows[0][Param.Name] = Param.Value;
                        changedFields.Add(Param.Name);
                    }
                }
                string dbName = M.DatabaseName;
                if (!dbName.Contains("["))
                    dbName = "[" + dbName + "]";
                //if(doInsert)
                //    M.InsertTable(dbName, class_name, class_name.ToLower(), T, changedFields);
                //else
                //    M.UpdateTable(dbName, class_name, class_name.ToLower(), T, changedFields);
                return "";
            }
            catch (System.Exception exc)
            {
                return exc.Message;
            }

        }

        public string ObjectDrop(string class_name, int id)
        {
            DersaUserSqlManager M = new DersaUserSqlManager();
            try
            {
                M.ExecuteMethod("OBJ", "Drop", new object[] { class_name, id });
                return "";
            }
            catch (System.Exception exc)
            {
                return exc.Message;
            }
        }
    }
}
