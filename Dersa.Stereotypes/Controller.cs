using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Controller: StereotypeBaseE, ICompiledEntity
	{
		public Controller(){}

		public Controller(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string Using = "";

		#region Методы
		#region Generate
		public string Generate()
		{
            Dersa.Common.CachedObjects.ClearCache();
			            Package package = this.Parent as Package;
			            if (package == null) return "Не найдена папка";
			
			            Map methods = new Map();
			            System.Collections.IList children = this.Children;
			            for (int i = 0; i < children.Count; i++)
			            {
			                Method obj = children[i] as Method;
			                if (obj == null) continue;
			                methods.Add(obj.GetMapKey(), obj);
			            }
			
			            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			            sb.Append("using System.Data;\r\n");
			            sb.Append("using System.Linq;\r\n");
			            sb.Append("using System.Web.Mvc;\r\n");
			            sb.Append("using " + package.Namespace + ".Models;\r\n");
			            sb.Append(this.Using);
			            sb.Append("\r\n\r\n");
			
			            sb.Append("namespace " + package.Namespace + ".Controllers\r\n");
			            sb.Append("{\r\n");
			
			            sb.Append("\tpublic class " + this.Name + "Controller : Controller\r\n");
			            sb.Append("\t{\r\n");
			
			            foreach (Method m in methods)
			            {
			                if (!string.IsNullOrEmpty(m.Attributes))
			                {
			                    sb.Append("\r\n\t\t");
			                    sb.Append(m.Attributes);
			                }
			                sb.Append("\r\n\t\t");
			                sb.Append(m.AccessModifier);
			                sb.Append(" ");
			                sb.Append(m.ReturnType);
			                sb.Append(" ");
			                sb.Append(m.Name);
			                sb.Append("(");
			                sb.Append(m.Parameters);
			                sb.Append(")");
			                sb.Append("\r\n\t\t");
			                sb.Append("{");
			                sb.Append("\r\n");
			                if (m.Text != null && m.Text != "")
			                    sb.Append(m.Text);
			                else
			                {
			                    sb.Append("\t\t\t");
			                    sb.Append("return (new " + this.Name + "ControllerAdapter()).");
			                    sb.Append(m.Name);
			                    sb.Append("(");
			                    string[] ParamsWithTypes = m.Parameters.Split(',');
			                    System.Text.StringBuilder ParamsWoTypes = new System.Text.StringBuilder();
			                    for (int p = 0; p < ParamsWithTypes.Length; p++)
			                    {
			                        string ParWithType = ParamsWithTypes[p].Trim();
			                        if (ParWithType != "")
			                        {
			                            if (p > 0)
			                                ParamsWoTypes.Append(", ");
			                            ParamsWoTypes.Append(ParamsWithTypes[p].Trim().Split(' ')[1]);
			                        }
			                    }
			                    sb.Append(ParamsWoTypes.ToString());
			                    sb.Append(");");
			                }
			                sb.Append("\r\n\t\t");
			                sb.Append("}");
			                sb.Append("\r\n");
			            }
			
			            sb.Append("\t}\r\n");
			            sb.Append("}\r\n");
			
			            string result = sb.ToString();
			            return result;
			
		}
		#endregion
		#endregion
	}
}
