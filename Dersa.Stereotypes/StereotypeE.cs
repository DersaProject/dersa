using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.IO;

namespace DersaStereotypes
{
	[Serializable()]
	public class StereotypeE: StereotypeBaseE, ICompiledEntity
	{
		public StereotypeE(){}

		public StereotypeE(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string Using = "";
		public string ViewFormat = "";
		public string IconPath = "";
		public Map attributes;

		#region Методы
		#region AllowExecuteMethod
		public override bool AllowExecuteMethod(string userName, string methodName)
		{
        int userPermissions = Dersa.Common.DersaUtil.GetUserPermissions(userName);
				return (userPermissions & 4) > 0;
			
		}
		#endregion
		#region SaveStereotype
		public string SaveStereotype(object[] Params)
		{
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\Build\\Stereotypes\\" + this.Name + ".cs";
			            string backupFileName = AppDomain.CurrentDomain.BaseDirectory + "\\Build\\Stereotypes\\bak\\" + this.Name + ".cs";
			            string deepBackupFileName = AppDomain.CurrentDomain.BaseDirectory + "\\Build\\Stereotypes\\bak\\" + this.Name + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + ".cs";
			            if (File.Exists(backupFileName))
			                File.Move(backupFileName, deepBackupFileName);
			            if (File.Exists(fileName))
			                File.Move(fileName, backupFileName);
			            string body = this.GenerateStereotype();
			            Static.SaveToFile(fileName, body);
			            return string.Format("alert('Для стереотипа {0} записан файл.');", this.Name);
		}
		#endregion
		#region RemoveStereotypeFile
		public string RemoveStereotypeFile(object[] Params)
		{
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\Build\\Stereotypes\\" + this.Name + ".cs";
			            if (File.Exists(fileName))
			                File.Delete(fileName);
			            return string.Format("alert('файл стереотипа {0} удален.');", this.Name);
		}
		#endregion
		#region sqlUpdateStereotype
		public dynamic sqlUpdateStereotype()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("update STEREOTYPE\r\n");
			sb.Append("\tset view_format = '" + this.ViewFormat.Replace("'", "''") + "',\r\n");
			sb.Append("\t\t\ticon_path = '" + this.IconPath + "'\r\n");
			sb.Append("\t\twhere name = '" + this.Name + "'\r\n");
			
			return sb.ToString();
			
		}
		#endregion
		#region sqlGenerateOperations
		public dynamic sqlGenerateOperations()
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			            sb.Append("declare xstereotype int;\r\n");
			            sb.Append("begin\r\n");
			            sb.Append("select stereotype into xstereotype\r\n");
			            sb.Append("\tfrom STEREOTYPE\r\n");
			            sb.Append("\t\twhere name = '" + this.Name + "';\r\n\r\n");
			            sb.Append("delete from METHOD where stereotype = xstereotype;\r\n\r\n");
			            System.Collections.IList methods = this.GetMethods(true);
			            for (int i = 0; i < methods.Count; i++)
			            {
			                Method M = methods[i] as Method;
			                if (M.AccessModifier != "published")
			                    continue;
			                string resultType = "null";
			                if (M.ReturnType == "SqlExecForm")
			                    resultType = "1";
			                if (M.ReturnType == "ExecJScript")
			                    resultType = "2";
			                sb.Append("insert into METHOD(method, stereotype, name, result_type) values(METHOD_SEQ.nextval, xstereotype, '" + M.Name + "', " + resultType + ");\r\n");
			            }
				    sb.Append("commit;\r\n");
				    sb.Append("\r\n");
				    sb.Append("end;\r\n");
			
			            return sb.ToString();
			
		}
		#endregion
		#region GetMethods
		public System.Collections.IList GetMethods(bool forSQL)
		{
            Map methods = new Map();
			            System.Collections.IList children = this.Children;
			            for (int i = 0; i < children.Count; i++)
			            {
			                Method obj = children[i] as Method;
			                if (obj == null) continue;
			                methods.Add(obj.GetMapKey(), obj);
			            }
			            if (forSQL)
			            {
			                System.Collections.IList relations = this.ARelations;
			                for (int i = 0; i < relations.Count; i++)
			                {
			                    ICompiledRelation rel = (ICompiledRelation)relations[i];
			                    if (rel is Inherit)
			                    {
			                        Inherit inhRel = rel as Inherit;
			                        string prefix = inhRel.Prefix;
			                        string namePrefix = inhRel.NamePrefix;
			                        if (prefix == null) prefix = "";
			                        if (namePrefix == null) namePrefix = "";
			                        if (!(rel.B is StereotypeE))
			                            continue;
			                        StereotypeE rB = (StereotypeE)rel.B;
			                        System.Collections.IList inDictionary = rB.GetMethods(forSQL);
			                        foreach (Method m in inDictionary)
			                        {
			                            if (m.AccessModifier == "published")
			                                methods.Add(m.GetMapKey(), m);
			                        }
			                    }
			                }
			            }
			
			            return methods;
			
			
		}
		#endregion
		#region GetAttributes
		public System.Collections.IList GetAttributes(bool forSQL)
		{
            if (attributes != null) return attributes;
			            attributes = new Map();
			
			            System.Collections.IList children = this.Children;
			            for (int i = 0; i < children.Count; i++)
			            {
			                ICompiledEntity obj = (ICompiledEntity)children[i];
			                if (obj is Attribute)
			                {
			                    attributes.Add(((Attribute)obj).GetSqlName(), obj);
			                }
			            }
			            if (forSQL)
			            {
			                System.Collections.IList relations = this.ARelations;
			                for (int i = 0; i < relations.Count; i++)
			                {
			                    ICompiledRelation rel = (ICompiledRelation)relations[i];
			                    if (rel is Inherit)
			                    {
			                        Inherit inhRel = rel as Inherit;
			                        string prefix = inhRel.Prefix;
			                        string namePrefix = inhRel.NamePrefix;
			                        if (prefix == null) prefix = "";
			                        if (namePrefix == null) namePrefix = "";
			                        if (!(rel.B is StereotypeE))
			                            continue;
			                        StereotypeE rB = (StereotypeE)rel.B;
			                        System.Collections.IList inDictionary = rB.GetAttributes(forSQL);
			                        foreach (Attribute a in inDictionary)
			                        {
			                            if (a.PhisicalName == null)
			                            {
			                                a.PhisicalName = prefix + a.Name;
			                                a.Name = namePrefix + a.Name;
			                            }
			                            else
			                            {
			                                a.Name = namePrefix + a.Name;
			                                a.PhisicalName = prefix + a.PhisicalName;
			                            }
			                            attributes.Add(a.GetSqlName(), a);
			                        }
			                    }
			                }
			            }
			            return attributes;
			
		}
		#endregion
		#region GenerateStereotype
		public System.String GenerateStereotype()
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			            System.Text.StringBuilder sbInterfaces = new System.Text.StringBuilder();
			            System.Text.StringBuilder sbNamespace = new System.Text.StringBuilder();
			
			            Package package = this.Parent as Package;
			            if (package == null) return "Не найдена папка";
			
			            string defUsing = "using System;\r\nusing System.Runtime.Serialization;\r\nusing Dersa.Interfaces;\r\n";
			            sbNamespace.Append(defUsing);
			            sbNamespace.Append(this.Using);
			            sbNamespace.Append("\r\n");
			            string NS = package.Namespace;
			            if (NS == null) NS = "";
			            while (NS == "")
			            {
			                package = package.Parent as Package;
			                if (package == null) break;
			                NS = package.Namespace;
			                if (NS == null) NS = "";
			            }
			            sbNamespace.Append("namespace " + package.Namespace + "\r\n{\r\n");
			            sb.Append("\t[Serializable()]\r\n");
			            sb.Append("\tpublic ");
			
			            string baseClassName = "StereotypeBaseE";
			            System.Collections.IList rels = this.ARelations;
			            for (int i = 0; i < rels.Count; i++)
			            {
			                Inherit Inh = rels[i] as Inherit;
			                if (Inh != null)
			                    baseClassName = Inh.B.Name;
			                Implement Impl = rels[i] as Implement;
			                if (Impl == null)
			                    continue;
			                Interface Ifc = Impl.B as Interface;
			                if (Ifc == null)
			                    continue;
			                sbInterfaces.Append(", " + Ifc.GetFullName());
			            }
			
			            sb.Append("class " + Name + ": " + baseClassName + ", ICompiledEntity");
			            //if(!string.IsNullOrEmpty(this.Interface))
			            //    sb.Append(", " + this.Interface);
			            sb.Append(sbInterfaces.ToString());
			
			            sb.Append("\r\n\t{\r\n");
			
			            //конструктор
			
			            sb.Append("\t\tpublic " + Name + "(){}\r\n\r\n");
			            sb.Append("\t\tpublic ");
			            sb.Append(Name);
			            sb.Append("(IDersaEntity obj)");
			            sb.Append("\r\n\t\t{");
			            sb.Append("\r\n\t\t\t_object = obj;");
			            sb.Append("\r\n\t\t\tif (_object != null)");
			            sb.Append("\r\n\t\t\t{");
			            sb.Append("\r\n\t\t\t\t_name = _object.Name;");
			            sb.Append("\r\n\t\t\t\t_id = _object.Id;");
			            sb.Append("\r\n\t\t\t}");
			            sb.Append("\r\n\t\t}\r\n");
			
			            System.Collections.IList attrs = this.GetAttributes(false);
			
			            foreach (Attribute attr in attrs)
			            {
			                string attr_name = attr.Name;
			                string attr_type = attr.Type;
			                string attr_default = attr.Default;
			                if (!string.IsNullOrEmpty(attr.PropertyName))
			                    sb.Append("\t\t#region " + attr_name + "\r\n");
			                sb.Append("\t\tpublic " + attr_type + " " + attr_name);
			                bool AttributeIsString = System.Type.GetType(attr_type.Trim()) == typeof(string) || attr_type.ToLower().Trim() == "string";
			                if (attr_default == "" && !AttributeIsString)
			                    attr_default = null;
			                if (attr_default != null || AttributeIsString)
			                {
			                    if (attr_default == null)
			                        attr_default = "";
			                    sb.Append(" = ");
			                    if (AttributeIsString)
			                        sb.Append("\"");
			                    sb.Append(attr_default.Trim());
			                    if (AttributeIsString)
			                        sb.Append("\"");
			                }
			                sb.Append(";\r\n");
			                if (!string.IsNullOrEmpty(attr.PropertyName))
			                {
			                    sb.Append("\t\tpublic " + attr_type + " " + attr.PropertyName);
			                    sb.Append("\r\n\t\t{\r\n");
			                    sb.Append("\t\t\tget\r\n");
			                    sb.Append("\t\t\t{\r\n");
					    if(!string.IsNullOrEmpty(attr.Get))
			                    	sb.Append(attr.Get + "\r\n");
					    else
			                    	sb.Append("\t\t\t\treturn " + attr_name + ";\r\n");
			                    sb.Append("\t\t\t}\r\n");
			                    sb.Append("\t\t\tset\r\n");
			                    sb.Append("\t\t\t{\r\n");
			                    sb.Append("\t\t\t\t" + attr_name + " = value;\r\n");
			                    sb.Append("\t\t\t}\r\n");
			                    sb.Append("\t\t}\r\n");
			                }
			                if (!string.IsNullOrEmpty(attr.PropertyName))
			                    sb.Append("\t\t#endregion\r\n");
			            }
			
			            //Формирование методов
			            sb.Append("\r\n\t\t#region Методы\r\n");
			
			            System.Collections.IList methods = this.GetMethods(false);
			
			            for (int i = 0; i < methods.Count; i++)
			            {
			                Method m = methods[i] as Method;
			                if (m != null)
			                {
			                    sb.Append("\t\t#region " + m.Name + "\r\n");
			                    if(!string.IsNullOrEmpty(m.Attributes))
			                    {
			                        sb.Append(m.Attributes);
			                        sb.Append("\r\n");
			                    }
			                    sb.Append(m.GetDeclarationString().Replace("published", "public")
			                        .Replace("SqlExecForm", "object")
			                        .Replace("ExecJScript", "string"));
			
			                    sb.Append("\r\n\t\t{\r\n");
			                    string MethodBody = m.Text;
			                    MethodBody = MethodBody
			                             .Replace("\r", "")
			                             .Replace("\n", "\r\n\t\t\t");
			                    sb.Append(MethodBody);
			                    sb.Append("\r\n\t\t}\r\n");
			                    sb.Append("\t\t#endregion\r\n");
			                }
			            }
			
			            sb.Append("\t\t#endregion\r\n");
			
			            sb.Append("\t}\r\n");
			            sb.Append("}\r\n");
			
			            string result = sbNamespace.ToString() + sb.ToString();
			            return result;
		}
		#endregion
		#region sqlGenerateStereotype
		public dynamic sqlGenerateStereotype()
		{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
					sb.Append("declare xstereotype int; xattribute int; xparent int;\r\n");
					sb.Append("begin\r\n");
					sb.Append("select stereotype into xstereotype\r\n");
					sb.Append("\tfrom STEREOTYPE\r\n");
			                sb.Append("\t\twhere name = '" + this.Name + "';\r\n\r\n");
					sb.Append("if xstereotype is null then begin\r\n");
					sb.Append("select max(stereotype) + 1 into xstereotype \r\n");
					sb.Append("\tfrom STEREOTYPE;\r\n");
					sb.Append("\tinsert into STEREOTYPE(stereotype, name, icon_path, view_format) values(xstereotype");
				    sb.Append(", '" + this.Name + "'");
				    sb.Append(", '" + this.IconPath + "'");
				    sb.Append(", '" + this.ViewFormat.Replace("'", "''") + "'");
					sb.Append(");\r\n");
					sb.Append("end;\r\n");
					sb.Append("end if;\r\n");
			
					System.Collections.IList rels = this.ARelations;
			            for (int i = 0; i < rels.Count; i++)
			            {
			                Relation R = rels[i] as Relation;
			                if (R == null)
			                    continue;
			
					sb.Append("select stereotype into xparent from STEREOTYPE where name = '" + R.B.Name + "';\r\n");
					sb.Append("insert into CHILD_STEREOTYPE(child_stereotype, parent, stereotype) \r\n");
					sb.Append(" select CHILD_STEREOTYPE_SEQ.nextval, xparent, xstereotype from dual where not exists(select 1 from CHILD_STEREOTYPE where parent = xparent and stereotype = xstereotype);\r\n");
			            }
					sb.Append("commit;\r\n");
					sb.Append("end;\r\n");
			
					return sb.ToString();
			
			
			sb.Append("declare @stereotype int, @attribute int, @parent int;\r\n");
			            sb.Append("select @stereotype = stereotype\r\n");
			            sb.Append("\tfrom STEREOTYPE(nolock)\r\n");
			            sb.Append("\t\twhere name = '" + this.Name + "';\r\n\r\n");
			            sb.Append("if @stereotype is null begin\r\n");
			            sb.Append("select @stereotype = max(stereotype) + 1\r\n");
			            sb.Append("\tfrom STEREOTYPE(nolock)\r\n");
			            sb.Append("\tinsert into STEREOTYPE(stereotype, GUID, name, icon_path, view_format) values(@stereotype, newid()");
				    sb.Append(", '" + this.Name + "'");
				    sb.Append(", '" + this.IconPath + "'");
				    sb.Append(", '" + this.ViewFormat.Replace("'", "''") + "'");
				    sb.Append(")\r\n");
			            sb.Append("end\r\n");
		}
		#endregion
		#endregion
	}
}
