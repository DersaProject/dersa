using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DIOS.Common.Interfaces;
using Dersa.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;


namespace Dersa.Controllers
{
	public class AccountController : Controller
	{

        private static Hashtable cookieTable = new Hashtable();


        public ActionResult Activate(string token)
		{
            string login = "";
            try
            {
                login = (new AccountControllerAdapter(HttpContext.User.Identity.Name)).Activate(token);
            }
            catch(System.Exception exc)
            {
                //Response.Write(exc.Message);
                return null;
            }
            //return RedirectToAction("RegistrationSucceded");
            return Login(0, login, "Регистрация прошла успешно. Введите пароль.");

		}

	/*  code template for controller Adapter
		public ActionResult Activate(string token)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForActivate));
			return M.MakeResponse();
		}
		private string doTrueResponseForActivate()
		{
		}
	*/
		[HttpPost]

		public async Task<IActionResult> Auth(string login, string password)
		{
			string authResult = "";// AccountControllerAdapter.AuthorizeUser(login, password);
			if (authResult == "")
			{
				await Authenticate(login);
				return RedirectToAction("Index", "Home");
			}
			else
			{
				return Login(0, login, authResult);
			}

		}

	/*  code template for controller Adapter
		public ActionResult Auth(string login, string password)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForAuth));
			return M.MakeResponse();
		}
		private string doTrueResponseForAuth()
		{
		}
	*/
		public string Info(string login)
		{

			string result = (new AccountControllerAdapter(HttpContext.User.Identity.Name)).Info(login);
			return result;

		}

	/*  code template for controller Adapter
		public string Info(string login)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForInfo));
			return M.MakeResponse();
		}
		private string doTrueResponseForInfo()
		{
		}
	*/
		public string JsSettings(string settingName = null)
		{

			string result = (new AccountControllerAdapter(HttpContext.User.Identity.Name)).JsSettings(settingName);
			return result;

		}

	/*  code template for controller Adapter
		public string JsSettings()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForJsSettings));
			return M.MakeResponse();
		}
		private string doTrueResponseForJsSettings()
		{
		}
	*/
		public ActionResult Login(int userid=0, string login = "", string result = "")
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				if (Dersa.Models.User.Exists(HttpContext.User.Identity.Name))
					return RedirectToAction("Index", "Home");
			}
			if (result != "")
                ViewBag.AuthResult = result;
            if(login != "")
            {
                LoginInfo Model = new LoginInfo(login);
                return View("Login", Model);
            }
            return View("Login");

		}

	/*  code template for controller Adapter
		public ActionResult Login(int userid=0, string login = "", string result = "")
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForLogin));
			return M.MakeResponse();
		}
		private string doTrueResponseForLogin()
		{
		}
	*/
		//public ActionResult Logout()
		//{
  //          (new AccountControllerAdapter()).Logout();
  //          return RedirectToAction("Index", "Home");

		//}

	/*  code template for controller Adapter
		public ActionResult Logout()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForLogout));
			return M.MakeResponse();
		}
		private string doTrueResponseForLogout()
		{
		}
	*/
		public ActionResult Register()
		{
            return View();

		}

	/*  code template for controller Adapter
		public ActionResult Register()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForRegister));
			return M.MakeResponse();
		}
		private string doTrueResponseForRegister()
		{
		}
	*/
		[HttpPost]
		public ActionResult Register(string login, string password, string name, string email)
		{
            string result = (new AccountControllerAdapter(HttpContext.User.Identity.Name)).Register(login, password, name, email);
            if(result != "")
            {
                ViewBag.ErrorDescr = result;
                return View();
            }
            //return RedirectToAction("Index", "Home");
            return Login(0, login, "Для завершения регистрации следуйте инструкциям в письме.");

		}

	/*  code template for controller Adapter
		public ActionResult Register(string login, string password, string name, string email)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForRegister));
			return M.MakeResponse();
		}
		private string doTrueResponseForRegister()
		{
		}
	*/
		public ActionResult Settings()
		{
			return View();
		}

	/*  code template for controller Adapter
		public ActionResult Settings()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForSettings));
			return M.MakeResponse();
		}
		private string doTrueResponseForSettings()
		{
		}
	*/
		public string SetUserSettings(string json_params)
		{

			string result = (new AccountControllerAdapter(HttpContext.User.Identity.Name)).SetUserSettings(json_params);
			return result;

		}

	/*  code template for controller Adapter
		public string SetUserSettings(string json_params)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForSetUserSettings));
			return M.MakeResponse();
		}
		private string doTrueResponseForSetUserSettings()
		{
		}
	*/
		public string Token(string login)
		{

			string result = (new AccountControllerAdapter(HttpContext.User.Identity.Name)).Token(login);
			return result;

		}

        public string aspNetCookie(string login)
        {
            if (login == null && HttpContext.User.Identity.Name != "")
            {
                //cookieTable[HttpContext.User.Identity.Name] = Request.Cookies[".AspNet.ApplicationCookie"].Value;
                return "OK";
            }
            return (string)cookieTable[login];
        }

		/*  code template for controller Adapter
			public string Token(string login)
			{
				ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForToken));
				return M.MakeResponse();
			}
			private string doTrueResponseForToken()
			{
			}
		*/

		private async Task Authenticate(string userName)
		{
			// создаем один claim
			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
			};
			// создаем объект ClaimsIdentity
			ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
			// установка аутентификационных куки
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login", "Account");
		}

	}
}
