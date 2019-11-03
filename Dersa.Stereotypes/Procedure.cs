using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Text.RegularExpressions;
namespace DersaStereotypes
{
	[Serializable()]
	public class Procedure: StereotypeBaseE, ICompiledEntity
	{
		public Procedure(){}

		public Procedure(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String SQL = "";
		public System.String PrefixDelimiter = "$";
		public System.Boolean MakePrefix = true;
		public System.String Source = "SQL";
		public System.String Params = "()";
		public System.String Return = "";
		public System.String Description = "";
		public System.String Visibility = "public";
		public System.String CallingServer = "";

		#region Ìועמהû
		#region Generate
		public System.String Generate(Dersa.Interfaces.ICompiledEntity owner)
		{
if (owner == null)
			{
				owner = this.Parent;
			}
			
			string sqlName = this.GetSqlName(owner);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("if exists (select * from sysobjects where id = object_id('dbo." + sqlName + "'))\n");
			sb.Append("\tdrop procedure dbo." + sqlName + "\n");
			sb.Append("go\n\n");
			
			if(Source != "SQL" || !this.SQL.ToLower().Contains("create procedure"))
			    sb.Append("create procedure dbo." + sqlName + "\n");
			if (Source == "SQL")
			{
				sb.Append(this.SQL);
			}
			else if (Source == "CSharp")
			{
				object result = Static.CompileAndExecuteAditionalMethod(owner, "System.String", this.Name, this.SQL);
				if (result != null)
				{
					sb.Append((string)result);
				}
			}
			sb.Append("\ngo\n\n");
			sb.Append("grant execute on " + sqlName + " to public\n");
			sb.Append("go\n\n");
			
			string sOut = sb.ToString();
			return sOut;
		}
		#endregion
		#region GetSqlName
		public System.String GetSqlName(Dersa.Interfaces.ICompiledEntity owner)
		{
if (MakePrefix)
			{
				if (owner == null)
				{
					owner = this.Parent;
					if (owner == null)
					{
						return this.Name;
					}
				}
				System.Reflection.MethodInfo mi = owner.GetType().GetMethod("GetSqlName");
				string objectName = "";
				if (mi != null)
				{
					objectName = (string)mi.Invoke(owner, new object[]{});
				}
				return objectName + this.PrefixDelimiter + this.Name;
			}
			else
			{
				return this.Name;
			}
		}
		#endregion
		#region sqlGenerate
		public string sqlGenerate()
		{
return Generate(null);
		}
		#endregion
		#region GetTextForDeclaration
		public string GetTextForDeclaration()
		{
string procType = "PROCEDURE";
			if(!this.SQL.Replace(" ","").Contains("PROCEDURE"+this.Name))
			    procType = "FUNCTION";
			Regex regEx = new Regex("(" + procType + ".*?)[\\s)][AI]S\\s.*",  RegexOptions.Singleline);
			string result = regEx.Replace(this.SQL, "$1;");
			return result;
			
		}
		#endregion
		#endregion
	}
}
