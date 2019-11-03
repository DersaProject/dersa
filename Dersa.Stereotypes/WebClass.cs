using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class WebClass: StereotypeE, ICompiledEntity
	{
		public WebClass(){}

		public WebClass(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

		#region Методы
		#region GenerateClass
		public System.String GenerateClass()
		{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
					System.Text.StringBuilder sbNamespace = new System.Text.StringBuilder();
					
					Package package = this.Parent as Package;
					if (package == null) return "Не найдена папка";
					
					string defUsing = "using System;\nusing System.Runtime.Serialization;\nusing Dersa.Interfaces;\n";
					sbNamespace.Append(defUsing);
					
					sbNamespace.Append("\n");
			        string NS = package.Namespace;
			        if (NS == null) NS = "";
			        while (NS == "")
			        {
			            package = package.Parent as Package;
			            if(package == null) break;
			            NS = package.Namespace;
			            if (NS == null) NS = "";
			        }
					sbNamespace.Append("namespace " + package.Namespace + "\n{\n");
			        sb.Append("\t[Serializable()]\n");
					sb.Append("\tpublic ");
			
			        sb.Append("class " + Name + ": StereotypeBaseE, ICompiledEntity");
					sb.Append("\n\t{\n");
					
					//конструктор
			
					sb.Append("\t\tpublic " + Name + "(){}\n\n");
			        sb.Append("\t\tpublic "); 
			        sb.Append(Name);
			        sb.Append("(IEntity obj)");
			        sb.Append("\n\t\t{");
			        sb.Append("\n\t\t\t_object = obj;");
			        sb.Append("\n\t\t\tif (_object != null)");
			        sb.Append("\n\t\t\t{");
			        sb.Append("\n\t\t\t\t_name = _object.Name;");
			        sb.Append("\n\t\t\t\t_id = _object.Id;");
			        sb.Append("\n\t\t\t}");
			        sb.Append("\n\t\t}\n");
			
					System.Collections.IList attrs = this.GetAttributes();
					
					foreach (Attribute attr in attrs)
					{
						string attr_name = attr.Name;
						string attr_type = attr.Type;
			            string attr_default = attr.Default;
			            sb.Append("\t\tpublic " + attr_type + " " + attr_name);
						bool AttributeIsString = System.Type.GetType(attr_type) == typeof(string);
			            if (attr_default == "" && !AttributeIsString) attr_default = null;
			            if (attr_default != null || AttributeIsString)
						{
							if(attr_default == null)attr_default = "";
			                sb.Append(" = ");
			                if(AttributeIsString)
			                    sb.Append("\"");
			                sb.Append(attr_default);
			                if (AttributeIsString)
			                    sb.Append("\"");
			            }
						sb.Append(";\n");
					}
					
					//Формирование методов
					sb.Append("\n\t\t#region Методы\n");
					
					//sb.Append(this.GenerateMethods());
					System.Collections.IList methods = this.GetMethods();
			
					for (int i = 0; i < methods.Count; i++)
					{
						Method m = methods[i] as Method;
						if(m != null)
						{
							sb.Append("\t\t#region " + m.Name + "\n");
							//if ((m.Attributes != null)&&(m.Attributes != String.Empty))
							//{
							//	sb.Append("\t\t" + m.Attributes + "\n");
							//}
							
							sb.Append(m.GetDeclarationString());
							
							sb.Append("\n\t\t{\n");
							string MethodBody = m.Text;
							MethodBody = MethodBody.Replace("\n", "\n\t\t\t");
							sb.Append(MethodBody);
							sb.Append("\n\t\t}\n");
							sb.Append("\t\t#endregion\n");
						}
					}
					
					sb.Append("\t\t#endregion\n");
					
					sb.Append("\t}\n");
			        sb.Append("}\n");
					
					//string fileName = package.GetDirectory() + "\\" + Name + ".cs";
			
			        string result = sbNamespace.ToString() + sb.ToString();
					//Static.SaveToFile(fileName, sbNamespace.ToString() + sb.ToString() + "\t#region Interface\n" + sbInterface.ToString() + "\t#endregion\n" + "}");
			        string fileName = AppDomain.CurrentDomain.BaseDirectory + "App_Code\\DersaStereotypes\\" + Name + ".cs";
			        string backupFileName = AppDomain.CurrentDomain.BaseDirectory + "Stereotypes.Backup\\" + Name + ".cs";
			
			        //System.IO.File.Copy(fileName, backupFileName);
			        //Static.SaveToFile(fileName, result);
			
			        //Console.WriteLine(result);
			        return result;
		}
		#endregion
		#endregion
	}
}
