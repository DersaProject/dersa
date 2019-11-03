using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Type: StereotypeBaseE, ICompiledEntity
	{
		public Type(){}

		public Type(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String PhisicalName = "";
		public System.String Get = "";
		public System.String Set = "";
		public System.Boolean GenerateTable = false;
		public System.String SqlType = "int";
		public System.String NameSQLType = "varchar(40)";
		public System.Boolean Abstract = false;
		public System.String Default = "";
		public System.String Null = "";
		public System.String DIOSDefault = "";
		public System.Boolean FlagsAttribute = false;

		#region Методы
		#region GetEnumTypeName
		public string GetEnumTypeName()
		{
            Entity eP = this.Parent as Entity;
            string prefix = "";
            if (eP != null)
            {
                prefix = Static.GetCSharpObjectName(eP);
            }
            //Console.WriteLine(prefix + Static.GetCSharpObjectName(this));
            return prefix + Static.GetCSharpObjectName(this);

		}
		#endregion
		#region GetPKAttributes
		public System.Collections.IList GetPKAttributes()
		{
System.Collections.ArrayList attrs = new System.Collections.ArrayList();
			Attribute attr = new Attribute(null);
			attr.Name = "#";
			attr.PhisicalName = this.GetSqlName();
			attr.Type = this.SqlType;
			attr.Null = "not null";
			attr.Default = this.Default;
			attr.Description = "PRIMARY KEY";
			attr.Get = this.Get;
			attr.Set = this.Set;
			attr.DIOSDefault = this.DIOSDefault;
			attrs.Add(attr);
			return attrs;
		}
		#endregion
		#region GetSqlName
		public System.String GetSqlName()
		{
string s = this.PhisicalName;
if (s.Length > 0)
{
	return s;
}
else
{
	return this.Name.ToUpper();
}
		}
		#endregion
		#region GenerateEnum
		public System.String GenerateEnum(System.Boolean dialog, System.Boolean separatly)
		{
            Package package = this.Parent as Package;
            if (package == null)
                package = this.Parent.Parent as Package;
            if (package == null && separatly) return "";
            if (Abstract) return "";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (separatly)
            {
                string globalName = package.GetGlobalName();
                sb.Append("using System;\nusing " + globalName + ".Common.Interfaces;\nusing " + globalName + ".Common;\n");
                sb.Append("\nnamespace " + package.Namespace + "\n{\n");
            }

            if (FlagsAttribute)
            {
                sb.Append("\t[Flags]\n");
            }
            sb.Append("\tpublic enum " + /*Static.GetCSharpObjectName(this)*/GetEnumTypeName() + ": " + Static.GetCSharpNativeType(this.SqlType) + " {");
            string comma = "";
            System.Collections.IList consts = this.GetConstants();
            foreach (Const obj in consts)
            {
                sb.Append(comma + obj.Name);
                if ((obj.Value != null) && (obj.Value != ""))
                {
                    sb.Append(" = " + obj.Value);
                }
                comma = ", ";
            }
            sb.Append("}\n");
            if (!separatly) return sb.ToString();

            sb.Append("}");

            string cSharpName = GetEnumTypeName();//Static.GetCSharpObjectName(this);
            string fileName = package.GetDirectory() + "\\" + cSharpName + ".cs";
            Static.SaveToFile(fileName, sb.ToString());
            Console.WriteLine("Сформирован: " + fileName);
            //System.Windows.Forms.Application.DoEvents();
            return sb.ToString();

		}
		#endregion
		#region Generate
		public System.String Generate(System.Boolean dialog, System.Boolean doDrop)
		{
string sqlName = GetSqlName();
			string pkName = sqlName.ToLower();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			if (doDrop)
			{
				sb.Append(DropTable(false));
			}
			
			if (GenerateTable)
			{
				sb.Append("create table dbo." + sqlName + " (\n");
				sb.Append("	" + pkName + " " + SqlType + " not null,\n");
				if ((NameSQLType != null)&&(NameSQLType.Length > 0))
				{
					sb.Append("	name " + NameSQLType + " null,\n");
				}
				sb.Append("	primary key(" + pkName + ")\n");
				sb.Append(")\ngo\n");
				sb.Append("grant select on dbo." + sqlName + " to public\n");
				sb.Append("go\n");
				sb.Append(GenerateValues());
			}
			else
			{
				bool prc = false;
				string union = "";
				System.Collections.IList consts = this.GetConstants();
			
				System.Collections.IList children = this.Children;
				for (int i = 0; i < children.Count; i++)
				{
					ICompiledEntity obj = (ICompiledEntity)children[i];
					if ((obj is Procedure)&&(obj.Name == "List"))
					{
						prc = true;
					}
				}
				if(!prc)
				{
					sb.Append("if object_id('" + sqlName + "$List') is not null \n");
					sb.Append("	drop procedure " + sqlName + "$List \n");
					sb.Append("\ngo \n");
					sb.Append(" create procedure " + sqlName + "$List \n");
					sb.Append("	@class sysname = '" + sqlName + "', \n");
					sb.Append("	@where varchar(255) = NULL, \n");
					sb.Append("	@order varchar(255) = NULL \n");
					sb.Append("as \n");
					
					foreach (Const cons_t in consts)
					{
						sb.Append(union);
						union = "union \n";
						sb.Append("select " + pkName + " = " + cons_t.Value + ", name = '" + cons_t.Name + "' \n");
					}
					sb.Append("order by 1 \n");
					sb.Append("\ngo \n");
					sb.Append("grant execute on " + sqlName + "$List to public \n");
					sb.Append("\ngo \n");
				}
			
				sb.Append("if exists (select * from dbo.sysobjects where id = object_id('dbo." + sqlName + "@Name') and xtype in (N'FN', N'IF', N'TF')) \n");
				sb.Append("drop function dbo." + sqlName + "@Name \n");
				sb.Append("go \n");
				sb.Append("create function dbo." + sqlName + "@Name \n");
				sb.Append("	(@" + pkName + " int) \n");
				sb.Append("RETURNS varchar(255) AS  \n");
				sb.Append("BEGIN  \n");
				sb.Append("declare \n");
				sb.Append("	@res varchar(255) \n");
				sb.Append("select \n");
				sb.Append("	@res =  \n");
				sb.Append("		case \n");
				foreach (Const cons_t in consts)
				{
					sb.Append("			when @" + pkName + " = " + cons_t.Value + " then '" + cons_t.Name + "' \n");
				}
				sb.Append("			else 'Неопределен' \n");
				sb.Append("		end \n");
				sb.Append("return @res \n");
				sb.Append("END \n");
				sb.Append("go \n");
				sb.Append("GRANT  EXECUTE  ON dbo." + sqlName + "@Name  TO [public] \n");
				sb.Append("go \n");
			}
			sb.Append(GenerateStoredProcedures(false));
			string sOut = sb.ToString();
			if (dialog)
			{
				SqlExecForm.Exec(sOut);
				return null;
			}
			return sOut;
		}
		#endregion
		#region DropTable
		public System.String DropTable(System.Boolean dialog)
		{
string sOut = "";
			string sqlName = this.GetSqlName();
			
			sOut += "if object_id('dbo." + sqlName + "') is not null\n";
			sOut += "\tdrop table dbo." + sqlName + "\n";
			sOut += "go\n";
			if (dialog)
			{
				SqlExecForm.Exec(sOut);
				return null;
			}
			return sOut;
		}
		#endregion
		#region GenerateStoredProcedures
		public System.String GenerateStoredProcedures(System.Boolean dialog)
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Collections.IList m = this.GetStoredProcedures();
            for (int i = 0; i < m.Count; i++)
            {
                sb.Append((m[i] as Procedure).Generate(this));
            }

            string sOut = sb.ToString();
            if (dialog)
            {
                SqlExecForm.Exec(sOut);
                return "";
            }
            return sOut;

		}
		#endregion
		#region GetStoredProcedures
		public System.Collections.IList GetStoredProcedures()
		{
System.Collections.IList sps = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if ((rel is Inherit)&&(((Inherit)rel).InheritProcedures))
				{
					System.Collections.IList inherited_sps = ((Type)rel.B).GetStoredProcedures();
					for (int k = 0; k < inherited_sps.Count; k++)
					{
						sps.Add(inherited_sps[k]);
					}
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Procedure)
				{
					sps.Add(obj);
				}
			}
			return sps;
		}
		#endregion
		#region GenerateValues
		public System.String GenerateValues()
		{
string sqlName = GetSqlName();
			string pkName = sqlName.ToLower();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			System.Collections.IList pkAttrs = this.GetPKAttributes();
			Attribute attribute = (Attribute)pkAttrs[0];
			
			System.Collections.IList consts = this.GetConstants();
			foreach (Const cons_t in consts)
			{
				if ((NameSQLType != null)&&(NameSQLType.Length > 0))
				{
					sb.Append("if not exists(select * from " + sqlName + " (nolock) where " + pkName + " = " + cons_t.Value + ")\n");
					sb.Append("\tinsert into " + sqlName + "(" + pkName + ", name) values(" + cons_t.Value + ", '" + cons_t.Name + "')\n");
					sb.Append("else");
					sb.Append("\tupdate " + sqlName + " set name = '" + cons_t.Name + "' where " + pkName + " = " + cons_t.Value + "\n");
				}
				else
				{
					sb.Append("if not exists(select * from " + sqlName + " (nolock) where " + pkName + " = " + cons_t.Value + ")\n");
					sb.Append("\tinsert into " + sqlName + " values(" + cons_t.Value + ")\n");
				}
			}
			
			if (consts.Count > 0) sb.Append("go\n");
			return sb.ToString();
		}
		#endregion
		#region GetConstants
		public System.Collections.IList GetConstants()
		{
Map contsts = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					System.Collections.IList inheritedConsts = ((Type)rel.B).GetConstants();
					foreach (Const c in inheritedConsts)
					{
						contsts.Add(c.GetName(), c);
					}
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Const)
				{
					Const cnst = (Const)obj;
					contsts.Add(cnst.GetName(), cnst);
				}
			}
			return contsts;
		}
		#endregion
		#endregion
	}
}
