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
		public System.String Visibility = "public";
		public System.String Description = "";
		public System.String Return = "";
		public System.String Source = "SQL";
		public System.String PrefixDelimiter = "$";
		public System.String CallingServer = "";
		public System.Boolean MakePrefix = true;
		public System.String SQL = "";
		public System.String Params = "()";

		#region Ìועמהû
		#region IsCompact
		public bool IsCompact()
		{
	Regex regEx = new Regex("(PROCEDURE|FUNCTION)[\\s\\S]*?[\\s)][AI]S",  RegexOptions.Singleline);  //(PROCEDURE|FUNCTION) .*? [AI]S
				MatchCollection Matches = regEx.Matches(this.SQL);
				return Matches.Count < 1;
		}
		#endregion
		#region sqlGenerate
		public object sqlGenerate()
		{
return Generate(null);
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
		#region Generate
		public object Generate(Dersa.Interfaces.ICompiledEntity owner)
		{
if (owner == null)
			{
				owner = this.Parent;
			}
			
			string sqlName = this.GetSqlName(owner);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if(Source != "SQL" || !this.SQL.ToLower().Contains("create procedure"))
			    sb.Append("create or replace function " + sqlName + "\n");
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
			sb.Append("\n/");
			
			string sOut = sb.ToString();
			return new
			{
				sql = sOut,
				object_name = sqlName,
				object_type = "FUNCTION"
			};
		}
		#endregion
		#region GetText
		public string GetText(bool ForDeclaration)
		{
string sqlText = this.SQL;
			if(this.IsCompact())
				sqlText = "FUNCTION " + this.Name + sqlText;
			if(ForDeclaration)
			{
				string procType = "PROCEDURE";
				if(!sqlText.Replace(" ","").Contains("PROCEDURE"+this.Name))
			    		procType = "FUNCTION";
				Regex regEx = new Regex("(" + procType + ".*?)[\\s)][AI]S\\s.*",  RegexOptions.Singleline);
				string result = regEx.Replace(sqlText, "$1;");
				return result;
			}
			else
			{
				sqlText = this.Description.Trim() + "\r\n" + sqlText;
			}
			return sqlText;
			
		}
		#endregion
		#endregion
	}
}
