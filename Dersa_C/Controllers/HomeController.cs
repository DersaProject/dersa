using System.Reflection;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DIOS.Common.Interfaces;
//using DIOS.Common;
using Dersa.Models;
using System.Net.Mail;
using System.Net;
using Dersa.Common;
using Newtonsoft.Json;

namespace Dersa.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
            if (true)//(System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //if (!Dersa.Models.User.Exists(System.Web."lanitadmin"/*HttpContext.Current.User.Identity.Name*/))
                //    return RedirectToAction("Login", "Account");
                string userName = "lanitadmin";//System.Web."lanitadmin"/*HttpContext.Current.User.Identity.Name*/;
                ViewBag.Login = userName;
                ViewBag.ToolBoxData = "[]";
                try
                {
                    DersaSqlManager DM = new DersaSqlManager(DIOS.Common.SqlBrand.ORACLE);
                    System.Data.DataTable T = DM.ExecuteMethod("DERSA_USER", "GetTextUserSetting", new object[] { userName, DersaUtil.GetPassword(userName), "toolbox JSON" });
                    string jsonData = (string)T.Rows[0][0];
                    JsonConvert.DeserializeObject(jsonData);
                    if (T != null && T.Rows.Count > 0)
                        ViewBag.ToolBoxData = jsonData;
                }
                catch(System.Exception exc)
                {
                }
                return View();
            }
            else
                return RedirectToAction("Login", "Account"); 
		}
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult LicenceMIT()
        {
            return View();
        }
        public ActionResult LicenceAPACHE()
        {
            return View();
        }

	}
}
