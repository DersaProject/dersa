using System.Reflection;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DIOS.Common.Interfaces;

namespace CuksWebApiApplication.Controllers
{
	public class AdminController : Controller
	{

		[MethodPermissionsAttribute("admin")]
		public ActionResult Index()
		{
			MethodInfo method = this.GetType().GetMethod("Index", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			//string canExec = ObjectMethods.CanExecuteMethod(method);
			//if (canExec != "")
			//	return null;
            return View();
		}

	}
}
