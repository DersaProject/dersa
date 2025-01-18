using System;
using System.Reflection;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Dersa.Models;
using Dersa.Common;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Newtonsoft.Json;
using DersaStereotypes;
using System.Net.PeerToPeer;

namespace Dersa.Controllers
{
	public class DiagramController : Controller
	{

		public string RelationInfo(int diagram, int relation)
		{
            string userName = HttpContext.User.Identity.Name;
            var DM = new DersaSqlManager();
            var T = DM.ExecuteMethod("DIAGRAM", "RelationInfo", new object[] { diagram, relation, userName, DersaUtil.GetPassword(userName) });
            return JsonConvert.SerializeObject(T);
        }

        public string Create(int parent)
		{

			string result = DersaUtil.CreateDiagram(parent, HttpContext.User.Identity.Name);
			return result;

		}

		public string GetJson(string id)
		{
			id = id.Replace("D_", "");
			return DersaUtil.GetDiagramJson(id, HttpContext.User.Identity.Name);
		}

		public string GetEntities(string id)
		{
			id = id.Replace("D_", "");
			return DersaUtil.GetDiagramEntities(id, HttpContext.User.Identity.Name);
		}

		public string GetRelations(string id)
		{
			id = id.Replace("D_", "");
			return DersaUtil.GetDiagramRelations(id, HttpContext.User.Identity.Name);
		}

		public string SaveDiagram(string id, string jsonObject)
        {
			return DersaUtil.SaveDiagramFromJson(id, jsonObject);
        }


        public string Save(string id, string xml)
		{
            return DersaUtil.SaveDiagramXml(id, xml);
		}

	}
}
