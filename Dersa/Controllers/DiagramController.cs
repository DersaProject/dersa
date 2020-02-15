using System.Reflection;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Dersa.Models;
using Dersa.Common;
using Newtonsoft.Json;

namespace Dersa.Controllers
{
	public class DiagramController : Controller
	{

		public string Create(int parent)
		{

			string result = (new DiagramControllerAdapter()).Create(parent);
			return result;

		}

        public string GetJson(string id)
        {
            string userName = HttpContext.User.Identity.Name;
            DersaSqlManager DM = new DersaSqlManager();
            DataTable T = DM.ExecuteMethod("DIAGRAM", "GetEntities", new object[] { id, userName, DersaUtil.GetPassword(userName) });
            int appIndex = 0;
            var query = from DataRow R in T.Rows
                        select new
                        {
                            displayed_name = R["name"],
                            id = R["id"],
                            app_index = appIndex++,
                            left = R["left"],
                            top = R["top"],
                            width = R["width"],
                            height = R["height"],
                            is_selected = false,
                            is_visible = true
                        };
            return JsonConvert.SerializeObject(query);
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
