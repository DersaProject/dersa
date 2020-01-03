using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Ref: StereotypeBaseE, ICompiledEntity
	{
		public Ref(){}

		public Ref(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Prefix = "source";
		public System.String Interface = "IObject";
		public System.String PropName = "";
		public System.String RefObjectIdType = "int";
		public System.Boolean ForDIOS = false;
		public System.String set_class = "";
		public System.String set_ref = "";

		#region Ìועמהû
		#region GenerateName
		public System.String GenerateName()
		{
string name = this.PropName;
			if (name.Length > 0)
			{
				return name;
			}
			return this.Name;
		}
		#endregion
		#region GenerateInterface
		public System.String GenerateInterface()
		{
string pName = GenerateName(); 
			string interf = this.Interface;
			return "\t\t" + interf + " "  + pName + "Ref{get;set;}" + "\n";
		}
		#endregion
		#region Generate
		public System.String Generate()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			string pName = GenerateName(); 
			string lowerName = pName.ToLower();
			string interf = this.Interface;
			string prefix = this.Prefix;
			
			sb.Append("\t\tpublic " + interf + " "  + pName + "Ref" + "\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t\tget\n");
			sb.Append("\t\t\t{\n");
			sb.Append("\t\t\t\tif ((this." + prefix+ "_ref.IsNull)||(this." + prefix + "_class.IsNull)) return null;\n");
			sb.Append("\t\t\t\treturn GetObject(this." + prefix + "_class, this." + prefix + "_ref) as " + interf + ";\n");
			sb.Append("\t\t\t}\n");
			sb.Append("\t\t\tset\n");
			sb.Append("\t\t\t{\n");
			sb.Append("\t\t\t\tif (value!= null)\n");
			sb.Append("\t\t\t\t{\n");
			if(this.ForDIOS)
				sb.Append("\t\t\t\t\tthis." + prefix + "_class = value.Entity.ClassName;\n");
			else
				sb.Append("\t\t\t\t\tthis." + prefix + "_class = value.Factory.FactoryClassName;\n");
			sb.Append("\t\t\t\t\tthis." + prefix + "_ref = (" + Static.GetCSharpNativeType(this.RefObjectIdType) + ")value.id;\n");
			sb.Append("\t\t\t\t}\n");
			sb.Append("\t\t\t\telse\n"); 
			sb.Append("\t\t\t\t{\n");
			sb.Append("\t\t\t\t\tthis." + prefix+ "_class = new SqlString();\n");
			sb.Append("\t\t\t\t\tthis." + prefix + "_ref = new " + Static.GetCSharpType(this.RefObjectIdType) + "();\n");
			sb.Append("\t\t\t\t}\n");
			sb.Append("\t\t\t}\n");
			sb.Append("\t\t}\n");
			return sb.ToString();
		}
		#endregion
		#endregion
	}
}
