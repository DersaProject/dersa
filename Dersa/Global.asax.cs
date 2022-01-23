using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
//using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Web.Configuration;
using DIOS.WCF.Core;

namespace Dersa
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801
    public class DersaApplication : System.Web.HttpApplication
    {
        private static string StartErrorText = "";
        private void DeleteViewFile(string view_name)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "Views/Temp/" + Response.Headers["view_name"];
            Response.Headers.Remove("view_name");
            if (System.IO.File.Exists(fileName))
            {
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch { }
            }
        }
        private bool UserIsAuthenticated()
        {
            //if (_dbManager.IsRunningInHttpContext)
            //{
            //    if (UserName == "" && WcfCoreUtil.GetCurrentUserName() == "" && !Anonimous)
            //        throw new UnauthorizedAccessException("Операция запрещена для неавторизованных пользователей.");
            //}
            return System.Web.HttpContext.Current.User.Identity.IsAuthenticated || WcfCoreUtil.GetCurrentUserName() != "";
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            new DIOS.Common.SqlManagerConfigProvider(configuration, new DIOS.Common.UserIsAuthenticatedMethod(UserIsAuthenticated));
            try
            {
                var Params = new DIOS.Common.ParameterCollection();
                var M = new Dersa.Common.DersaAnonimousSqlManager();
                var LM = new Dersa.Common.DersaLogSqlManager();
                int Id = M.ExecuteIntMethod("SYSTEM_START", "LogCurrent", Params);
                Params.Add("start_number", Id);
                int result = LM.ExecuteIntMethod("SYSTEM_START", "LogCurrent", Params);
                if(result < 0)
                    StartErrorText = string.Format("Проблема при старте с номером {0}, проверьте логи.", Id);
            }
            catch(Exception exc)
            {
                DIOS.Common.Logger.LogStatic(string.Format("Ошибка инициализации лога: {0}", exc.Message));
                StartErrorText = string.Format("База данных лога не настроена!");
            }
        }
        protected void Application_BeginRequest()
        {
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            if (Request.Path == "/")
            {
                string LogCopyErrorText = "";
                try
                {
                    var LM = new Dersa.Common.DersaLogSqlManager();
                    int result = LM.ExecuteIntMethod("ENTITY_LOG", "Move", new DIOS.Common.ParameterCollection());
                }
                catch (Exception exc)
                {
                    DIOS.Common.Logger.LogStatic(string.Format("Ошибка копирования лога: {0}", exc.Message));
                    LogCopyErrorText += string.Format(" Ошибка копирования лога сущностей");
                }
                try
                {
                    var LM = new Dersa.Common.DersaLogSqlManager();
                    int result = LM.ExecuteIntMethod("ATTRIBUTE_LOG", "Move", new DIOS.Common.ParameterCollection());
                }
                catch (Exception exc)
                {
                    DIOS.Common.Logger.LogStatic(string.Format("Ошибка копирования лога: {0}", exc.Message));
                    LogCopyErrorText += string.Format(" Ошибка копирования лога атрибутов");
                }
                if (configuration.AppSettings.Settings["DisplayErrorsOnRefresh"] == null || configuration.AppSettings.Settings["DisplayErrorsOnRefresh"].Value.ToLower() == "true")
                {
                    if (StartErrorText != "" || LogCopyErrorText != "")
                        Response.Write(string.Format("<b><span style='color:red;'>{0}</span></b><br>", StartErrorText + LogCopyErrorText));
                }
            }
        }

        protected void Application_EndRequest()
        {
            if (Response.Headers["view_name"] != null)
            {
                DeleteViewFile(Response.Headers["view_name"]);
            }
        }

        protected void Application_Error()
        {
            if (Response.Headers["view_name"] != null)
            {
                DeleteViewFile(Response.Headers["view_name"]);
            }
        }

    }
}