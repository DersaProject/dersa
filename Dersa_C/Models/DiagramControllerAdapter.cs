using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Dersa.Common;

namespace Dersa.Models
{
    public class DiagramControllerAdapter
    {
        private string _contextUserName;
        public DiagramControllerAdapter(string contextUserName) : base()
        {
            _contextUserName = contextUserName;
        }
        public string Create(int parent)
        {
            IParameterCollection Params = new ParameterCollection();
            Params.Add("@parent", parent);
            string currentUser = _contextUserName;
            Params.Add("@login", currentUser);
            Params.Add("@password", DersaUtil.GetPassword(currentUser));
            DersaSqlManager M = new DersaSqlManager();
            int res = M.ExecuteIntMethod("DIAGRAM", "CreateDiagram", Params);
            return res.ToString();
        }

        public string Save(string id, string xml)
        {
            XmlDocument doc = new XmlDocument();
            string decodedXml = xml.Replace("{lt;", "<").Replace("{gt;", ">");
            IParameterCollection Params = new ParameterCollection();
            Params.Add("@diagram", id.Replace("D_", ""));
            Params.Add("@xml", decodedXml);
            string currentUser = _contextUserName;
            Params.Add("@login", currentUser);
            Params.Add("@password", DersaUtil.GetPassword(currentUser));
            DersaSqlManager M = new DersaSqlManager();
            int res = M.ExecuteIntMethod("DIAGRAM", "SaveFromXml", Params);
            return res.ToString();
        }
    }
}