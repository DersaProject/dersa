using System.Data;
using System.Linq;
using System.Web.Mvc;
using Dersa.Models;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Dersa.Common;
using Newtonsoft.Json;
using System.IO;
using System;


namespace Dersa.Controllers
{
    public class QueryController : Controller
    {

        public void DownloadSavedHtml(string Id, bool doCompress = false, string fileName = "result.html", string password = "")
        {
            try
            {
                string content = DersaUtil.GetString(Id, true);
                Response.ContentType = "application/force-download";
                if (doCompress)
                {
                    dynamic fileArray = JsonConvert.DeserializeObject(content);
//                    Response.ContentType = "application/zip";
                    Response.AppendHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
                    using (Ionic.Zip.ZipOutputStream zOut = new Ionic.Zip.ZipOutputStream(Response.OutputStream))
                    {
                        if (password != "")
                            zOut.Password = password;
                        foreach (dynamic fileUnit in fileArray)
                        {
                            string fName = fileUnit.name;
                            zOut.PutNextEntry(fName);
                            string fileBody = fileUnit.data;
                            byte[] bts = System.Convert.FromBase64String(fileBody);
                            zOut.Write(bts, 0, bts.Length);
                        }
                    }
                }
                else
                {
                    string Header = "Attachment; Filename=" + fileName;
                    Response.AppendHeader("Content-Disposition", Header);
                    using (var SW = new System.IO.StreamWriter(Response.OutputStream, System.Text.Encoding.Default))
                    {
                        SW.Write(content);
                        SW.Flush();
                        SW.Close();
                    }
                }
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
        public void DownloadContent(string urlId, string fileName="result.html") 
        {
            try
            {
                string url = DersaUtil.GetString(urlId, true);
                System.Net.HttpWebRequest req = System.Net.HttpWebRequest.CreateHttp(url);
                req.Method = "GET";
                req.Timeout = 600000;
                req.CookieContainer = new System.Net.CookieContainer();
                System.Net.Cookie cookie = new System.Net.Cookie(".AspNet.ApplicationCookie", Request.Cookies[".AspNet.ApplicationCookie"].Value, "/", Request.UrlReferrer.Host);
                req.CookieContainer.Add(cookie);
                var resp = req.GetResponse();
                var respStream = resp.GetResponseStream();
                var SR = new System.IO.StreamReader(respStream);
                string result = SR.ReadToEnd();
                Response.ContentType = "application/force-download";
                string Header = "Attachment; Filename=" + fileName;
                Response.AppendHeader("Content-Disposition", Header);
                var SW = new System.IO.StreamWriter(Response.OutputStream, System.Text.Encoding.Default);
                SW.Write(result);
                SW.Flush();
                SW.Close();
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

        public ActionResult GetViewByName(string view_name, string json_params)
        {
            IParameterCollection Params = DIOS.Common.Util.DeserializeParams(json_params);
            return GetViewByName(view_name, Params, false);
        }

        public ActionResult GetViewById(string cshtmlId)
        {
            IParameterCollection Params = (new QueryControllerAdapter()).GetViewParams(cshtmlId);
            if (Params.Contains("view_name"))
            {
                string view_name = Params["view_name"].Value.ToString();
                return GetViewByName(view_name, Params);
            }
            return null;
        }
        public ActionResult GetView(string json_params)
        {
            IParameterCollection Params = Util.DeserializeParams(json_params);
            if (!Params.Contains("cshtml"))
                return null;
            string view_name = (new QueryControllerAdapter()).GetViewName(Params["cshtml"].Value.ToString());
            return GetViewByName(view_name, Params);
        }

        private ActionResult GetViewByName(string view_name, IParameterCollection Params, bool doDeleteView = true)
        {
            foreach (IParameter Param in Params)
            {
                if (Param.Name != "cshtml")
                    ViewData[Param.Name] = Param.Value;
            }
            ActionResult V = View("~/Views/Temp/" + view_name);
            if (doDeleteView)
                Response.Headers.Add("view_name", view_name);
            return V;
        }

        public void DownloadReport(int id, string parameters)
        {
            DersaSqlManager M = new DersaAnonimousSqlManager();
            IParameterCollection Params = Util.DeserializeParams(parameters);
            if (Params.Contains("proc_name"))
            {
                StreamWriter SW = null;
                string proc_name = Params["proc_name"].Value.ToString();
                Params.Remove("proc_name");
                try
                {
                    Response.ContentType = "application/force-download; charset =windows-1251";
                    string Header = "Filename=" + "report_" + id.ToString() + ".csv";  //Attachment; 
                    Response.AppendHeader("Content-Disposition", Header);

                    //MemoryStream S = new MemoryStream();
                    //SW = new StreamWriter(S);
                    SW = new StreamWriter(Response.OutputStream, System.Text.Encoding.Default);
                    M.ExecSqlToStream(proc_name, SW, Params);
                    SW.Close();
                    //string result = System.Text.Encoding.UTF8.GetString(S.ToArray());
                    //byte[] btres = System.Text.Encoding.Default.GetBytes(result);
                    //Response.AppendHeader("Content-Length", btres.Length.ToString());
                    //Response.OutputStream.Write(btres, 0, btres.Length);
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
            else
                throw new System.Exception("procedure for report is not defined!");

        }

        public ActionResult Report(string class_name, string method_name, string parameters)
        {
            DersaSqlManager M = new DersaSqlManager();
            IParameterCollection Params = Util.DeserializeParams(parameters);
            if (Params.Contains("proc_name") || !string.IsNullOrEmpty(method_name))
            {
                if (Params.Contains("proc_name"))
                {
                    method_name = Params["proc_name"].Value.ToString();
                    Params.Remove("proc_name");
                }
                System.Data.DataTable T = M.ExecuteMethod(class_name,  method_name, Params);
                return View(T);
            }
            else
                throw new System.Exception("procedure for report is not defined!");
        }

        public string DebugParams(string json_params)
        {
            //object[] htmlArr = new object[] { new { Name = "html", Value = json_params } };
            //return OpenHtml(JsonConvert.SerializeObject(htmlArr));
            string Id = PutHtml(json_params);
            var result = new { action = "window.open('Query/GetHtml?Id=" + Id + "&viewSource=true','user html', 'width=400,height=400,status=1,menubar=1');" };
            return JsonConvert.SerializeObject(result);
        }

        public string ReportParams(string json_params)
        {
            return (new QueryControllerAdapter()).ReportParams(json_params);
        }

        public string GetActionForParams(string json_params)
        {
            return QueryControllerAdapter.GetActionForParams(json_params);
        }

        public string GetAction(string MethodName, int id, string paramString = null, bool redirect = true) 
        {
            try
            {
                return QueryControllerAdapter.GetAction(MethodName, id, paramString);
            }
            catch(Exception exc)
            {
                while (exc.InnerException != null)
                    exc = exc.InnerException;
                if (redirect && exc is RouteRedirectException)
                {
                    string configFolder = (exc as RouteRedirectException).configFolder;
                    Response.Redirect("/Query/" + configFolder + "/GetAction/" + id.ToString() + "/" + MethodName + "?paramString=" + paramString);
                    return "redirected";
                }
                else
                    return "alert('" + exc.Message + "');";
            }
        }

        public string GetActionWithConfig(string config_folder, string MethodName, int id, string paramString = null)
        {
            return GetAction(MethodName, id, paramString, false);
            //return config_folder + "_" + MethodName + "_" + id.ToString();
            //return Request.Path;
        }
        //public string GetQueryId(string query)
        //{
        //    return QueryControllerAdapter.GetQueryId(query);
        //}

        public ActionResult Table(string table_name, string db_name = null, string order = null)
        {
            if (table_name.Contains(";") || table_name.Contains("--") || table_name.Contains(" "))
                return null;
            DersaSqlManager M = new DersaSqlManager();
            if (db_name != null)
                M.SetDatabaseName(db_name);
            string query = string.Format("select top 1000 * from {0}(nolock)", table_name);
            if (!string.IsNullOrEmpty(order))
            {
                order = order.Replace(" desc", "____desc");
                if (order.Contains(";") || order.Contains("--") || order.Contains(" "))
                    return null;
                order = order.Replace("____desc", " desc");
                query += (" order by " + order);
            }
            System.Data.DataTable T = M.ExecSql(query, null, true);//.GetSqlObject(table_name, "", 1000);//ObjectMethods.ExecProc("REPORT$WorkplaceList");
            return View(T);

        }

        public string GetText(string json_params)
        {
            return (new QueryControllerAdapter()).GetText(json_params);
        }

        public string OpenHtml(string json_params)
        {
            return (new QueryControllerAdapter()).OpenHtml(json_params);
        }

        public string PutCSHtml(string json_params, bool addController = true)
        {
            string resultId = DersaUtil.PutString(json_params);
            if (addController)
                resultId = "/Query/GetViewById?cshtmlId=" + resultId;
            return resultId;
        }

        public string PutHtml(string html)
        {
            return DersaUtil.PutString(html);
        }

        public string PutText(string text)
        {
            return DersaUtil.PutString(text);
        }

        public string GetHtml(string Id, bool viewSource = false)
        {
            return DersaUtil.GetString(Id, viewSource);
        }

        [HttpPost]
        public string ExecSql(string json_params)
        {
            return (new QueryControllerAdapter()).ExecSql(json_params);
        }

        public ActionResult LogTable()
        {
            System.Data.DataTable T = new System.Data.DataTable();
            if (Logger.LogTable != null)
            {
                object[] valArray = new object[Logger.LogTable.Keys.Count];
                int i = 0;
                foreach (object key in Logger.LogTable.Keys)
                {
                    valArray[i++] = Logger.LogTable[key];
                }
                string logTable = JsonConvert.SerializeObject(valArray);
                T = JsonConvert.DeserializeObject(logTable, typeof(System.Data.DataTable)) as System.Data.DataTable;
                DataView dv = T.DefaultView;
                if (T.Columns.Contains("occur"))
                    dv.Sort = "occur Asc";
                T = dv.ToTable().Copy();
            }
            return View("Table", T);
        }
    }
}
