using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dersa.Models;
using Dersa.Common;
using System.Web;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Dersa.Controllers
{
    public class NodeController : Controller
    {
        public string GetInsertSubmenu(int id)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).GetInsertSubmenu(id);
        }

        public int CanDnD(string src, int dst)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).CanDnD(src, dst);
        }

        //public void DownloadMethodResult(int id, string method_name)
        //{
        //    object execResult = DersaUtil.ExecMethodResult(id, method_name);
        //    string file_name = "emptyfile.txt";
        //    byte[] bts = new byte[0];
        //    if (execResult is string)
        //    {
        //        file_name = method_name + "_" + id.ToString() + ".txt";
        //        bts = System.Text.Encoding.Default.GetBytes(execResult.ToString());
        //    }
        //    else if (execResult is System.IO.FileInfo)
        //    {
        //        System.IO.FileInfo fi = execResult as System.IO.FileInfo;
        //        file_name = fi.Name;
        //        System.IO.Stream S = fi.OpenRead();
        //        bts = new byte[S.Length];
        //        S.Read(bts, 0, bts.Length);
        //        S.Flush();
        //        S.Close();
        //        fi.Delete();
        //    }
        //    try
        //    {

        //        Response.ContentType = "application/force-download";
        //        //Response.ContentType = "APPLICATION/OCTET-STREAM";
        //        string Header = "Attachment; Filename=" + file_name;
        //        Response.AppendHeader("Content-Disposition", Header);
        //        Response.OutputStream.Write(bts, 0, bts.Length);
        //        Response.End();
        //    }
        //    catch (Exception exc)
        //    {
        //        Response.OutputStream.Flush();
        //        Response.OutputStream.Close();
        //        Response.ContentType = "TEXT/HTML";
        //        Response.ClearHeaders();
        //        Response.Write(exc.Message);
        //    }

        //}

        //[HttpPost]
        //public ActionResult UploadContent(int id, IEnumerable<HttpPostedFileBase> fileUpload, bool closeWindow = false)
        //{
        //    string userName = HttpContext.User.Identity.Name;
        //    foreach (var file in fileUpload)
        //    {
        //        if (file == null) continue;
        //        byte[] fileContent = new byte[file.InputStream.Length];
        //        file.InputStream.Read(fileContent, 0, fileContent.Length);
        //        string JsonContent = System.Text.Encoding.Default.GetString(fileContent);
        //        SchemaEntity[] schemaContent = DersaUtil.GetSchema(JsonContent);
        //        for (int i = 0; i < schemaContent.Length; i++)
        //        {
        //            SchemaEntity schemaEntity = schemaContent[i];
        //            string entId = DersaUtil.CreateEntity(schemaEntity, id.ToString(), userName);
        //        }
        //    }
        //    if (closeWindow)
        //        return View("Close");
        //    else
        //        return RedirectToAction("UploadContent/" + id.ToString());

        //}

        [HttpPost]
        public ActionResult UploadContentFromUrl(int id, string uploadUrl, bool closeWindow = false)
        {
            //foreach (var file in fileUpload)
            //{
            //    if (file == null) continue;
            //    byte[] fileContent = new byte[file.InputStream.Length];
            //    file.InputStream.Read(fileContent, 0, fileContent.Length);
            //    string JsonContent = System.Text.Encoding.Default.GetString(fileContent);
            //    NodeControllerAdapter.SchemaEntity[] schemaContent = NodeControllerAdapter.GetSchema(JsonContent);
            //    for (int i = 0; i < schemaContent.Length; i++)
            //    {
            //        NodeControllerAdapter.SchemaEntity schemaEntity = schemaContent[i];
            //        string entId = CreateEntity(schemaEntity, id.ToString());
            //    }
            //}
            string userName = HttpContext.User.Identity.Name;

            System.Net.HttpWebRequest req = System.Net.HttpWebRequest.CreateHttp(uploadUrl);
            System.Net.HttpWebRequest authReq = System.Net.HttpWebRequest.CreateHttp("http://" + req.Address.Authority + "/Account/aspNetCookie?login=" + userName);
            authReq.Method = "GET";
            var resp = authReq.GetResponse();
            var respStream = resp.GetResponseStream();
            var SR = new System.IO.StreamReader(respStream);
            string authCookie = SR.ReadToEnd();
            req.Method = "GET";
            req.CookieContainer = new System.Net.CookieContainer();
            System.Net.Cookie cookie = new System.Net.Cookie(".AspNet.ApplicationCookie", authCookie, "/", req.Address.Host);
            //Request.Cookies[".AspNet.ApplicationCookie"].Value, "/", req.Address.Host);
            req.CookieContainer.Add(cookie);
            resp = req.GetResponse();
            respStream = resp.GetResponseStream();
            SR = new System.IO.StreamReader(respStream);
            string JsonContent = SR.ReadToEnd();
            SchemaEntity[] schemaContent = DersaUtil.GetSchema(JsonContent);
            if (schemaContent != null)
            {
                for (int i = 0; i < schemaContent.Length; i++)
                {
                    SchemaEntity schemaEntity = schemaContent[i];
                    string entId = DersaUtil.CreateEntity(schemaEntity, id.ToString(), userName);
                }
            }

            if (closeWindow)
                return View("Close");
            else
                return RedirectToAction("UploadContent/" + id.ToString());

        }

        public ActionResult UploadContent(int id)
        {
            ViewBag.PackageId = id;
            return View();

        }

        public ActionResult UploadContentFromUrl(int id)
        {
            ViewBag.PackageId = id;
            return View();

        }

        public string Restore(int id)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).Restore(id);
        }

        public string Close()
        {
            return "close";
        }

        public string List(string id)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).List(id);
        }

        public string Remove(int id, string diagram_id = null, int options = 0)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).Remove(id, diagram_id, options);
        }

        public string Rename(string id, string name)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).Rename(id, name);
        }

        public string DnD(string src, string dst, int options)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).DnD(src, dst, options);
        }

        public string Description(string id, string attr_name)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).Description(id, attr_name);
        }

        [HttpPost]
        public string SetProperties(string json_params)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).SetProperties(json_params);
        }

        [HttpPost]

        public string SetTextProperty(int entity, string prop_name, string prop_value)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).SetTextProperty(entity, prop_name, prop_value);

        }

        public string PropertiesForm(int id)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).PropertiesForm(id);
        }

        public string PropertyForm(int id, string prop_name, int prop_type)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).PropertyForm(id, prop_name, prop_type);
        }

        public string MethodsForm(int id)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).MethodsForm(id);
        }

        public string ExecMethodForm(int id, string method_name)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).ExecMethodForm(id, method_name);
        }

        public string ChildStereotypes(int id)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).ChildStereotypes(id);
        }

        public string Properties(int id)
        {
            return (new NodeControllerAdapter(HttpContext.User.Identity.Name)).Properties(id);
        }
    }
}
