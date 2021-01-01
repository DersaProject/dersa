using System.Reflection;
using System.Data;
using System.Linq;
using System.Web.Mvc;
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
		public ActionResult Index(int node = 0)
		{
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string userName = System.Web.HttpContext.Current.User.Identity.Name;
                if (!Dersa.Models.User.Exists(userName))
                {
                    return RedirectToAction("Unauthorized");
                }
                ViewBag.Login = userName;
                ViewBag.initialNodeId = node < 1 ? 0 : node;
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
        public ActionResult Unauthorized()
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name;
            ViewBag.Title = "сообщение системы авторизации";
            ViewBag.Message = "”важаемый пользователь " + userName + ", дл€ работы в системе DERSA нужно получить разрешение. ќбратитесь к администраторам системы.";
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
