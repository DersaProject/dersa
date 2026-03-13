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
using Microsoft.AspNetCore.Hosting.Server;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Dersa.Controllers
{
	public class HomeController : Controller
	{
        private IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        //public ActionResult Index()
        //{
        //    string filePath = Path.Combine(_environment.WebRootPath, "index.html");
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        var fileContent = System.IO.File.ReadAllText(filePath);
        //        return Content(fileContent, "text/html");
        //    }
        //    return NotFound(); // Если файл не найден
        //}
        public ActionResult Index()
        {
            //DIOS.Common.SqlManager.SqlBrand = DIOS.Common.SqlBrand.MSSqlServer;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (!Dersa.Models.User.Exists(HttpContext.User.Identity.Name))
                    return RedirectToAction("Login", "Account");
                string userName = HttpContext.User.Identity.Name;
                ViewBag.Login = userName;
                ViewBag.ToolBoxData = "[]";
                try
                {
//                    DersaSqlManager DM = new DersaSqlManager(DIOS.Common.SqlBrand.MSSqlServer);
                    DersaSqlManager DM = new DersaSqlManager();
                    System.Data.DataTable T = DM.ExecuteMethod("DERSA_USER", "GetTextUserSetting", new object[] { userName, DersaUtil.GetPassword(userName), "toolbox JSON" });
                    if (T != null)
                    {
                        string jsonData = (string)T.Rows[0][0];
                        JsonConvert.DeserializeObject(jsonData);
                        if (T.Rows.Count > 0)
                            ViewBag.ToolBoxData = jsonData;
                    }
                }
                catch (System.Exception exc)
                {
                }
                string filePath = Path.Combine(_environment.WebRootPath, "index.html");
                if (System.IO.File.Exists(filePath))
                {
                    var fileContent = System.IO.File.ReadAllText(filePath);
                    return Content(fileContent, "text/html");
                }
                return NotFound(); // Если файл не найден
                //                return View();
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
