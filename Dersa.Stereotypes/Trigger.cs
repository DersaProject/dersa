using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Trigger: StereotypeBaseE, ICompiledEntity
	{
		public Trigger(){}

		public Trigger(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Type = "";
		public System.Boolean Active = true;
		public System.String TableExt = "";
		public System.Boolean InsteadOf = false;
		public System.String SQL = "";
		public System.String Source = "SQL";
		public System.String Sequence = "";

		#region Ìועמהû
		#region Generate
		public System.String Generate(System.Boolean dialog, Dersa.Interfaces.ICompiledEntity owner)
		{
if (Active == false) return "";
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			//sb.Append(Drop(false, owner));
			
			if ((SQL == null)||(SQL.Length == 0)) return "";
			string sqlName = GetSqlName(owner);
			
			sb.Append("CREATE OR REPLACE TRIGGER ");
			sb.Append(sqlName + "\r\n");
			sb.Append(this.Type + "\r\n");
			if (owner == null)
			{
				owner = this.Parent;
			}
			string ownerSqlName = GetOwnerSqlName(owner);
			
			sb.Append("on " + ownerSqlName + TableExt + "\r\n");
			sb.Append("REFERENCING NEW AS NEW OLD AS OLD\r\n");
			sb.Append("FOR EACH ROW\r\n");
			sb.Append("BEGIN\r\n");
			
			
			if(Source == "SQL")
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
			
			sb.Append("\r\nEND;\r\n/\r\n\r\n");
			
			return sb.ToString();
			
		}
		#endregion
		#region GetSqlName
		public System.String GetSqlName(Dersa.Interfaces.ICompiledEntity owner)
		{
string ownerSqlName = GetOwnerSqlName(owner);
			if (ownerSqlName.Length > 0)
			{
				return ownerSqlName + TableExt + "$" + this.Name;
			}
			return this.Name;
		}
		#endregion
		#region Drop
		public System.String Drop(System.Boolean dialog, Dersa.Interfaces.ICompiledEntity owner)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string sqlName = this.GetSqlName(owner);
			
			sb.Append("if exists (select * from sysobjects where id = object_id('dbo." + sqlName + "') and sysstat & 0xf = 8)\n");
			sb.Append("\tdrop trigger dbo." + sqlName + "\n");
			sb.Append("go\n\n");
			
			string sOut = sb.ToString();
			if (dialog)
			{
				SqlExecForm.Exec(sOut);
				return "";
			}
			return sOut;
		}
		#endregion
		#region GetOwnerSqlName
		public System.String GetOwnerSqlName(Dersa.Interfaces.ICompiledEntity owner)
		{
if (owner == null)
			{
				owner = this.Parent;
				if (owner == null)
				{
					return "";
				}
			}
			string ownerSqlName = "";
			if (owner is Entity) ownerSqlName = ((Entity)owner).GetTableName();
			else if (owner is View) ownerSqlName = ((View)owner).GetSqlName();
			return ownerSqlName;
		}
		#endregion
		#region ora_Generate
		public object ora_Generate()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("CREATE OR REPLACE ");
			sb.Append(this.SQL);
			return sb.ToString();
			
		}
		#endregion
		#endregion
	}
}
