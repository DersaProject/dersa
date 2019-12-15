using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class PlSqlPackage: StereotypeBaseE, ICompiledEntity
	{
		public PlSqlPackage(){}

		public PlSqlPackage(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string PackageDeclarations = "";
		public string Description = "";
		public string BodyDeclarations = "";

		#region Ìועמהû
		#region ora_Generate
		public string ora_Generate()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			sb.Append("create or replace PACKAGE " + this.Name + " AS\n\n");
			sb.Append(this.PackageDeclarations);
			sb.Append("\n");
			
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Procedure)
				{
					Procedure proc = (Procedure)obj;
			                sb.Append(proc.GetTextForDeclaration());
			                sb.Append("\n");
				}
			}
			sb.Append("END " + this.Name + ";\n\n");
			
			sb.Append("create or replace PACKAGE BODY " + this.Name + " AS\n\n");
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Procedure)
				{
					Procedure proc = (Procedure)obj;
			                sb.Append(proc.SQL);
			                sb.Append("\n");
				}
			}
			sb.Append("END " + this.Name + ";");
			
			
			return sb.ToString();
		}
		#endregion
		#endregion
	}
}
