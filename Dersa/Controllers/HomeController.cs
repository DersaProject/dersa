using System.Reflection;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DIOS.Common.Interfaces;
using DIOS.Common;
using Dersa.Models;
using System.Net.Mail;
using System.Net;
using Dersa.Common;

namespace Dersa.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (!Dersa.Models.User.Exists(System.Web.HttpContext.Current.User.Identity.Name))
                    return RedirectToAction("Login", "Account");
                string userName = System.Web.HttpContext.Current.User.Identity.Name;
                ViewBag.Login = userName;
                DersaSqlManager DM = new DersaSqlManager();
                System.Data.DataTable T = DM.ExecuteSPWithParams("DERSA_USER$GetTextUserSetting", new object[] { userName, Util.GetPassword(userName), "toolbox JSON" });
                if (T != null && T.Rows.Count > 0)
                    ViewBag.ToolBoxData = T.Rows[0][0];
                else
                    ViewBag.ToolBoxData = "[]";
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
