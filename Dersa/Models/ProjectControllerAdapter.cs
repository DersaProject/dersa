using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Dersa.Common;
using System.IO;

namespace Dersa.Models
{
    public class ProjectControllerAdapter
    {
        public string CreateDir(string name)
        {
            return "OK";
        }


        public string CreateTextFile(string json_params)
        {
            IParameterCollection Params = Util.DeserializeParams(json_params);
            if (Params.Contains("file_name") && Params.Contains("file_body"))
            {
                string userName = HttpContext.Current.User.Identity.Name;
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