using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class View: StereotypeBaseE, ICompiledEntity
	{
		public View(){}

		public View(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String SQL = "";
		public System.String Source = "SQL";
		public System.String PhisicalName = "";

		#region Ìועמהû
		#region Generate
		public System.String Generate(System.Boolean dialog, Dersa.Interfaces.ICompiledEntity owner)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string sqlName = this.GetSqlName();
			
			sb.Append("if exists(select * from sysobjects where type = 'V' and id = object_id('" + sqlName + "')) \n");
			sb.Append("	drop view dbo." + sqlName + "\n");
			sb.Append("go\n");
			sb.Append("create view dbo." + sqlName + "\n");
			string comma = "(";
			
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] is Attribute)
				{
					sb.Append(comma + (children[i] as Attribute).GetSqlName());
					comma = ", ";
				}
			}
			if(comma != "(") sb.Append(")\n");
			sb.Append("as\n");
			
			sb.Append(SQL);
			
			sb.Append("\n");
			sb.Append("go\n");
			sb.Append("grant select on dbo." + sqlName + " to public \n");
			sb.Append("go \n");
			
			System.Collections.IList m = this.GetTriggers();
			for (int i = 0; i < m.Count; i++)
			{
				sb.Append((m[i] as Trigger).Generate(false, this));
			}
			
			string sOut = sb.ToString();
			if (dialog)
			{
				string serverName = "";
				string dbName = "";
				if (Parent is Package) 
				{
					serverName = ((Package)Parent).GetDatabaseServer();
					dbName = ((Package)Parent).GetDBName();
				}
				SqlExecForm.Exec(sOut, serverName, dbName);
				return "";
			}
			return sOut;
		}
		#endregion
		#region GetSqlName
		public System.String GetSqlName()
		{
string s = this.PhisicalName;
			if ((s != null)&&(s.Length > 0))
			{
				return s;
			}
			else
			{
				return this.Name.ToUpper();
			}
		}
		#endregion
		#region GetTriggers
		public System.Collections.IList GetTriggers()
		{
System.Collections.IList triggers = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if ((rel is Inherit)&&(((Inherit)rel).InheritTriggers))
				{
					System.Collections.IList inherited_triggers = ((View)rel.B).GetTriggers();
					for (int k = 0; k < inherited_triggers.Count; k++)
					{
						triggers.Add(inherited_triggers[k]);
					}
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Trigger)
				{
					triggers.Add(obj);	
				}
			}
			return triggers;
		}
		#endregion
		#endregion
	}
}
