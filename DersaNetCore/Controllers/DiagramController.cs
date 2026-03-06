using System;
using System.Reflection;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dersa.Models;
using Dersa.Common;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Newtonsoft.Json;
//using DersaStereotypes;

namespace Dersa.Controllers
{
	public class DiagramController : Controller
	{

		public string Create(int parent)
		{

			string result = (new DiagramControllerAdapter(HttpContext.User.Identity.Name)).Create(parent);
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
        [HttpPost]
        public string SaveDiagram(string id, string jsonObject)
        {
            //displayed_name":"OBJ","id":"n3","app_index":2,"left":407,"top":254,"width":100,"height":25,"is_selected":false,"is_visible":true 
            try
            {
                object[] diagArray = JsonConvert.DeserializeObject<object[]>(jsonObject);
                var query = from dynamic N in diagArray
                            select new { N.is_visible, N.id, N.left, N.top, N.width, N.height };

                DersaSqlManager DM = new DersaSqlManager();
                IParameterCollection Params = new ParameterCollection();
                foreach (dynamic X in query)
                {
                    int diagram_entity = -1;
                    int entity = -1;
                    bool saveCoords = true;
                    Params.Clear();
                    string S = "";
                    if(!(bool)X.is_visible)
                    {
                        saveCoords = false;
                        if (((string)X.id).Substring(0, 2) == "DE")
                        {
                            diagram_entity = int.Parse(((string)X.id).Split('_')[1]);
                            Params.Add("diagram_entity", diagram_entity);
                            DM.ExecuteIntMethod("DIAGRAM", "DropEntity", Params);
                        }
                    }
                    else if(((string)X.id).Substring(0, 1) == "n")
                    {
                        entity = int.Parse(((string)X.id).Split('_')[1]);
                        Params.Add("diagram", id);
                        Params.Add("entity", entity);
                        Params.Add("left", X.left);
                        Params.Add("top", X.top);
                        Params.Add("w", X.width);
                        Params.Add("h", X.height);
                        DM.ExecuteIntMethod("DIAGRAM", "CreateEntity", Params);
                    }
                    else
                    {
                        diagram_entity = int.Parse(((string)X.id).Split('_')[1]);
                        Params.Add("diagram_entity", diagram_entity);
                        Params.Add("left", X.left);
                        Params.Add("top", X.top);
                        Params.Add("w", X.width);
                        Params.Add("h", X.height);
                        entity = DM.ExecuteIntMethod("DIAGRAM", "UpdateEntity", Params);
                    }
                    if (saveCoords)
                    {
                        DersaStereotypes.StereotypeBaseE objToSaveCoords = DersaStereotypes.StereotypeBaseE.GetSimpleInstance(entity);
                        objToSaveCoords.SaveCoords("", (int)X.left, (int)X.top, (int)X.width, (int)X.height);
                    }
                }
                string result = JsonConvert.SerializeObject(query);
                return result;
            }
            catch(Exception exc)
            {
                return exc.Message;
            }
        }


        public string Save(string id, string xml)
		{

			string result = (new DiagramControllerAdapter(HttpContext.User.Identity.Name)).Save(id, xml);
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
