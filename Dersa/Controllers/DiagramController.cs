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

namespace Dersa.Controllers
{
	public class DiagramController : Controller
	{

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
