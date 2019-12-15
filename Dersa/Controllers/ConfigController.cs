using System.Reflection;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DIOS.Common.Interfaces;
using Dersa.Models;
using DIOS.Common;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using DIOS.BusinessBase;
using Dersa.Common;
using Dersa.Interfaces;
using System.Web.Configuration;
using System.Configuration;



namespace CuksWebApiApplication.Controllers
{
	public class ConfigController : Controller
	{

		[MethodPermissionsAttribute("admin")]
		public ActionResult CachedObjects()
		{
			MethodInfo method = this.GetType().GetMethod("CachedObjects", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            return View(CommonEnvironment.GetCachedObjects());
		}

	/*  code template for controller Adapter
		public ActionResult CachedObjects()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForCachedObjects));
			return M.MakeResponse();
		}
		private string doTrueResponseForCachedObjects()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult ClearCache(string key)
		{
			MethodInfo method = this.GetType().GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            try
            {
                CommonEnvironment.ClearCache(key);
            }
            catch
            {
                //return exc.Message;
            }
			return RedirectToAction("CachedObjects");

		}

	/*  code template for controller Adapter
		public ActionResult ClearCache(string key)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForClearCache));
			return M.MakeResponse();
		}
		private string doTrueResponseForClearCache()
		{
		}
	*/
		public void CloneDataToFile()
		{
            try
            {
                AnonimousAppSqlManager M = new AnonimousAppSqlManager();
                DataTable T = M.ExecuteSPWithParams("OBJECT_INFO$GetSql", new object[] { });
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach(DataRow R in T.Rows)
                {
                    sb.Append(R[0].ToString());
                    sb.Append("\n\n");
                }
                Response.ContentType = "APPLICATION/OCTET-STREAM";
                string Header = "Attachment; Filename=initdb.sql";
                Response.AppendHeader("Content-Disposition", Header);

                byte[] bts = System.Text.Encoding.Default.GetBytes(sb.ToString());
                Response.OutputStream.Write(bts, 0, bts.Length);
                Response.End();
            }
            catch(Exception exc)
            {
                Response.OutputStream.Flush();
                Response.OutputStream.Close();
                Response.ContentType = "TEXT/HTML";
                Response.ClearHeaders();
                Response.Write(exc.Message);
            }
		}

	/*  code template for controller Adapter
		public void CloneDataToFile()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForCloneDataToFile));
			return M.MakeResponse();
		}
		private string doTrueResponseForCloneDataToFile()
		{
		}
	*/
		public void CloneToFile()
		{
            try
            {
                AnonimousAppSqlManager M = new AnonimousAppSqlManager();
                DataTable T = M.ExecuteSPWithParams("OBJECT_INFO$GetSql", new object[] { });
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach(DataRow R in T.Rows)
                {
                    sb.Append(R[0].ToString());
                    sb.Append("\n\n");
                }
                Response.ContentType = "APPLICATION/OCTET-STREAM";
                string Header = "Attachment; Filename=initdb.sql";
                Response.AppendHeader("Content-Disposition", Header);

                byte[] bts = System.Text.Encoding.Default.GetBytes(sb.ToString());
                Response.OutputStream.Write(bts, 0, bts.Length);
                Response.End();
            }
            catch(Exception exc)
            {
                Response.OutputStream.Flush();
                Response.OutputStream.Close();
                Response.ContentType = "TEXT/HTML";
                Response.ClearHeaders();
                Response.Write(exc.Message);
            }
		}

	/*  code template for controller Adapter
		public void CloneToFile()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForCloneToFile));
			return M.MakeResponse();
		}
		private string doTrueResponseForCloneToFile()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult Create(int id)
		{
			MethodInfo method = this.GetType().GetMethod("Create", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            SysParam P = SysParam.GetFactory().GetObject(id) as SysParam;
            return View(P);

		}

	/*  code template for controller Adapter
		public ActionResult Create(int id)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForCreate));
			return M.MakeResponse();
		}
		private string doTrueResponseForCreate()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult Delete(int id)
		{
			MethodInfo method = this.GetType().GetMethod("Delete", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            SysParam P = SysParam.GetFactory().GetObject(id) as SysParam;
            return View(P);
		}

	/*  code template for controller Adapter
		public ActionResult Delete(int id)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForDelete));
			return M.MakeResponse();
		}
		private string doTrueResponseForDelete()
		{
		}
	*/
		[HttpPost]
		[MethodPermissionsAttribute("admin")]
		public string DeleteImmediately(FormCollection collection)
		{
			MethodInfo method = this.GetType().GetMethod("DeleteImmediately", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return (new FrontResponse(-1, canExec, "", -1)).ToJson();
			int id = -1;
			try
			{
				id = int.Parse(collection["sys_param"]);
			}
			catch{}
			if(id < 0)
				return "invalid parameter value";
            return "OK";// (new ObjectController()).Drop("SYS_PARAM", id);
		}

	/*  code template for controller Adapter
		public string DeleteImmediately(FormCollection collection)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForDeleteImmediately));
			return M.MakeResponse();
		}
		private string doTrueResponseForDeleteImmediately()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult Details(int id)
		{
			MethodInfo method = this.GetType().GetMethod("Details", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            int rowcount = 0;
            IParameterCollection Params = new ParameterCollection();
            Params.Add("sys_param", id);
            IObjectCollection oc = ObjectMethods.List("SYS_PARAM_ATTR", Params, "", -1, 0, out rowcount);
            List<SysParamAttr> modelList = new List<SysParamAttr>();
            foreach (SysParamAttr p in oc)
            {
                modelList.Add(p);
            }
            return View(modelList);

		}

	/*  code template for controller Adapter
		public ActionResult Details(int id)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForDetails));
			return M.MakeResponse();
		}
		private string doTrueResponseForDetails()
		{
		}
	*/
		public ActionResult Display(string class_name)
		{
            DiosSqlManager M = new DiosSqlManager();
            System.Data.DataTable T = M.ExecuteSPWithParams("OBJ$List", new object[]{class_name, "", "", 500});
            try
            {
                ActionResult CV = View(class_name, T);
                CV.ExecuteResult(this.ControllerContext);
                return null;
            }
            catch(Exception exc)
            {
				ViewBag.ClassName = class_name;
				ViewBag.KeyName = class_name.ToLower();
                return View("Table", T);
            }

		}

	/*  code template for controller Adapter
		public ActionResult Display(string class_name)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForDisplay));
			return M.MakeResponse();
		}
		private string doTrueResponseForDisplay()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult Edit(int id)
		{
			MethodInfo method = this.GetType().GetMethod("Edit", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            SysParam P = SysParam.GetFactory().GetObject(id) as SysParam;
            return View(P);

		}

	/*  code template for controller Adapter
		public ActionResult Edit(int id)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForEdit));
			return M.MakeResponse();
		}
		private string doTrueResponseForEdit()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult EditDetail(int id)
		{
			MethodInfo method = this.GetType().GetMethod("EditDetail", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            SysParamAttr PA = SysParamAttr.GetFactory().GetObject(id) as SysParamAttr;
            return View(PA);
		}

	/*  code template for controller Adapter
		public ActionResult EditDetail(int id)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForEditDetail));
			return M.MakeResponse();
		}
		private string doTrueResponseForEditDetail()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult EditObject(string class_name, int id)
		{
			MethodInfo method = this.GetType().GetMethod("EditObject", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            DiosSqlManager M = new DiosSqlManager();
            DIOS.ObjectLib.Object Obj = M.GetFactory(class_name).GetObject(id) as DIOS.ObjectLib.Object;
            return View(Obj);

		}

	/*  code template for controller Adapter
		public ActionResult EditObject(string class_name, int id)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForEditObject));
			return M.MakeResponse();
		}
		private string doTrueResponseForEditObject()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult EditRegEx(int id)
		{
			MethodInfo method = this.GetType().GetMethod("EditRegEx", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            RegexChecker RC = RegexChecker.GetFactory().GetObject(id) as RegexChecker;
            return View(RC);
		}

	/*  code template for controller Adapter
		public ActionResult EditRegEx(int id)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForEditRegEx));
			return M.MakeResponse();
		}
		private string doTrueResponseForEditRegEx()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult Index()
		{
            //MethodInfo method = this.GetType().GetMethod("Index", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
            //string canExec = ObjectMethods.CanExecuteMethod(method);
            //if (canExec != "")
            //	return null;
            //int rowcount = 0;
            //IObjectCollection oc = ObjectMethods.List("SYS_PARAM", null, "", -1, 0, out rowcount);
            //            List<SysParam> modelList = new List<SysParam>();
            //            foreach (SysParam p in oc)
            //            {
            //                modelList.Add(p);
            //            }
            Dictionary<string, string> keys = new Dictionary<string, string>();
            foreach(ConnectionStringSettings css in WebConfigurationManager.ConnectionStrings)
            {
                keys.Add(css.Name, css.ConnectionString);
            }
            return View(keys);
            //return View(oc);
		}

	/*  code template for controller Adapter
		public ActionResult Index()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForIndex));
			return M.MakeResponse();
		}
		private string doTrueResponseForIndex()
		{
		}
	*/

        public string SetConnectionString(string csName, string csValue)
        {
            try
            {
                Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
                configuration.ConnectionStrings.ConnectionStrings[csName].ConnectionString = csValue;
                configuration.Save();
                //WebConfigurationManager.ConnectionStrings[csName].ConnectionString = csValue;
                return "OK";
            }
            catch(Exception exc)
            {
                return exc.Message;
            }
        }

		[MethodPermissionsAttribute("admin")]
		public ActionResult RegEx()
		{
			MethodInfo method = this.GetType().GetMethod("RegEx", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            int rowcount = 0;
            IObjectCollection oc = ObjectMethods.List("REGEX_CHECKER", null, "", -1, 0, out rowcount);
            List<RegexChecker> modelList = new List<RegexChecker>();
            foreach (RegexChecker p in oc)
            {
                modelList.Add(p);
            }
            return View(modelList);
		}

	/*  code template for controller Adapter
		public ActionResult RegEx()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForRegEx));
			return M.MakeResponse();
		}
		private string doTrueResponseForRegEx()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public string Restart()
		{
			MethodInfo method = this.GetType().GetMethod("Restart", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return (new FrontResponse(-1, canExec, "", -1)).ToJson();
			System.Web.HttpRuntime.UnloadAppDomain();
			return "system restarted";
		}

	/*  code template for controller Adapter
		public string Restart()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForRestart));
			return M.MakeResponse();
		}
		private string doTrueResponseForRestart()
		{
		}
	*/
		[HttpPost]
		[MethodPermissionsAttribute("admin")]
		public ActionResult SaveCreate(FormCollection collection)
		{
			MethodInfo method = this.GetType().GetMethod("SaveCreate", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
			int id = -1;
			try
			{
				id = int.Parse(collection["sys_param"]);
			}
			catch{}
			if(id < 0)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            //string result = (new ObjectController()).Create("SYS_PARAM", JsonConvert.SerializeObject(new { name = collection["name"], value = collection["value"] }));
			return RedirectToAction("Index");
		}

	/*  code template for controller Adapter
		public ActionResult SaveCreate(FormCollection collection)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForSaveCreate));
			return M.MakeResponse();
		}
		private string doTrueResponseForSaveCreate()
		{
		}
	*/
		[HttpPost]
		[MethodPermissionsAttribute("admin")]
		public ActionResult SaveDetailEdit(FormCollection collection)
		{
			MethodInfo method = this.GetType().GetMethod("SaveDetailEdit", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
			int id = -1;
			try
			{
				id = int.Parse(collection["sys_param_attr"]);
			}
			catch{}
			if(id < 0)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            //string result = (new ObjectController()).Update("SYS_PARAM_ATTR", JsonConvert.SerializeObject(new { id = id, value = collection["value"] }));
			return RedirectToAction("Index");
		}

	/*  code template for controller Adapter
		public ActionResult SaveDetailEdit(FormCollection collection)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForSaveDetailEdit));
			return M.MakeResponse();
		}
		private string doTrueResponseForSaveDetailEdit()
		{
		}
	*/
		[HttpPost]
		[MethodPermissionsAttribute("admin")]
		public ActionResult SaveEdit(FormCollection collection)
		{
			MethodInfo method = this.GetType().GetMethod("SaveEdit", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
			int id = -1;
			try
			{
				id = int.Parse(collection["sys_param"]);
			}
			catch{}
			if(id < 0)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            //string result = (new ObjectController()).Update("SYS_PARAM", JsonConvert.SerializeObject(new { id = id, value = collection["value"] }));
			return RedirectToAction("Index");
		}

	/*  code template for controller Adapter
		public ActionResult SaveEdit(FormCollection collection)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForSaveEdit));
			return M.MakeResponse();
		}
		private string doTrueResponseForSaveEdit()
		{
		}
	*/
		[HttpPost]
		[MethodPermissionsAttribute("admin")]
		public ActionResult SaveEditObject(FormCollection collection)
		{
			MethodInfo method = this.GetType().GetMethod("SaveEditObject", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
			int id = -1;
			try
			{
				id = int.Parse(collection["satellite"]);
			}
			catch{}
			if(id < 0)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            //string result = (new ObjectController()).Update("SATELLITE", JsonConvert.SerializeObject(new { id = id, name = collection["name"] }));
			return RedirectToAction("Display");
		}

	/*  code template for controller Adapter
		public ActionResult SaveEditObject(FormCollection collection)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForSaveEditObject));
			return M.MakeResponse();
		}
		private string doTrueResponseForSaveEditObject()
		{
		}
	*/
		[HttpPost]
		[MethodPermissionsAttribute("admin")]
		public ActionResult SaveRegExEdit(FormCollection collection)
		{
			MethodInfo method = this.GetType().GetMethod("SaveRegExEdit", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
			int id = -1;
			try
			{
				id = int.Parse(collection["regex_checker"]);
			}
			catch{}
			if(id < 0)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            //string result = (new ObjectController()).Update("REGEX_CHECKER", JsonConvert.SerializeObject(new { id = id, regexp = collection["regexp"] }));
			return RedirectToAction("RegEx");
		}

	/*  code template for controller Adapter
		public ActionResult SaveRegExEdit(FormCollection collection)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForSaveRegExEdit));
			return M.MakeResponse();
		}
		private string doTrueResponseForSaveRegExEdit()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult Timer(int id, string timerAction)
		{
			MethodInfo method = this.GetType().GetMethod("Timer", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
			if(timerAction == "start")
			{
				TaskTimer.Start(id);
//				ObjectMethods.StartTimer();
//				return "Timer started";				
			}
			if(timerAction == "stop")
			{
				TaskTimer.Stop(id);
//				ObjectMethods.StopTimer();
//				return "Timer stopped";				
			}
//			return "action was not recognized";
			return RedirectToAction("TimerState");
		}

	/*  code template for controller Adapter
		public ActionResult Timer(int id, string timerAction)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForTimer));
			return M.MakeResponse();
		}
		private string doTrueResponseForTimer()
		{
		}
	*/
		[MethodPermissionsAttribute("admin")]
		public ActionResult TimerState()
		{
			MethodInfo method = this.GetType().GetMethod("TimerState", BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
			string canExec = ObjectMethods.CanExecuteMethod(method);
			if (canExec != "")
				return null;
            IObjectCollection S = ApplicationMethods.GetCachedObjects("TASK_TIMER", null, -1);
            return View(S);
		}

	/*  code template for controller Adapter
		public ActionResult TimerState()
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForTimerState));
			return M.MakeResponse();
		}
		private string doTrueResponseForTimerState()
		{
		}
	*/
	}
}
