using System.Data;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Dersa.Models;
using Dersa.Common;


namespace Dersa.Controllers
{
    public class EntityController : Controller
    {

        public string List(string class_name, string filter = null, string order = "", int limit = -1, int offset = 0)
        {

            string result = (new EntityControllerAdapter(HttpContext.User.Identity.Name)).List(class_name, filter, order, limit, offset);
            return result;

        }

        //public string DownloadIcon(int id, bool from_stereotype)
        //{
        //    try
        //    {
        //        DersaSqlManager M = new DersaSqlManager();
        //        string sql = from_stereotype ? "select icon, name from STEREOTYPE where stereotype = " + id.ToString()
        //            : "select s.icon, s.name from STEREOTYPE s join ENTITY e on e.stereotype = s.stereotype where e.entity = " + id.ToString();
        //        System.Data.DataTable T = M.ExecSql(sql);
        //        Response.ContentType = "APPLICATION/OCTET-STREAM";
        //        string Header = "Attachment; Filename=" + T.Rows[0][1].ToString() + ".gif";
        //        Response.AppendHeader("Content-Disposition", Header);
        //        byte[] bts = (byte[])T.Rows[0][0];
        //        Response.OutputStream.Write(bts, 0, bts.Length);
        //        Response.End();
        //        return "OK";
        //    }
        //    catch (System.Exception exc)
        //    {
        //        Response.OutputStream.Flush();
        //        Response.OutputStream.Close();
        //        Response.ContentType = "TEXT/HTML";
        //        Response.ClearHeaders();
        //        Response.Write(exc.Message);
        //        return exc.Message;
        //    }

        //}

        public string GetPath(int id, int for_system)
        {
            return (new EntityControllerAdapter(HttpContext.User.Identity.Name)).GetPath(id, for_system);
        }

        public string Find(string srchval, int root_entity = 0, int entity = 0)
        {
            return (new EntityControllerAdapter(HttpContext.User.Identity.Name)).Find(srchval, root_entity, entity);
        }

        public void ClearCache()
        {
            CachedObjects.ClearCache();
        }

        public string Range(string ids)
        {
            return (new EntityControllerAdapter(HttpContext.User.Identity.Name)).Range(ids);
        }

        //public void IconForStereotype(string name)
        //{
        //    DersaSqlManager M = new DersaSqlManager();
        //    string sql = "select icon_path from STEREOTYPE where name = '" + name + "'";
        //    System.Data.DataTable T = M.ExecSql(sql);
        //    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, T.Rows[0][0].ToString().Replace("/", "\\"));

        //    using (Stream iconStream = new FileStream(filePath, FileMode.Open))
        //        try
        //        {
        //            Response.ContentType = "APPLICATION/OCTET-STREAM";
        //            string Header = "Attachment; Filename=" + name;
        //            Response.AppendHeader("Content-Disposition", Header);
        //            byte[] bts = new byte[iconStream.Length];
        //            iconStream.Read(bts, 0, bts.Length);
        //            Response.OutputStream.Write(bts, 0, bts.Length);
        //            Response.End();
        //        }
        //        catch (System.Exception exc)
        //        {
        //            Response.OutputStream.Flush();
        //            Response.OutputStream.Close();
        //            Response.ContentType = "TEXT/HTML";
        //            Response.ClearHeaders();
        //            Response.Write(exc.Message);
        //        }
        //}

        //public void Icon(int id)
        //{
        //    DersaSqlManager M = new DersaSqlManager();
        //    try
        //    {
        //        string sql = "select s.icon_path, s.name from STEREOTYPE s join ENTITIES e on e.stereotype = s.stereotype where e.entity = " + id.ToString();
        //        System.Data.DataTable T = M.ExecSql(sql);
        //        string filePath = (AppDomain.CurrentDomain.BaseDirectory + T.Rows[0][0].ToString().Replace("/", "\\")).Replace("\\\\", "\\");
        //        using (Stream iconStream = new FileStream(filePath, FileMode.Open))
        //        {
        //            Response.ContentType = "APPLICATION/OCTET-STREAM";
        //            string Header = "Attachment; Filename=" + T.Rows[0][1].ToString();
        //            Response.AppendHeader("Content-Disposition", Header);
        //            byte[] bts = new byte[iconStream.Length];
        //            iconStream.Read(bts, 0, bts.Length);
        //            Response.OutputStream.Write(bts, 0, bts.Length);
        //            Response.End();
        //        }
        //    }
        //    catch (System.Exception exc)
        //    {
        //        Response.OutputStream.Flush();
        //        Response.OutputStream.Close();
        //        Response.ContentType = "TEXT/HTML";
        //        Response.ClearHeaders();
        //        Response.Write(exc.Message);
        //    }
        //}
    }
}
