using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Dersa.Common;
using System.IO;
using System.Reflection;
using Dersa.Interfaces;
using DersaStereotypes;

namespace Dersa.Models
{
    public class ProjectControllerAdapter
    {
        private string _contextUserName;
        public ProjectControllerAdapter(string contextUserName) : base()
        {
            _contextUserName = contextUserName;
        }
        public string CreateDir(string name)
        {
            return "OK";
        }

        public void SetImagePath(int id, string fileName)
        {
            string userName = _contextUserName;
            string path = "/user_resources/" + userName + "/" + fileName;
            StereotypeBaseE target = StereotypeBaseE.GetSimpleInstance(id);
            CachedObjects.CachedEntities[id] = null;
            DersaSqlManager M = new DersaSqlManager();
            System.Data.DataTable t = M.GetEntity(id.ToString());
            if (t == null)
                throw new Exception(string.Format("Table is null for entity {0}", id));
            if (t.Rows.Count < 1)
                throw new Exception(string.Format("Table is empty for entity {0}", id));
            DersaEntity ent = new DersaEntity(t, M);
            CachedObjects.CachedCompiledInstances[ent.StereotypeName + id.ToString()] = null;
            foreach (DersaEntity child in ent.Children)
            {
                CachedObjects.CachedCompiledInstances[child.StereotypeName + child.Id.ToString()] = null;
            }

            ICompiled cInst = ent.GetCompiledInstance();
            MethodInfo mi = cInst.GetType().GetMethod("SetPath");
            if (mi == null)
            {
                string excMessage = "Method SetPath not found ";
                Logger.LogStatic(excMessage);
                throw new Exception(excMessage);
            }
            object[] callParams = new object[2];
            callParams[0] = userName;
            callParams[1] = path;
            mi.Invoke(cInst, callParams);
        }

        public System.Tuple<string, string> GetPath(string clientPath)
        {
            string userName = _contextUserName;
            string[] fileNameParts = clientPath.Split('\\');
            string fileName = fileNameParts[fileNameParts.Length - 1];
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "user_resources", userName, fileName);
            return new Tuple<string, string>(path, fileName);
        }

        public string CreateTextFile(string json_params)
        {
            IParameterCollection Params = Util.DeserializeParams(json_params);
            if (Params.Contains("file_name") && Params.Contains("file_body"))
            {
                string userName = _contextUserName;
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_resources", userName, Params["file_name"].Value.ToString());
                StreamWriter SW = File.CreateText(path);
                SW.Write((string)Params["file_body"].Value);
                SW.Flush();
                SW.Close();
                return "OK";
            }
            return "no parameters";
        }
    }
}