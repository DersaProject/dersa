using System.Reflection;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DIOS.Common.Interfaces;
using Dersa.Models;


namespace Dersa.Controllers
{
	public class DiagramController : Controller
	{

		public string Create(int parent)
		{

			string result = (new DiagramControllerAdapter()).Create(parent);
			return result;

		}

	/*  code template for controller Adapter
		public string Create(int parent)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForCreate));
			return M.MakeResponse();
		}
		private string doTrueResponseForCreate()
		{
		}
	*/
		public string Save(string id, string xml)
		{

			string result = (new DiagramControllerAdapter()).Save(id, xml);
			return result;

		}

	/*  code template for controller Adapter
		public string Save(string id, string xml)
		{
			ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForSave));
			return M.MakeResponse();
		}
		private string doTrueResponseForSave()
		{
		}
	*/	}
}
