using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using DIOS.Common;
using Dersa.Common;

namespace DersaStereotypes
{
	[Serializable()]
	public class Entity: StereotypeBaseE, ICompiledEntity
	{
		public Entity(){}

		public Entity(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String PhisicalName = "";
		public System.Boolean Abstract = false;
		public System.Boolean MakePK = true;
		public System.Boolean AutoPK = true;
		public System.Boolean NoRefObjects = false;
		public System.String Interfaces = "";
		public System.String Comment = "";
		public System.String LogDBExtention = "_log";
		public System.String ViewName = "";
		public System.Int32 MaxCount = -1;
		public System.String Using = "";
		public Map attributes;
		public System.String KeyType = "int";
		public System.Boolean ImplementInterface = true;
		public System.Boolean CanCreateManualy = false;
		public System.Int32 CachingPolicy = 1;
		public System.Boolean AllowExternalModification = false;
		public System.String ModifyUniView = "";
		public System.Boolean HasIdentityKey = true;

		#region Методы
		#region RegisterForOracle
		public string RegisterForOracle()
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			            string key_name = GetPKName();
			            string cSharpName = Static.GetCSharpObjectName(this);
			            string database_server = "";
			            string local_namespace = "";
			            string assembly_name = "";
			            if (this.Parent is Package)
			            {
			                database_server = ((Package)this.Parent).GetDatabaseServer();
			                local_namespace = ((Package)this.Parent).Namespace;
			                assembly_name = ((Package)this.Parent).AssemblyName;
			            }
			
			            string database_name = GetDBName();
			
			            sb.Append("\tinsert into OBJECT_TYPE (object_type, name, class_name, table_name,  key_name, type_name, assembly_name, database_name, records_limit)\n");
			            sb.Append("\tvalues ((select nvl(max(object_type), 0) + 1 from OBJECT_TYPE), '" + this.Name + "','" + this.GetSqlName() + "','" + this.GetSqlName() + "', '" + key_name);
			            sb.Append("', '" + local_namespace + "." + cSharpName + "'" + ", '" + assembly_name + "'");
			            sb.Append(", '" + database_name + "', 500)\n");
			
			
			//            SqlExecForm.Exec(sb.ToString());
			//            Console.WriteLine(sb.ToString());
			
			            return sb.ToString();
			
		}
		#endregion
		#region UnRegister
		public string UnRegister()
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			            sb.Append("\tdelete from OBJECT_TYPE where class_name = '");
			            sb.Append(this.GetSqlName());
			            sb.Append("'\n");
			            sb.Append("go\n");
			            sb.Append("commit\n");
			
			            return sb.ToString();
			
		}
		#endregion
		#region AddVersionControl
		public string AddVersionControl(object[] callParams)
		{
            DersaUtil.EntitySetStereotype(callParams[0].ToString(), this.Id, "EntityVC");
			        return "alert('stereotype changed')";
		}
		#endregion
		#region sql_Generate
		public string sql_Generate()
		{
            System.Boolean doDrop = true;
			            System.Boolean doSP = true;
			            System.Boolean doTrig = true;
			            System.Boolean doRef = true;
			
			            string comma = "";
			            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			            string sqlName = this.GetSqlName();
			            if (!this.Abstract)
			            {
			                if (doDrop)
			                {
			                    sb.Append(this.sql_DropTable());
			                }
			                sb.Append("create table dbo." + sqlName + "\n(");
			                System.Collections.IList attrs = this.GetAttributes();
			                foreach (Attribute attr in attrs)
			                {
			                    sb.Append(comma + "\n\t" + attr.GetSqlName() + " ");
			                    string AttrType = attr.Type;
			                    if (attr.GetSqlName() != this.GetPKName())
			                        AttrType = AttrType.Replace("identity", "").Trim();
			                    else if (AttrType == "int" && this.HasIdentityKey)
			                        AttrType = "int identity";
			                    sb.Append(AttrType + " ");
			                    sb.Append(attr.Null);
			                    string attr_default = attr.Default;
			                    if ((attr_default != null) && (attr_default.Length > 0))
			                    {
			                        sb.Append(" default " + attr_default);
			                    }
			                    comma = ",";
			                }
			                System.Collections.IList pk_attrs = this.GetPKAttributes();
			                if ((pk_attrs != null) && (pk_attrs.Count > 0))
			                {
			                    string cm = "";
			                    sb.Append(comma + "\n\t" + "primary key (");
			                    for (int i = 0; i < pk_attrs.Count; i++)
			                    {
			                        Attribute attr = (Attribute)pk_attrs[i];
			                        sb.Append(cm + attr.GetSqlName());
			                        cm = ", ";
			                    }
			                    sb.Append(")");
			                }
			                if (doRef)
			                {
			                    Map fk_attrs = this.GetFKLinks() as Map;
			                    if ((fk_attrs != null) && (fk_attrs.Count > 0))
			                    {
			                        for (int i = 0; i < fk_attrs.Count; i++)
			                        {
			                            Entity link = fk_attrs[i] as Entity;
			                            if (link != null)
			                            {
			                                sb.Append(comma + "\n\t" + "foreign key (" + fk_attrs.KeyAt(i) + ") references " + link.GetSqlName());
			                            }
			                        }
			                    }
			                }
			                sb.Append("\n)");
			
			                sb.Append("\ngo\n");
			                sb.Append("grant " + GetPermissions() + " on dbo." + sqlName + " to " + GetRoleNames() + "\n");
			                sb.Append("go\n");
			
			                sb.Append(this.GenerateIndexes(this));
			
			                object insertData = this.ExecuteScript("InsertData", this, new object[0]);
			                if (insertData != null)
			                {
			                    sb.Append((string)insertData);
			                }
			
			                object generateAdditional = this.ExecuteScript("GenerateAdditional", this, new object[] { false });
			                if (generateAdditional != null)
			                {
			                    sb.Append((string)generateAdditional);
			                }
			                else
			                {
			                    sb.Append("/* No Additional Script */\n");
			                }
			            }
			
			            if (doSP)
			            {
			                sb.Append(this.sql_GenerateStoredProcedures());
			                sb.Append(this.sql_GenerateFunctions());
			            }
			
			            if (doTrig)
			            {
			                sb.Append(this.sql_GenerateTriggers(false));
			            }
			
			            sb.Append(GenerateView(this));
			
			            string sOut = sb.ToString();
			            object sOutCorrected = ExecuteScript("Corrections", this, new object[] { sOut });
			            if (sOutCorrected != null)
			            {
			                sOut = sOutCorrected.ToString();
			            }
			            //if (dialog)
			            //{
			            //    string serverName = null;
			            //    if (Parent is Package) serverName = ((Package)Parent).GetDatabaseServer();
			            //    SqlExecForm.Exec(sOut, serverName, this.GetDBName(), this.Id, sOut, this.sql_Alter(false, true, true));
			            //    return null;
			            //}
			            return sOut;
			
		}
		#endregion
		#region GetAttributes
		public System.Collections.IList GetAttributes()
		{
if (attributes != null) return attributes;
			attributes = new Map();
			Attribute primaryKey = this.GetPrimaryKey();
			if (primaryKey != null) //throw new Exception("Не определен первичный ключ");
			{
				attributes.Add(primaryKey.GetSqlName(), primaryKey);
			}
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
					Entity rB = (Entity)rel.B;
					System.Collections.IList inDictionary = rB.GetAttributes();
					foreach (Attribute a in inDictionary)
					{
						if (inhRel.InheritPK || rB.GetPKName() != a.GetSqlName())
						{
							if (!inhRel.InheritGetAccessors)
								a.Get = null;
							if (!inhRel.InheritSetAccessors)
								a.Set = null;
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
				else if (rel is Relation)
				{
					System.Collections.IList inCollection = ((Relation)rel).GetAttributesForA();
					foreach (Attribute a in inCollection)
					{
						attributes.Add(a.GetSqlName(), a);
					}
				}
			}
			relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Relation)
				{
					System.Collections.IList inCollection = ((Relation)rel).GetAttributesForB();
					foreach (Attribute a in inCollection)
					{
						attributes.Add(a.GetSqlName(), a);
					}
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Attribute)
				{
					attributes.Add(((Attribute)obj).GetSqlName(), obj);
				}
				else if (obj is Type)
				{		
					Type local_type = obj as Type;
					Attribute attr = new Attribute(null);
					attr.Name = local_type.Name.ToLower();
					attr.PhisicalName = local_type.PhisicalName.ToLower();
					attr.Type = local_type.SqlType;
					attr.Null = local_type.Null;
					
					string csTypeName = Static.GetCSharpObjectName(local_type);
					string typeSqlName = attr.GetSqlName();
					string csType = Static.GetCSharpNativeType(local_type.SqlType);
					attr.EnumTypeName = csTypeName;
					attr.DIOSDefault = local_type.DIOSDefault;
					attr.Get = "\t\t\t\treturn (" + csType + ")this." + csTypeName + ";";
					attr.Set = "\t\t\t\tthis." + csTypeName + " = (" + csTypeName + ")value.Value;";
					attributes.Add(attr.GetSqlName(), attr);
				}
				else if (obj is StateEngine)
				{
					StateEngine local_type = obj as StateEngine;
					Attribute attr = new Attribute(null);
					attr.Name = local_type.Name.ToLower();
					attr.PhisicalName = local_type.PhisicalName.ToLower();
					attr.Type = local_type.SqlType;
					attr.Null = "not null";
					
					string csTypeName = Static.GetCSharpObjectName(local_type);
					string typeSqlName = attr.GetSqlName();
					string csType = Static.GetCSharpNativeType(local_type.SqlType);
					attr.DIOSDefault = local_type.DIOSDefault;
					attr.Get = "\t\t\t\treturn (" + csType + ")this." + csTypeName + ";";
					attr.Set = "\t\t\t\tthis." + csTypeName + " = (" + csTypeName + ")value.Value;";
					attributes.Add(attr.GetSqlName(), attr);
				}
				else if (obj is Ref)
				{
					Ref reff = obj as Ref;
					string pName = reff.GenerateName(); 
					string interf = reff.Interface;
					string prefix = reff.Prefix;
					
					string key = prefix + "_ref";
					Attribute attr = new Attribute(null);
					attr.Name = key;
					attr.Type = reff.RefObjectIdType;
					attr.Null = "null";
					attr.Default = "";
					attr.Description = "";
					attr.UseInWebForm = true;
					if ((reff.set_ref != null)&&(reff.set_ref.Length > 0))
					{
						attr.Set = reff.set_ref;
					}
					else
					{
						attr.Set = "\t\t\t\t_" + key + " = value;";
					}
					attributes.Add(attr.GetSqlName(), attr);
					
					
					key = prefix + "_class";
					attr = new Attribute(null);
					attr.Name = key;
					attr.Type = "varchar(128)";
					attr.Null = "null";
					attr.Default = "";
					attr.Description = "";
					attr.UseInWebForm = true;
					if ((reff.set_class != null)&&(reff.set_class.Length > 0))
					{
						attr.Set = reff.set_class;
					}
					else
					{
						attr.Set = "\t\t\t\t_" + key + " = value;";
					}
					attributes.Add(attr.GetSqlName(), attr);
				}
			}
			return attributes;
		}
		#endregion
		#region GetProperties
		public System.Collections.IList GetProperties()
		{
Map properties = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit && ((Inherit)rel).InheritProperties)
				{
					System.Collections.IList inList = ((Entity)rel.B).GetProperties();
					foreach (Property p in inList)
					{
						properties.Add(p.Name, p);
					}
				}
				if (rel is Relation)
				{
					Relation relation = (Relation)rel;
					bool isNull = false;
					if (relation.GenerateForA(out isNull) && relation.B is Type && !((Type)relation.B).GenerateTable)
					{
						Type localType = (Type)relation.B;
						string typeSqlName = localType.Name.ToLower();
						if ((relation.BRole != null)&&(relation.BRole != ""))
						{
							typeSqlName = relation.BRole;
						}
						else 
						{
							string typePhisicalName = localType.PhisicalName.ToLower();
							if ((typePhisicalName != null)&&(typePhisicalName != ""))
							{
								typeSqlName = typePhisicalName;
							}
						}
						string csTypeName = Static.GetCSharpObjectName(localType);
						string csType = Static.GetCSharpNativeType(localType.SqlType);
						string csPropertyName = Static.GetCSharpName(typeSqlName);
						
						Property prop = new Property(null);
						prop.Name = csPropertyName;
						prop.Type = csTypeName;
						prop.ReadOnly = false;
						prop.AccessModifier = "public";
						prop.Interface = true;
						prop.Get = "\t\t\t\treturn (" + csTypeName + ") _" + typeSqlName + ".Value;";
						if ((relation.Set != null)&&(relation.Set.Length > 0))
						{
							prop.Set = relation.Set;
						}
						else
						{
							prop.Set = "\t\t\t\t_" + typeSqlName + " = (" + csType + ")value;";
						}
						properties.Add(prop.Name, prop);
					}
					if (relation.B is StateEngine)
					{
						StateEngine localType = (StateEngine)relation.B;
						string typeSqlName = localType.Name.ToLower();
						string typePhisicalName = localType.PhisicalName.ToLower();
						if ((typePhisicalName != null)&&(typePhisicalName != ""))
						{
							typeSqlName = typePhisicalName;
						}
						string csTypeName = Static.GetCSharpObjectName(localType);
						string csType = Static.GetCSharpNativeType(localType.SqlType);
						
						Property prop = new Property(null);
						prop.Name = csTypeName;
						prop.Type = csTypeName;
						prop.ReadOnly = false;
						prop.AccessModifier = "public";
						prop.Interface = true;
						if ((localType.Get != null)&&(localType.Get.Length > 0))
						{
							prop.Get = localType.Get;
						}
						else
						{
							prop.Get = "\t\t\t\treturn (" + csTypeName + ") _" + typeSqlName + ".Value;";
						}
						if ((localType.Set != null)&&(localType.Set.Length > 0))
						{
							prop.Set = localType.Set;
						}
						else
						{
							prop.Set = "\t\t\t\t_" + typeSqlName + " = (" + csType + ")value;";
						}
						properties.Add(prop.Name, prop);
					}
				}
			}
			relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Relation)
				{
					Relation relation = (Relation)rel;
					bool isNull = false;
					if (relation.GenerateForB(out isNull) && relation.A is Type && !((Type)relation.A).GenerateTable)
					{
						Type localType = (Type)relation.A;
						string typeSqlName = localType.Name.ToLower();
						if ((relation.ARole != null)&&(relation.ARole != ""))
						{
							typeSqlName = relation.ARole;
						}
						else 
						{
							string typePhisicalName = localType.PhisicalName.ToLower();
							if ((typePhisicalName != null)&&(typePhisicalName != ""))
							{
								typeSqlName = typePhisicalName;
							}
						}
						string csTypeName = Static.GetCSharpObjectName(localType);
						string csType = Static.GetCSharpNativeType(localType.SqlType);
						string csPropertyName = Static.GetCSharpName(typeSqlName);
						
						Property prop = new Property(null);
						prop.Name = csPropertyName;
						prop.Type = csTypeName;
						prop.ReadOnly = false;
						prop.AccessModifier = "public";
						prop.Interface = true;
						prop.Get = "\t\t\t\treturn (" + csTypeName + ") _" + typeSqlName + ".Value;";
						if ((relation.Set != null)&&(relation.Set.Length > 0))
						{
							prop.Set = relation.Set;
						}
						else
						{
							prop.Set = "\t\t\t\t_" + typeSqlName + " = (" + csType + ")value;";
						}
						properties.Add(prop.Name, prop);
					}
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Property)
				{
					properties.Add(obj.Name, obj);
				}
				else if (obj is Type)
				{
					Type localType = (Type)obj;
					if (localType.GenerateTable) continue;
					string typeSqlName = localType.Name.ToLower();
					string typePhisicalName = localType.PhisicalName.ToLower();
					if ((typePhisicalName != null)&&(typePhisicalName != ""))
					{
						typeSqlName = typePhisicalName;
					}
					string csTypeName = Static.GetCSharpObjectName(localType);
					string csType = Static.GetCSharpNativeType(localType.SqlType);
					
					Property prop = new Property(null);
					prop.Name = csTypeName;
					prop.Type = csTypeName;
					prop.ReadOnly = false;
					prop.AccessModifier = "public";
					prop.Interface = true;
					if ((localType.Get != null)&&(localType.Get.Length > 0))
					{
						prop.Get = localType.Get;
					}
					else
					{
						prop.Get = "\t\t\t\treturn (" + csTypeName + ") _" + typeSqlName + ".Value;";
					}
					if ((localType.Set != null)&&(localType.Set.Length > 0))
					{
						prop.Set = localType.Set;
					}
					else
					{
						prop.Set = "\t\t\t\t_" + typeSqlName + " = (" + csType + ")value;";
					}
					properties.Add(prop.Name, prop);
				}
				else if (obj is StateEngine)
				{
					StateEngine localType = (StateEngine)obj;
					string typeSqlName = localType.Name.ToLower();
					string typePhisicalName = localType.PhisicalName.ToLower();
					if ((typePhisicalName != null)&&(typePhisicalName != ""))
					{
						typeSqlName = typePhisicalName;
					}
					string csTypeName = Static.GetCSharpObjectName(localType);
					string csType = Static.GetCSharpNativeType(localType.SqlType);
					
					Property prop = new Property(null);
					prop.Name = csTypeName;
					prop.Type = csTypeName;
					prop.ReadOnly = false;
					prop.AccessModifier = "public";
					prop.Interface = true;
					if ((localType.Get != null)&&(localType.Get.Length > 0))
					{
						prop.Get = localType.Get;
					}
					else
					{
						prop.Get = "\t\t\t\treturn (" + csTypeName + ") _" + typeSqlName + ".Value;";
					}
					if ((localType.Set != null)&&(localType.Set.Length > 0))
					{
						prop.Set = localType.Set;
					}
					else
					{
						prop.Set = "\t\t\t\t_" + typeSqlName + " = (" + csType + ")value;";
					}
					properties.Add(prop.Name, prop);
				}
			}
			return properties;
		}
		#endregion
		#region InheritsFrom
		public System.Boolean InheritsFrom(Entity fromEntity)
		{
System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation cr = (ICompiledRelation)relations[i];
				if (cr is Inherit)
				{
					if (cr.B.Id == fromEntity.Id) return true;
					bool result = ((Entity)cr.B).InheritsFrom(fromEntity);
					if (result) return true;
				}
			}
			return false;
		}
		#endregion
		#region sql_GenerateIndexes
		public string sql_GenerateIndexes()
		{
return GenerateIndexes(this);
		}
		#endregion
		#region GetUsing
		public System.String GetUsing()
		{
string result = "";
			if ((this.Using != null)&&(this.Using.Length > 0))
			{
				result += this.Using;
			}
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit && ((Inherit)rel).InheritUsing)
				{
					Entity bEntity = (rel as Inherit).B as Entity;
					result += ((rel as Inherit).B as Entity).GetUsing();
				}
			}
			return result;
		}
		#endregion
		#region GenerateRef
		public System.String GenerateRef(System.String refMethod)
		{
	return this.GenerateRef(refMethod, "");
		}
		#endregion
		#region GetStateEngines
		public System.Collections.IList GetStateEngines()
		{
Map stateEngines = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					System.Collections.IList inheritStateEngines = (rel.B as Entity).GetStateEngines();
					foreach(StateEngine se in inheritStateEngines)
					{
						stateEngines.Add(se.PhisicalName, se);
					}
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				StateEngine obj = children[i] as StateEngine;
				if (obj == null) continue;
				stateEngines.Add(obj.PhisicalName, obj);
			}
			return stateEngines;
		}
		#endregion
		#region GenerateStateEngines
		public System.String GenerateStateEngines()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList stateEngines = this.GetStateEngines();
			foreach(StateEngine se in stateEngines)
			{
				sb.Append(se.GenerateEngine());
			}
			
			return sb.ToString();
		}
		#endregion
		#region GenerateInterfaceStateEngines
		public System.String GenerateInterfaceStateEngines()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList stateEngines = this.GetStateEngines();
			foreach(StateEngine se in stateEngines)
			{
				sb.Append(se.GenerateInterfaceEngine());
			}
			
			return sb.ToString();
		}
		#endregion
		#region TestViewAttrs
		public string TestViewAttrs()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Text.StringBuilder sbInterface = new System.Text.StringBuilder();
			System.Collections.IList children = this.GetViewAttributes();
			for (int i = 0; i < children.Count; i++)
			{
				ViewAttribute obj = children[i] as ViewAttribute;
				if (obj == null) continue;
				string attr_name = obj.Name;
				string attr_type = obj.Type;
				string attr_phisical_name = obj.GetSqlName();
				string attr_default_csharp = obj.DIOSDefault;
				string attr_csharp_type = obj.GetCSharpType();
				string attr_get = obj.Get; if (attr_get == null) attr_get = "";
				string attr_set = obj.Set; if (attr_set == null) attr_set = "";
			
				sb.Append("\t\t#region " + attr_phisical_name + "\n");
				if (obj.CreatePrivateField)
				{
					sb.Append("\t\tprotected " + attr_csharp_type + " _" + attr_phisical_name);
					if ((attr_default_csharp != null)&&(attr_default_csharp.Length > 0))
					{
						sb.Append(" = " + attr_default_csharp);
					}
					sb.Append(";\n");
				}
				sb.Append("\t\t[ViewProperty]\n");
				sb.Append("\t\t[ObjectPropertyAttribute(\"" + attr_name + "\", false, " + (!(attr_set.Length > 0)).ToString().ToLower() + ")]\n");
				sbInterface.Append("\t\t[ObjectPropertyAttribute(\"" + attr_name + "\", false, " + (!(attr_set.Length > 0)).ToString().ToLower() + ")]\n");
			
				sb.Append("\t\tpublic " + attr_csharp_type + " " + attr_phisical_name + "\n\t\t{");
				sbInterface.Append("\t\t" + attr_csharp_type + " " + attr_phisical_name + "{get;");
				if (attr_set.Length > 0)
				{
					sbInterface.Append("set;");
				}
				sbInterface.Append("}\n");
					
				if (attr_get == "")
				{
					attr_get = "\t\t\t\treturn _" + attr_phisical_name + ";"; 
				}
				sb.Append("\n\t\t\tget\n\t\t\t{\n");
				sb.Append(attr_get);
				sb.Append("\n\t\t\t}\n");
				if (attr_set.Length > 0)
				{
					sb.Append("\t\t\tset\n\t\t\t{\n");
					sb.Append(attr_set);
					sb.Append("\n\t\t\t}\n");
				}
				sb.Append("\t\t}\n");
				sb.Append("\t\t#endregion\n");
				//sbProperties.Append(", " + attr_phisical_name);
			}
			
			return sb.ToString();
			
		}
		#endregion
		#region GenerateAdditionalObjectProperties
		public System.String GenerateAdditionalObjectProperties(System.Collections.IList m)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("\t\t#region AdditionalObjectProperties()\n");
			if (m == null)
			{
				m = GetCSharpAttributes(true);
			}
			sb.Append("\t\tpublic static IndexerPropertyDescriptorCollection AdditionalObjectProperties()\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t\tIndexerPropertyDescriptorCollection objectProperties = null;\n");
			sb.Append("\t\t\tobjectProperties = IndexerPropertyDescriptorCollection.Empty;\n");
			for (int i = 0; i < m.Count; i++)
			{
				Property p = (Property)m[i];
				sb.Append("\t\t\tobjectProperties.Add(new IndexerPropertyDescriptor(");
				sb.Append("\"" + p.Name + "\", ");
				sb.Append("new Attribute[0], ");
				sb.Append("\"" + p.Name + "\", ");
				sb.Append("typeof(string)));\n");
			}
			/*
			System.Collections.IList propEntities = this.GetPropertyEntities();
			for (int i = 0; i < propEntities.Count; i++)
			{
				Entity e = (Entity)propEntities[i];
				System.Console.WriteLine(e.Name);
				object[] scriptObjects = e.ExecuteScripts(new string[]{"GetPropertyType", "GetPropertyPrefix"}, e, new object[2]);
				Entity ptEntity = scriptObjects[0] as Entity;
				if (ptEntity == null) throw new Exception("Не найден тип для значения свойства " + e.Name);
				Const ptConst = scriptObjects[1] as Const;
				if (ptConst == null) throw new Exception("Не найден префикс для значения свойства " + e.Name);
				
				string csPropertyObjectName = Static.GetCSharpObjectName(ptEntity);
				sb.Append("\t\t\t// Дополнительные свойства по типу " + csPropertyObjectName + "\n");
				sb.Append("\t\t\tIObjectCollection fields = StaticObjectManager.Manager.GetEntity(\"" + ptEntity.GetSqlName() + "\").List(null);\n");
				sb.Append("\t\t\tfields.Sort(new PropertiesComparer(new SortProperty[]{new SortProperty(\"sequence\")}));\n");
				sb.Append("\t\t\tfor(int i = 0; i < fields.Count; i++)\n");
				sb.Append("\t\t\t{\n");
				sb.Append("\t\t\t	IIndexer tp = fields[i] as IIndexer;\n");
				sb.Append("\t\t\t	objectProperties.Add(new IndexerPropertyDescriptor(" + ptConst.GetName() + " + tp[\"" + ptEntity.GetPKName() + "\"].ToString(), new Attribute[0], tp[\"localized_name\"].ToString(), Util.GetType(tp[\"type\"].ToString())));\n");
				sb.Append("\t\t\t}\n");
			}
			*/
			sb.Append("\t\t\treturn objectProperties;\n");
			sb.Append("\t\t}\n");
			sb.Append("\t\t#endregion\n");
			return sb.ToString();
		}
		#endregion
		#region TestAddProps
		public void TestAddProps()
		{
System.Console.WriteLine(this.GenerateAdditionalObjectProperties(this.GetProperties()));
		}
		#endregion
		#region GetSqlView
		public object GetSqlView()
		{
System.Collections.IList children = this.Children;
			foreach (object obj in children)
			{
				if (obj is View)
				{
					return obj;	
				}
			}
			return null;
		}
		#endregion
		#region sql_DropTable
		public string sql_DropTable()
		{
                        string sOut = "";
						string sqlName = this.GetSqlName();
						
						sOut += "if object_id('dbo." + sqlName + "') is not null\n";
						sOut += "\tdrop table dbo." + sqlName + "\n";
						sOut += "go\n";
						return sOut;
			
		}
		#endregion
		#region GetPKAttributes
		public System.Collections.IList GetPKAttributes()
		{
System.Collections.IList attrs = new Map();
			Attribute primaryKey = this.GetPrimaryKey();
			if (primaryKey != null) 
			{
				attrs.Add(primaryKey);
			}
			return attrs;
			
			/*
			System.Collections.IList attrs = new Map();
			if (this.MakePK)
			{
				Attribute attr = new Attribute(null);
				attr.Name = this.GetSqlName().ToLower();
				attr.PhisicalName = this.GetSqlName().ToLower();
				attr.Type = "int";
				attr.Null = "not null";
				attr.Default = "";
				attr.Description = "PRIMARY KEY";
				attr.ReadOnly = false;
				attrs.Add(attr);
			}
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit && (rel as Inherit).InheritPK)
				{
					System.Collections.IList inCollection = ((Entity)rel.B).GetPKAttributes();
					foreach(Attribute a in inCollection)
					{
						attrs.Add(a);
					}
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Key)
				{
					Attribute attr = new Attribute(null);
					attr.Name = ((Key)obj).GetSqlName();
					attr.PhisicalName = ((Key)obj).GetSqlName();
					attr.Type = ((Key)obj).Type;
					attr.Null = ((Key)obj).Null;
					attr.Default = ((Key)obj).Default;
					attr.ReadOnly = false;
					attrs.Add(attr);
				}
			}
			return attrs;*/
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
		#region GenerateEnums
		public System.String GenerateEnums()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Type)
				{
					sb.Append(((Type)obj).GenerateEnum(false, false));
				}
			}
			
			System.Collections.IList stateEngines = this.GetStateEngines();
			foreach(StateEngine se in stateEngines)
			{
				sb.Append(se.GenerateEnum());
			}
			
			return sb.ToString();
		}
		#endregion
		#region GetPKName
		public System.String GetPKName()
		{
Attribute primaryKey = this.GetPrimaryKey();
			if (primaryKey != null) return primaryKey.GetSqlName();
			return "";
			
			/*string key = "";
			if (this.MakePK)
			{
				key = this.GetSqlName().ToLower();
			}
			System.Collections.IList children = new System.Collections.ArrayList(this.Children);
			
			
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					System.Collections.IList inheritedChildren = rel.B.Children;
					for (int j = 0; j < inheritedChildren.Count; j++)
					{
						children.Add(inheritedChildren[j]);
					}
				}
			}
			
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Key)
				{
					string newkey = ((Key)obj).GetSqlName();
					if (key.Length > 0)
					{
						if (key.IndexOf(newkey) == -1)
						{
							key += ", " + newkey;
						}
					}
					else
					{
						key = newkey;
					}
				}
			}
			return key;*/
		}
		#endregion
		#region GenerateRef
		public System.String GenerateRef(System.String refMethod, System.String prefix)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit && ((Inherit)rel).InheritRefs)
				{
					sb.Append(((rel as Inherit).B as Entity).GenerateRef(refMethod));
				}
				else if (rel is Relation)
				{
					System.Reflection.MethodInfo mi = rel.GetType().GetMethod(refMethod + "ForA");
					sb.Append((string)mi.Invoke(rel, null));
				}
			}
			relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				Relation rel = relations[i] as Relation;
				if (rel == null) continue;
				System.Reflection.MethodInfo mi = rel.GetType().GetMethod(refMethod + "ForB");
				sb.Append((string)mi.Invoke(rel, null));
			}
			return sb.ToString();
		}
		#endregion
		#region GenerateMethods
		public System.String GenerateMethods()
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			            sb.Append("\n\t\t#region GetFactory");
			            sb.Append("\n\t\tpublic static ObjectFactory GetFactory()");
			            sb.Append("\n\t\t{");
			            sb.Append("\n\t\t\tApplicationSqlManager M = CommonEnvironment.StaticManager;");
			            sb.Append("\n\t\t\tObjectFactory F = M.GetFactory(EntityClassName);");
			            sb.Append("\n\t\t\tM.IsOccupied = false;");
			            sb.Append("\n\t\t\treturn F;");
			            sb.Append("\n\t\t}");
			            sb.Append("\n\t\t#endregion\n");
			
			            if (!this.HasIdentityKey)
			            {
			                sb.Append("\n\t\t#region HasIdentityKey");
			                sb.Append("\n\t\tprotected override bool HasIdentityKey()");
			                sb.Append("\n\t\t{");
			                sb.Append("\n\t\t\treturn false;");
			                sb.Append("\n\t\t}");
			                sb.Append("\n\t\t#endregion\n");
			            }
			
			            System.Collections.IList attrs = this.GetMethods();
			            for (int i = 0; i < attrs.Count; i++)
			            {
			                sb.Append((attrs[i] as Method).Generate());
			            }
			            return sb.ToString();
			
		}
		#endregion
		#region GenerateRefs
		public System.String GenerateRefs()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit && ((Inherit)rel).InheritRefs)
				{
					sb.Append(((Entity)rel.B).GenerateRefs());
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				Ref reff = children[i] as Ref;
				if (reff == null) continue;
				sb.Append(reff.Generate());
			}
			return sb.ToString();
		}
		#endregion
		#region GenerateProperties
		public System.String GenerateProperties()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList children = this.GetProperties();
			for (int i = 0; i < children.Count; i++)
			{
				Property prop = (Property)children[i];
				sb.Append(prop.Generate());
			}
			return sb.ToString();
		}
		#endregion
		#region GenerateInterfaceMethods
		public System.String GenerateInterfaceMethods()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList methods = this.GetMethods();
			for (int i = 0; i < methods.Count; i++)
			{
				Method method = methods[i] as Method;
				if (method != null) sb.Append(method.GenerateInterface());
			}
			return sb.ToString();
		}
		#endregion
		#region GetMethods
		public System.Collections.IList GetMethods()
		{
Map methods = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if ((rel is Inherit)&&(((Inherit)rel).InheritMethods))
				{
					System.Collections.IList inheritmethods = (rel.B as Entity).GetMethods();
					foreach(Method M in inheritmethods)
					{
						if(!M.DoNotInherit)
						{
							//inheritmethods.Remove(M);	
							methods.Add(M.GetMapKey(), M);
						}
					}
					//methods.AddRange(inheritmethods);
				}
				else if (rel is Relation)
				{
					System.Collections.IList inCollection = (rel as Relation).GenerateCollectionAccessMethodForA();
					foreach(Method M in inCollection)
					{
						methods.Add(M.GetMapKey(), M);
					}
				}
			}
			relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Relation)
				{
					System.Collections.IList inCollection = (rel as Relation).GenerateCollectionAccessMethodForB();
					foreach(Method M in inCollection)
					{
						methods.Add(M.GetMapKey(), M);
					}
				}
			}
			System.Collections.IList additionalMethods = this.ExecuteScript("GetAdditionalMethods", this, new object[0]) as System.Collections.IList;
			if (additionalMethods != null)
			{
				foreach(Method M in additionalMethods)
				{
					methods.Add(M.GetMapKey(), M);
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				Method obj = children[i] as Method;
				if (obj == null) continue;
				methods.Add(obj.GetMapKey(), obj);
			}
			return methods;
		}
		#endregion
		#region GenerateInterfaceRefs
		public System.String GenerateInterfaceRefs()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				Inherit rel = relations[i] as Inherit;
				if (rel == null) continue;
				if (rel.InheritRefs)
					sb.Append((rel.B as Entity).GenerateInterfaceRefs());
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				Ref reff = children[i] as Ref;
				if (reff == null) continue;
				sb.Append(reff.GenerateInterface());
			}
			return sb.ToString();
		}
		#endregion
		#region GenerateInterfaceProperties
		public System.String GenerateInterfaceProperties()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList children = this.GetProperties();
			for (int i = 0; i < children.Count; i++)
			{
				Property prop = (Property)children[i];
				sb.Append(prop.GenerateInterface());
			}
			return sb.ToString();
		}
		#endregion
		#region GenerateViewStruct
		public System.String GenerateViewStruct()
		{
throw new Exception ("GenerateViewStruct is depricated");
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Text.StringBuilder properties = new System.Text.StringBuilder();
			System.Text.StringBuilder propertiesWithType = new System.Text.StringBuilder();
			System.Text.StringBuilder setProperties = new System.Text.StringBuilder();
			
			string cSharpName = Static.GetCSharpObjectName(this);
			System.Collections.IList attrs = this.GetAttributes();
				
			sb.Append("\t[Serializable]\n");
			sb.Append("\tpublic struct " + cSharpName + "View:IViewStruct\n");
			sb.Append("\t{\n"); 
			
			int index = 0;	
			foreach (Attribute attr in attrs)
			{
				string attr_name = attr.Name;
				string attr_type = attr.Type;
				string attr_phisical_name = attr.GetSqlName();
				string attr_csharp_type = attr.GetCSharpType();
				string attr_DIOS_type = attr.GetDIOSType();
				sb.Append("\t\tprivate " + attr_csharp_type + " _" + attr_phisical_name + ";\n");
				if (attr_name != attr_phisical_name)
				{
					sb.Append("\t\t[LocalizedName(\"" + attr_name + "\")]\n");
				}
				sb.Append("\t\tpublic " + attr_DIOS_type +  " " + attr_phisical_name + "\n\t\t{\n");
				if (attr_csharp_type == attr_DIOS_type)
				{
					sb.Append("\t\t\tget\n\t\t\t{\n\t\t\t\treturn this._" + attr_phisical_name + ";\n\t\t\t}\n");
					setProperties.Append("\t\t\tthis._" + attr_phisical_name + " = " + attr_phisical_name + ";\n");
				}
				else
				{
					sb.Append("\t\t\tget\n\t\t\t{\n\t\t\t\treturn (" + attr_DIOS_type + ")(int)this._" + attr_phisical_name + ";\n\t\t\t}\n");
					setProperties.Append("\t\t\tthis._" + attr_phisical_name + " = (int)" + attr_phisical_name + ";\n");
				}
				sb.Append("\t\t}\n");
				propertiesWithType.Append(attr_DIOS_type + " " + attr_phisical_name);
				properties.Append(attr_phisical_name);
			
				if (index < attrs.Count - 1)
				{
					properties.Append(", ");
					propertiesWithType.Append(", ");
					if (index == 25)
					{
						propertiesWithType.Append("\n");
					}
				}
				index++;
			}
			//Дополняем ViewAttribute
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ViewAttribute obj = children[i] as ViewAttribute;
				if (obj == null) continue;
				string attr_name = obj.Name;
				string attr_type = obj.Type;
				string attr_phisical_name = obj.GetSqlName();
				string attr_csharp_type = Static.GetCSharpType(attr_type);
				string attr_DIOS_type = obj.GetCSharpType();
				
				properties.Append(", ");
				propertiesWithType.Append(", ");
				sb.Append("\t\t#region " + attr_phisical_name + "\n");
				sb.Append("\t\tprivate " + attr_csharp_type + " _" + attr_phisical_name + ";\n");
				if (attr_name != attr_phisical_name)
				{
					sb.Append("\t\t[LocalizedName(\"" + attr_name + "\")]\n");
				}
				sb.Append("\t\tpublic " + attr_DIOS_type + " " + attr_phisical_name + "\n\t\t{\n");
				if (attr_csharp_type == attr_DIOS_type)
				{
					sb.Append("\t\t\tget\n\t\t\t{\n\t\t\t\treturn this._" + attr_phisical_name + ";\n\t\t\t}\n");
					setProperties.Append("\t\t\tthis._" + attr_phisical_name + " = " + attr_phisical_name + ";\n");
				}
				else
				{
					sb.Append("\t\t\tget\n\t\t\t{\n\t\t\t\treturn (" + attr_DIOS_type + ")(int)this._" + attr_phisical_name + ";\n\t\t\t}\n");
					setProperties.Append("\t\t\tthis._" + attr_phisical_name + " = (int)" + attr_phisical_name + ";\n");
				}
				sb.Append("\t\t}\n");
				sb.Append("\t\t#endregion\n");
				propertiesWithType.Append(attr_DIOS_type + " " + attr_phisical_name);
				properties.Append(attr_phisical_name);
			}
			sb.Append("\t\tpublic " + cSharpName + "View(" + propertiesWithType.ToString() + ")\n\t\t{\n");
			sb.Append(setProperties.ToString());
			sb.Append("\t\t}\n");
			
			sb.Append("\t\tpublic object this[string propName]\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t\tget\n");
			sb.Append("\t\t\t{\n");
			sb.Append("\t\t\t\treturn Util.GetProperty(this, propName);\n");
			sb.Append("\t\t\t}\n");
			sb.Append("\t\t}\n");
			sb.Append(this.GenerateStreamMethod());
			sb.Append("\t}\n");
			return sb.ToString();
		}
		#endregion
		#region GenerateStreamMethod
		public System.String GenerateStreamMethod()
		{
string cSharpName = Static.GetCSharpObjectName(this);
			
			System.Text.StringBuilder sbTo = new System.Text.StringBuilder();
			System.Text.StringBuilder sbFrom = new System.Text.StringBuilder();
			
			sbTo.Append("\t\tpublic void ToStream(DIOS.Common.BinaryWriter bw)\n");
			sbFrom.Append("\t\tpublic void FromStream(DIOS.Common.BinaryReader br)\n");
			sbTo.Append("\t\t{\n");
			sbFrom.Append("\t\t{\n");
			sbFrom.Append("\t\t\tbool isNull;\n");
			sbFrom.Append("\t\t\tint byteArrayLen;\n");
			
			System.Collections.IList attrs = this.GetAttributes();
			foreach (Attribute attr in attrs)
			{
				string attr_name = attr.Name;
				string attr_type = attr.Type;
				string attr_phisical_name = attr.GetSqlName();
				string attr_csharp_type = attr.GetCSharpType();
				string attr_DIOS_type = attr.GetDIOSType();
				string attr_native_type = Static.GetCSharpNativeType(attr_type);
				
				sbTo.Append("\t\t\tbw.Write(_" + attr_phisical_name + ".IsNull);\n");
				sbTo.Append("\t\t\tif (!_" + attr_phisical_name + ".IsNull)\n");
				sbTo.Append("\t\t\t{\n");
				if (attr_native_type == "Binary")	
				{
					sbTo.Append("\t\t\t\tbyte[] b = _" + attr_phisical_name + ".Value;\n");
					sbTo.Append("\t\t\t\tif (b != null)\n");
					sbTo.Append("\t\t\t\t{\n");
					sbTo.Append("\t\t\t\t\tbw.Write(b.Length);\n");
					sbTo.Append("\t\t\t\t\tbw.Write(b);\n");
					sbTo.Append("\t\t\t\t}\n");
					sbTo.Append("\t\t\t\telse\n");
					sbTo.Append("\t\t\t\t{\n");
					sbTo.Append("\t\t\t\t\tbw.Write((int)0);\n");
					sbTo.Append("\t\t\t\t}\n");
				}
				else
				{
					sbTo.Append("\t\t\t\tbw.Write(_" + attr_phisical_name + ".Value);\n");
				}
				sbTo.Append("\t\t\t}\n");
			
				sbFrom.Append("\t\t\tisNull = br.ReadBoolean();\n");
				sbFrom.Append("\t\t\tif (!isNull)\n");
				sbFrom.Append("\t\t\t{\n");
				if (attr_native_type == "Binary")	
				{
					sbFrom.Append("\t\t\t\tbyteArrayLen = br.ReadInt32();\n");
					sbFrom.Append("\t\t\t\tif (byteArrayLen > 0)\n");
					sbFrom.Append("\t\t\t\t{\n");
					sbFrom.Append("\t\t\t\t\tbyte[] b = br.ReadBytes(byteArrayLen);\n");
					sbFrom.Append("\t\t\t\t\t_" + attr_phisical_name + " = new SqlBinary(b);\n");
					sbFrom.Append("\t\t\t\t}\n");
				}
				else
				{
					sbFrom.Append("\t\t\t\t" + attr_native_type + " b = br.Read" + attr_native_type + "();\n");
					sbFrom.Append("\t\t\t\t_" + attr_phisical_name + " = new "  + attr_csharp_type + "(b);\n");
				}
				sbFrom.Append("\t\t\t}\n");
			}
			
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ViewAttribute obj = children[i] as ViewAttribute;
				if (obj == null) continue;
				string attr_name = obj.Name;
				string attr_type = obj.Type;
				string attr_phisical_name = obj.GetSqlName();
				string attr_csharp_type = Static.GetCSharpType(attr_type);
				string attr_DIOS_type = obj.GetCSharpType();
				string attr_native_type = Static.GetCSharpNativeType(attr_type);
			
				sbTo.Append("\t\t\tbw.Write(_" + attr_phisical_name + ".IsNull);\n");
				sbTo.Append("\t\t\tif (!_" + attr_phisical_name + ".IsNull)\n");
				sbTo.Append("\t\t\t{\n");
				if (attr_native_type == "Binary")	
				{
					sbTo.Append("\t\t\t\tbyte[] b = _" + attr_phisical_name + ".Value;\n");
					sbTo.Append("\t\t\t\tif (b != null)\n");
					sbTo.Append("\t\t\t\t{\n");
					sbTo.Append("\t\t\t\t\tbw.Write(b.Length);\n");
					sbTo.Append("\t\t\t\t\tbw.Write(b);\n");
					sbTo.Append("\t\t\t\t}\n");
					sbTo.Append("\t\t\t\telse\n");
					sbTo.Append("\t\t\t\t{\n");
					sbTo.Append("\t\t\t\t\tbw.Write((int)0);\n");
					sbTo.Append("\t\t\t\t}\n");
				}
				else
				{
					sbTo.Append("\t\t\t\tbw.Write(_" + attr_phisical_name + ".Value);\n");
				}
				sbTo.Append("\t\t\t}\n");
			
				sbFrom.Append("\t\t\tisNull = br.ReadBoolean();\n");
				sbFrom.Append("\t\t\tif (!isNull)\n");
				sbFrom.Append("\t\t\t{\n");
				if (attr_native_type == "Binary")	
				{
					sbFrom.Append("\t\t\t\tbyteArrayLen = br.ReadInt32();\n");
					sbFrom.Append("\t\t\t\tif (byteArrayLen > 0)\n");
					sbFrom.Append("\t\t\t\t{\n");
					sbFrom.Append("\t\t\t\t\tbyte[] b = br.ReadBytes(byteArrayLen);\n");
					sbFrom.Append("\t\t\t\t\t_" + attr_phisical_name + " = new SqlBinary(b);\n");
					sbFrom.Append("\t\t\t\t}\n");
				}
				else
				{
					sbFrom.Append("\t\t\t\t" + attr_native_type + " b = br.Read" + attr_native_type + "();\n");
					sbFrom.Append("\t\t\t\t_" + attr_phisical_name + " = new "  + attr_csharp_type + "(b);\n");
				}
				sbFrom.Append("\t\t\t}\n");
			}
			
			sbTo.Append("\t\t}\n");
			sbFrom.Append("\t\t}\n");
			return sbTo.ToString() + sbFrom.ToString();
		}
		#endregion
		#region GetFKLinks
		public System.Collections.IList GetFKLinks()
		{
Map links = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit && ((Inherit)rel).InheritFKLinks)
				{
					Inherit inh = rel as Inherit;
					Map inCollection = (Map)((Entity)rel.B).GetFKLinks();
					for (int k = 0; k < inCollection.Count; k++)
					{
						object o = inCollection[k];
						string key = inCollection.KeyAt(k) as string;
						if (inh.Prefix != null)
							key = inh.Prefix + key;
						links.Add(key, o);
					}
				}
				else if (rel is Relation)
				{
					bool isNull = false;
					if ((((Relation)rel).MakeFK)&&((Relation)rel).GenerateForA(out isNull))
					{
						Entity B = ((Relation)rel).B as Entity;
						if (B != null)
						{
							string bRole = ((Relation)rel).BRole;
							if ((bRole == null)||(bRole == "")) bRole = B.GetPKName();
							links.Add(bRole, rel.B);
						}
					}
				}
			}
			relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Relation)
				{
					bool isNull = false;
					if ((((Relation)rel).MakeFK)&&((Relation)rel).GenerateForB(out isNull))
					{
						Entity A = ((Relation)rel).A as Entity;
						if (A != null)
						{
							string aRole = ((Relation)rel).ARole;
							if ((aRole == null)||(aRole == "")) aRole = A.GetPKName();
							links.Add(aRole, rel.A);
						}
					}
				}
			}
			return links;
		}
		#endregion
		#region GenerateIndexes
		public System.String GenerateIndexes(ICompiledEntity owner)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit && ((Inherit)rel).InheritIndices)
				{
					sb.Append(((Entity)rel.B).GenerateIndexes(owner));
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Index)
				{
					sb.Append(((Index)obj).Generate(false, owner));
				}
			}
			sb.Append("\ngo\n");
			
			string indexTablespace = null;
			Package entityPackage = this.Parent as Package;
			entityPackage.Reinitialize();
			Configuration dbSettings = entityPackage.GetConfiguration("Db Settings");
			if(dbSettings != null)
			{
			     indexTablespace = dbSettings.GetSetting("Index Tablespace");
			}
			
			Map fk_index_attrs = this.GetFKIndexLinks() as Map;
			if ((fk_index_attrs != null)&&(fk_index_attrs.Count > 0))
			{
				for (int i = 0; i < fk_index_attrs.Count; i++)
				{
					Entity link = fk_index_attrs[i] as Entity;
					if (link != null)
					{
			//sb.Append("\n if exists(select * from sysindexes (NOLOCK)\n");
			//sb.Append("\twhere indid not in (0, 255) and name = '" + this.GetSqlName() + "_" + link.GetSqlName() + "_FK_IDX" + "')\n");
			//sb.Append("\t\tdrop index " + this.GetSqlName() + "." + this.GetSqlName() + "_" + link.GetSqlName() + "_FK_IDX");
			//sb.Append("\ngo\n");
			sb.Append("create index " + this.GetSqlName() + "_" + link.GetSqlName() + "_FK_IDX");
			sb.Append(" on " + this.GetSqlName() + "(" + fk_index_attrs.KeyAt(i) + ")");
			     if(indexTablespace != null)
			         sb.Append(" tablespace " + indexTablespace + "\n");
			sb.Append("\ngo\n");
			
					}
				}
			}
			sb.Append("\ngo\n");
			
			return sb.ToString();
		}
		#endregion
		#region sql_GenerateStoredProcedures
		public string sql_GenerateStoredProcedures()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList m = this.GetStoredProcedures();
			for (int i = 0; i < m.Count; i++)
			{
				sb.Append((m[i] as Procedure).Generate(this));
			}
			
			string sOut = sb.ToString();
			return sOut;
		}
		#endregion
		#region GenerateInitObjectProperties
		public System.String GenerateInitObjectProperties(System.Collections.IList m)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("\t\t#region InitObjectProperties()\n");
			if (m == null)
			{
				m = GetCSharpAttributes(true);
			}
			sb.Append("\t\tpublic static IndexerPropertyDescriptorCollection InitObjectProperties()\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t\tIndexerPropertyDescriptorCollection objectProperties = null;\n");
			sb.Append("\t\t\tobjectProperties = IndexerPropertyDescriptorCollection.Empty;\n");
			for (int i = 0; i < m.Count; i++)
			{
				Attribute a = (Attribute)m[i];
				if (!a.SuppressInView)
				{
					sb.Append("\t\t\tobjectProperties.Add(new IndexerPropertyDescriptor(");
					sb.Append("\"" + a.GetSqlName() + "\", ");
					sb.Append("new Attribute[0], ");
					sb.Append("\"" + a.Name + "\", ");
					sb.Append("typeof(" + a.GetDIOSType() + ")));\n");
				}
			}
			System.Collections.IList propEntities = this.GetPropertyEntities();
			for (int i = 0; i < propEntities.Count; i++)
			{
				Entity e = (Entity)propEntities[i];
				System.Console.WriteLine(e.Name);
				object[] scriptObjects = e.ExecuteScripts(new string[]{"GetPropertyType", "GetPropertyPrefix"}, e, new object[2]);
				Entity ptEntity = scriptObjects[0] as Entity;
				if (ptEntity == null) throw new Exception("Не найден тип для значения свойства " + e.Name);
				Const ptConst = scriptObjects[1] as Const;
				if (ptConst == null) throw new Exception("Не найден префикс для значения свойства " + e.Name);
				
				string csPropertyObjectName = Static.GetCSharpObjectName(ptEntity);
				sb.Append("\t\t\t// Дополнительные свойства по типу " + csPropertyObjectName + "\n");
				sb.Append("\t\t\tIObjectCollection fields = StaticObjectManager.Manager.GetEntity(\"" + ptEntity.GetSqlName() + "\").List(null);\n");
				sb.Append("\t\t\tfields.Sort(new PropertiesComparer(new SortProperty[]{new SortProperty(\"sequence\")}));\n");
				sb.Append("\t\t\tfor(int i = 0; i < fields.Count; i++)\n");
				sb.Append("\t\t\t{\n");
				sb.Append("\t\t\t	IIndexer tp = fields[i] as IIndexer;\n");
				sb.Append("\t\t\t	objectProperties.Add(new IndexerPropertyDescriptor(" + ptConst.GetName() + " + tp[\"" + ptEntity.GetPKName() + "\"].ToString(), new Attribute[0], tp[\"localized_name\"].ToString(), Util.GetType(tp[\"type\"].ToString())));\n");
				sb.Append("\t\t\t}\n");
			}
			sb.Append("\t\t\treturn objectProperties;\n");
			sb.Append("\t\t}\n");
			sb.Append("\t\t#endregion\n");
			return sb.ToString();
		}
		#endregion
		#region GetCSharpAttributes
		public System.Collections.IList GetCSharpAttributes(System.Boolean withViewAttributes)
		{
Map attrs = ((Map)GetAttributes()).Clone();
			if (withViewAttributes)
			{
				System.Collections.IList inCollection = this.GetViewAttributes();
				for (int i = 0; i < inCollection.Count; i++)
				{
					ViewAttribute obj = inCollection[i] as ViewAttribute;
					if (obj == null) continue;
					Attribute attr = new Attribute(null);
					attr.Name = obj.Name;
					attr.PhisicalName = obj.PhisicalName;
					attr.Type = obj.Type;
					attr.DIOSType = obj.DIOSType;
					attr.DIOSDefault = obj.DIOSDefault;
					attr.Get = obj.Get;
					attr.Set = obj.Set;
					attrs.Add(attr.GetSqlName(), attr);
				}
			}
			return attrs;
		}
		#endregion
		#region GenerateGetUniView
		public System.String GenerateGetUniView(System.Collections.IList m)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (m == null)
			{
				m = GetCSharpAttributes(true);
			}
			sb.Append("\t\tprotected override  UniStructView GetUniView()\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t\tIndexerPropertyDescriptorCollection props = this.GetObjectProperties();\n");
			sb.Append("\t\t\tobject[] dataStore = new object[props.Count];\n");
			sb.Append("\t\t\tint i = 0;\n");
			for(int i = 0; i < m.Count; i++)
			{
				Attribute a = m[i] as Attribute;
				if (!a.SuppressInView)
				{
					sb.Append("\t\t\tdataStore[i++] = " + a.GetSqlName() + ";\n");
				}
			}
			sb.Append("\t\t\tfor(int k = i; k < props.Count; k++)\n");
			sb.Append("\t\t\t{\n");
			sb.Append("\t\t\t\tdataStore[k] = this[props[k].Name];\n");
			sb.Append("\t\t\t}\n");
			sb.Append("\t\t\tUniStructView result = new UniStructView(dataStore, props);\n");
			sb.Append(this.ModifyUniView);
			sb.Append("\t\t\treturn result;\n");
			sb.Append("\t\t}\n");
			return sb.ToString();
		}
		#endregion
		#region GenerateForm
		public void GenerateForm()
		{

		}
		#endregion
		#region sql_Alter
		public string sql_Alter()
		{
            System.Boolean doSP = true;
			            System.Boolean doTrig = true;
			            try
			            {
			                System.Text.StringBuilder sb = new System.Text.StringBuilder();
			                if (!Abstract)
			                {
			                    System.Collections.IList attrs = GetAttributes();
			                    /*string db_name = "";
			                    if (LogDBExtention != "")
			                    {
			                        db_name = GetDBName() + LogDBExtention + ".";
			                    }*/
			                    string sqlName = GetSqlName();
			                    for (int i = 0; i < attrs.Count; i++)
			                    {
			                        Attribute a = (Attribute)attrs[i];
			                        if (a.Description == "PRIMARY KEY")
			                            continue;
			                        string attrSqlName = a.GetSqlName();
			                        sb.Append("if not exists(select * from syscolumns where id = OBJECT_ID('"
			                            + sqlName + "') and name = '" + attrSqlName + "')\n");
			                        sb.Append("\talter table " + sqlName + " add " + attrSqlName + " " + a.Type.Replace("identity", "") + " ");
			                        string attrDefault = a.Default;
			                        if (attrDefault == null) attrDefault = "";
			
			                        if (attrDefault.Length > 0)
			                        {
			                            sb.Append(a.Null);
			                            sb.Append(" default " + attrDefault);
			                        }
			                        else
			                        {
			                            sb.Append("null");
			                        }
			                        sb.Append("\n");
			
			                        /*sb.Append("if OBJECT_ID('" + db_name + "dbo." + sqlName + "_LOG') is not null and not exists(select * from " + db_name + "dbo." + "syscolumns where id = OBJECT_ID('"
			                            + db_name + "dbo." + sqlName + "_LOG') and name = '" + attrSqlName + "')\n");
			                        sb.Append("\talter table " + db_name + "dbo." + sqlName + "_LOG add " + attrSqlName + " " + a.Type + " null");
			                        if (attrDefault.Length > 0)
			                        {
			                            sb.Append(" default " + attrDefault);
			                        }
			                        sb.Append("\n");*/
			
			                        //		sb.Append("if OBJECT_ID('" + sqlName + "_WRK') is not null and not exists(select * from syscolumns where id = OBJECT_ID('"
			                        //			+ sqlName + "_WRK') and name = '" + attrSqlName + "')\n");
			                        //		sb.Append("\talter table " + sqlName + "_WRK add " + attrSqlName + " " + a.Type + " null");
			                        //		if (attrDefault.Length > 0)
			                        //		{
			                        //			sb.Append(" default " + attrDefault);
			                        //		}
			                        //		sb.Append("\n");
			                    }
			
			                    Map m = (Map)GetFKLinks();
			                    for (int i = 0; i < m.Count; i++)
			                    {
			                        Entity e = m[i] as Entity;
			                        sb.Append("if not exists(select * from sysreferences r join syscolumns c");
			                        sb.Append(" on r.fkey1 = c.colid and c.id = r.fkeyid\n");
			                        sb.Append("where r.fkeyid = OBJECT_ID('" + sqlName + "')");
			                        sb.Append(" and r.rkeyid = OBJECT_ID('" + e.GetSqlName() + "')");
			                        sb.Append(" and c.name = '" + m.KeyAt(i) + "')\n");
			                        sb.Append("\talter table " + sqlName + " add ");
			                        sb.Append("foreign key (" + m.KeyAt(i) + ") references " + e.GetSqlName());
			                        sb.Append("\n");
			                    }
			                    sb.Append("\ngo\n");
			                    sb.Append((string)this.ExecuteScript("InsertData", this, new object[0]));
			
			                    object alterAdditional = this.ExecuteScript("AlterAdditional", this, new object[] { });
			                    if (alterAdditional != null)
			                    {
			                        sb.Append((string)alterAdditional);
			                    }
			                }
			
			                //sb.Append(this.GenerateIndexes(this));
			
			                if (doSP)
			                {
			                    sb.Append(this.sql_GenerateStoredProcedures());
			                    sb.Append(this.sql_GenerateFunctions());
			                }
			                if (doTrig)
			                {
			                    sb.Append(this.sql_GenerateTriggers(false));
			                }
			
			                sb.Append(GenerateView(this));
			
			                string serverName = null;
			                if (Parent is Package) serverName = ((Package)Parent).GetDatabaseServer();
			
			                string sOut = sb.ToString();
			                try
			                {
			                    sOut = ExecuteScript("Corrections", this, new object[] { sOut }).ToString();
			                }
			                catch { }
			
			                return sOut;
			            }
			            catch(Exception exc)
			            {
			                Logger.LogStatic(exc.Message);
			                throw;
			            }
			
		}
		#endregion
		#region GetDBName
		public System.String GetDBName()
		{
if (Parent is Package)
			{
				return (Parent as Package).GetDBName();
			}
			return "";
		}
		#endregion
		#region GenerateView
		public System.String GenerateView(ICompiledEntity owner)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if ((rel is Inherit)&&(((Inherit)rel).InheritProcedures))
				{
					sb.Append(((Entity)rel.B).GenerateView(owner));
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is View)
				{
					sb.Append(((View)obj).Generate(false, owner));
				}
			}
			return sb.ToString();
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
					System.Collections.IList inherited_triggers = ((Entity)rel.B).GetTriggers();
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
		#region sql_GenerateTriggers
		public System.String sql_GenerateTriggers(System.Boolean dialog)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList m = this.GetTriggers();
			for (int i = 0; i < m.Count; i++)
			{
				sb.Append((m[i] as Trigger).Generate(false, this));
			}
			
			string sOut = sb.ToString();
			//if (dialog)
			//{
			//	Package parent = this.Parent as Package;
			//	if (parent != null)
			//	{
			//		SqlExecForm.Exec(sOut, parent.GetDatabaseServer(), parent.GetDBName());
			//	}
			//	else
			//	{
			//		SqlExecForm.Exec(sOut);
			//	}
			//	return "";
			//}
			return sOut;
		}
		#endregion
		#region sql_GenerateFunctions
		public string sql_GenerateFunctions()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList m = this.GetFunctions();
			for (int i = 0; i < m.Count; i++)
			{
				sb.Append((m[i] as Function).Generate(this));
			}
			
			string sOut = sb.ToString();
			//if (dialog)
			//{
			//	SqlExecForm.Exec(sOut);
			//	return "";
			//}
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
					System.Collections.IList inherited_sps = ((Entity)rel.B).GetStoredProcedures();
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
		#region GetFunctions
		public System.Collections.IList GetFunctions()
		{
System.Collections.IList functions = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if ((rel is Inherit)&&(((Inherit)rel).InheritProcedures))
				{
					System.Collections.IList inherited_functions = ((Entity)rel.B).GetFunctions();
					for (int k = 0; k < inherited_functions.Count; k++)
					{
						functions.Add(inherited_functions[k]);
					}
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Function)
				{
					functions.Add(obj);	
				}
			}
			return functions;
		}
		#endregion
		#region ExecuteScript
		public object ExecuteScript(System.String scriptName, DersaStereotypes.Entity owner, object[] args)
		{
System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Script)
				{
					Script script = (Script)obj;
					if (script.Name == scriptName)
					{
						return script.Execute(owner, args);
					}
				}
			}
			
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					if (((Inherit)rel).InheritScripts)
					{
						object result = ((Entity)rel.B).ExecuteScript(scriptName, owner, args);
						if (result != null) return result;
					}
				}
			}
			return null;
		}
		#endregion
		#region GetPermissions
		public System.String GetPermissions()
		{
string permissions = "";
			ICompiledEntity parent = Parent;
			while(parent != null)
			{
				if (parent is Package)
				{
					permissions = ((Package)parent).Permissions;
					if ((permissions != null)&&(permissions != "")) break;
				}
				parent = parent.Parent;
			}
			if (permissions == "")
			{
				permissions = "all";
			}
			return permissions;
		}
		#endregion
		#region GetRoleNames
		public System.String GetRoleNames()
		{
string roleNames = "";
			ICompiledEntity parent = Parent;
			while(parent != null)
			{
				if (parent is Package)
				{
					roleNames = ((Package)parent).RoleNames;
					if ((roleNames != null)&&(roleNames != "")) break;
				}
				parent = parent.Parent;
			}
			if (roleNames == "")
			{
				roleNames = "public";
			}
			return roleNames;
		}
		#endregion
		#region Register
		public void Register()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			sb.Append("declare @object_type int\n");	
			sb.Append("select @object_type = max(object_type) + 1 from OBJECT_TYPE\n");
			
			string key_name = GetPKName();
			string cSharpName = Static.GetCSharpObjectName(this);
			string database_server = "";
			string log_database_server = "";
			string local_namespace = "";
			string assembly_name = "";
			string applicationServerName = "";
			if (this.Parent is Package)
			{
				database_server = ((Package)this.Parent).GetDatabaseServer();
				log_database_server = ((Package)this.Parent).GetLogDatabaseServer();
				local_namespace = ((Package)this.Parent).Namespace;
				assembly_name = ((Package)this.Parent).AssemblyName;
				if (assembly_name == null || assembly_name == "")
					assembly_name = local_namespace;
				applicationServerName = ((Package)this.Parent).GetApplicationServerName();
			}
			
			string database_name = GetDBName();
			string log_database_name = "";
			string with_log = "0";
			if ((LogDBExtention != null)&&(LogDBExtention != ""))
			{
				log_database_name = database_name + LogDBExtention;
				with_log = "1";
			}
			
			sb.Append("declare @assembly int, @applicationServerName varchar(255) \n");
			sb.Append("select @assembly = assembly from ASSEMBLY (nolock) where name = '" + assembly_name + "' \n");
			sb.Append("if @assembly is null \n");
			sb.Append("\traiserror('Не найдена сборка " + assembly_name + "', 16, -1) \n");
			sb.Append("else begin \n");
			sb.Append("\tselect @applicationServerName = s.object_name\n");
			sb.Append("\tfrom \n");
			sb.Append("\t	ASSEMBLY a (nolock)\n");
			sb.Append("\t	join APP_SERVER_INSTANCE s (nolock)\n");
			sb.Append("\t		on a.app_server_instance = s.app_server_instance\n");
			sb.Append("\twhere\n");
			sb.Append("\t	a.assembly = @assembly\n");
			sb.Append("\tinsert OBJECT_TYPE (object_type, name, class_name, table_name,  view_name, key_name, type_name, interface_type_name, assembly_name, database_server, database_name, max_count, no_caching, application_name, log_database_server, log_database_name, with_log, autoregister, assembly)\n");
			sb.Append("\tvalues (@object_type, '" + this.Name + "','" + this.GetSqlName() + "','" + this.GetSqlName() + "', '" + this.GetViewName() + "', '" + key_name);
			sb.Append("', '" + local_namespace + "." + cSharpName + "'" + ", '" + local_namespace + ".I" + cSharpName + "'");
			sb.Append(", '" + assembly_name + "', '" + database_server + "', '" + database_name + "', " + MaxCount + ", 0, @applicationServerName");
			sb.Append(",'" + log_database_server + "','" + log_database_name + "'," +  with_log + ", 0, @assembly)\n");
			sb.Append("\tset @object_type = @object_type + 1\n");
			sb.Append("end \n");
			
			SqlExecForm.Exec(sb.ToString());
		}
		#endregion
		#region GetViewName
		public System.String GetViewName()
		{
string res; 
			if ((ViewName != null)&&(ViewName != ""))
				return ViewName;
			else 
				return GetSqlName();
			
		}
		#endregion
		#region GetPK
		public System.String GetPK(System.String prefix, System.Boolean show_name, System.Boolean type)
		{
string result  = "";
			Attribute primaryKey = this.GetPrimaryKey();
			if (primaryKey != null) //throw new Exception("Не определен первичный ключ");
			{
				result = prefix;
				if (show_name) result += primaryKey.GetSqlName();
				if (type) result += " " + primaryKey.Type;
			}
			return result;
		}
		#endregion
		#region InheritsFrom
		public System.Boolean InheritsFrom(System.String className)
		{
if (this.GetSqlName() == className)
			{
				return true;
			}
			
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					Entity e = rel.B as Entity;
					if (e != null)
					{
						bool result = e.InheritsFrom(className);
						if (result) return true;
					}
				}
			}
			return false;
		}
		#endregion
		#region GetChildAggregated
		public System.Collections.IList GetChildAggregated(System.String aggregateType)
		{
Map childs = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					Map inCollection = (Map)((Entity)rel.B).GetChildAggregated(aggregateType);
					for (int k = 0; k < inCollection.Count; k++)
					{
						object o = inCollection[k];	
						childs.Add(inCollection.KeyAt(k), o);
					}
				}
				else if (rel is Relation)
				{
					bool isNull = false;
					if ((((Relation)rel).Aggregate == aggregateType)&&((Relation)rel).GenerateForB(out isNull))
					{
						Entity B = ((Relation)rel).B as Entity;
						if ((B != null)&&(B != this))
						{
							childs.Add(rel.B, rel);
						}
					}
				}
			}
			relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Relation)
				{
					bool isNull = false;
					if ((((Relation)rel).Aggregate == aggregateType)&&((Relation)rel).GenerateForA(out isNull))
					{
						Entity A = ((Relation)rel).A as Entity;
						if ((A != null)&&(A != this))
						{
							childs.Add(rel.A, rel);
						}
					}
				}
			}
			return childs;
		}
		#endregion
		#region GetParentAggregated
		public System.Collections.IList GetParentAggregated(System.String aggregateType)
		{
Map childs = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					Map inCollection = (Map)((Entity)rel.B).GetParentAggregated(aggregateType);
					for (int k = 0; k < inCollection.Count; k++)
					{
						object o = inCollection[k];	
						childs.Add(inCollection.KeyAt(k), o);
					}
				}
				else if (rel is Relation)
				{
					bool isNull = false;
					if ((((Relation)rel).Aggregate == aggregateType)&&((Relation)rel).GenerateForA(out isNull))
					{
						Entity B = ((Relation)rel).B as Entity;
						if ((B != null)&&(B != this))
						{
							childs.Add(rel.B, rel);
						}
					}
				}
			}
			relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Relation)
				{
					bool isNull = false;
					if ((((Relation)rel).Aggregate == aggregateType)&&((Relation)rel).GenerateForB(out isNull))
					{
						Entity A = ((Relation)rel).A as Entity;
						if ((A != null)&&(A != this))
						{
							childs.Add(rel.A, rel);
						}
					}
				}
			}
			return childs;
		}
		#endregion
		#region ExecuteScripts
		public object[] ExecuteScripts(System.String[] scriptNames, DersaStereotypes.Entity owner, object[] args)
		{
System.Collections.IList scripts = this.GetScripts(scriptNames);
			object[] a_results = new object[scripts.Count];
			/*for (int i = 0; i < scripts.Count; i++)
			{
				Script script = scripts[i] as Script;
				if (script != null)
				{
					a_results[i] = script.Execute(owner);
				}
				else
				{
					a_results[i] = null;
				}
			}*/
			
			ExecuteObject[] executeObject = new ExecuteObject[scripts.Count];
			for (int i = 0; i < scripts.Count; i++)
			{
				Script script = scripts[i] as Script;
				if (script != null)
				{
					if (script.CompileScript())
					{
						ExecuteObject eso = new ExecuteObject();
						eso.ReturnType = script.ReturnType;
						eso.MethodName = script.Name;
						eso.Text = script.Code;
						eso.Parameters = script.Parameters;
						eso.Args = args[i] as object[];
						executeObject[i] = eso;
					}
					else
					{
						a_results[i] = script.Execute(owner, null);
					}
				}
				else
				{
					a_results[i] = null;
				}
			}
			object[] exec_results = Static.CompileAndExecuteAditionalMethods(owner, executeObject);
			for (int i = 0; i < exec_results.Length; i++)
			{
				if (exec_results[i] != null)
				{
					a_results[i] = exec_results[i];
				}
			}
			
			return a_results;
		}
		#endregion
		#region GetScripts
		public System.Collections.IList GetScripts(System.String[] scriptNames)
		{
System.Collections.Hashtable hashScripts = new System.Collections.Hashtable();
			for (int i = 0; i < scriptNames.Length; i++)
			{
				hashScripts.Add(scriptNames[i], null);
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Script)
				{
					Script script = (Script)obj;
					for (int k = 0; k < scriptNames.Length; k++)
					{
						if (script.Name == scriptNames[k])
						{
							hashScripts[script.Name] = script;
							break;
						}
					}
				}
			}
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if ((rel is Inherit)&&(rel.B is Entity))
				{
					if (((Inherit)rel).InheritScripts)
					{
						System.Collections.Specialized.StringCollection strColl = new System.Collections.Specialized.StringCollection();
						foreach (string key in scriptNames)
						{
							if (hashScripts[key] == null)
							{
								strColl.Add(key);
							}
						}
						if (strColl.Count > 0)
						{
							string[] newScriptNames = new string[strColl.Count];
							strColl.CopyTo(newScriptNames, 0);
							
							System.Collections.IList scripts = ((Entity)rel.B).GetScripts(newScriptNames);
							for (int k = 0; k < scripts.Count; k++)
							{
								Script script = scripts[k] as Script;
								if (script != null)
								{
									if (hashScripts[script.Name] == null)
									{
										hashScripts[script.Name] = script;
									}
								}
							}
						}
					}
				}
			}
			System.Collections.ArrayList resultList = new System.Collections.ArrayList();
			for (int i = 0; i < scriptNames.Length; i++)
			{
				resultList.Add(hashScripts[scriptNames[i]]);
			}
			return resultList;
		}
		#endregion
		#region GetPrimaryKey
		public Attribute GetPrimaryKey()
		{
            if (this.Abstract) return null;
			            Attribute keyAttribute = null;
			            object primaryKey = this.ExecuteScript("GenerateAlternativePrimaryKey", this, new object[0]);
			            if (primaryKey != null)
			            {
			                keyAttribute = primaryKey as Attribute;
			            }
			            else
			            {
			                System.Collections.IList relations = this.ARelations;
			                for (int i = 0; i < relations.Count; i++)
			                {
			                    ICompiledRelation rel = (ICompiledRelation)relations[i];
			                    if ((rel is Inherit) && (((Inherit)rel).InheritPK))
			                    {
			                        Attribute pkAttribute = ((Entity)rel.B).GetPrimaryKey();
			                        if (pkAttribute != null) keyAttribute = pkAttribute;
			                    }
			                }
			                if (keyAttribute == null)
			                {
			                    System.Collections.IList children = this.Children;
			                    for (int i = 0; i < children.Count; i++)
			                    {
			                        ICompiledEntity obj = (ICompiledEntity)children[i];
			                        if (obj is Key)
			                        {
			                            Key key = obj as Key;
			                            Attribute attr = new Attribute(null);
			                            attr.Name = key.Name;
			                            attr.PhisicalName = key.PhisicalName;
			                            attr.Type = key.Type;
			                            attr.Null = key.Null;
			                            attr.Default = key.Default;
			                            attr.Description = key.Name + " PRIMARY KEY " + key.Description;
			                            keyAttribute = attr;
			                        }
			                    }
			                    if (keyAttribute == null)
			                    {
			                        if (this.MakePK)
			                        {
			                            Attribute attr = new Attribute(null);
			                            attr.Name = "#";
			                            attr.PhisicalName = this.GetSqlName().ToLower();
			                            attr.Type = this.KeyType;
			                            attr.Null = "not null";
			                            attr.Default = "";
			                            attr.Description = "PRIMARY KEY";
			                            attr.ReadOnly = false;
			                            attr.Get = "";
			                            attr.Set = "";
			                            attr.UseInWebForm = false;
			                            keyAttribute = attr;
			                        }
			                    }
			                }
			            }
			
			            if (keyAttribute != null)
			            {
			                keyAttribute.Set = "\t\t\t\tthis.SetId(value);";
			            }
			            return keyAttribute;
		}
		#endregion
		#region GeneratePropertyEntities
		public System.String GeneratePropertyEntities()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList pes = this.GetPropertyEntities();
			if (pes.Count > 0)
			{
				sb.Append("\t\t#region override Indexer\n");
				sb.Append("\t\tprotected override object this[string property]\n");
				sb.Append("\t\t{\n");
				sb.Append("\t\t\tget\n");
				sb.Append("\t\t\t{\n");
				sb.Append("\t\t\t\t//проанализировать префикс\n");
				for (int i = 0; i < pes.Count; i++)
				{
					Entity e = (Entity)pes[i];
					sb.Append(e.ExecuteScript("GenerateIndexerGet", e, new object[]{this}) as string);
				}
				sb.Append("\t\t\t\treturn base[property];\n");
				sb.Append("\t\t\t}\n");
				sb.Append("\t\t\tset\n");
				sb.Append("\t\t\t{\n");
				sb.Append("\t\t\t\t//проанализировать префикс\n");
				for (int i = 0; i < pes.Count; i++)
				{
					Entity e = (Entity)pes[i];
					sb.Append(e.ExecuteScript("GenerateIndexerSet", e, new object[]{this}) as string);
				}
				sb.Append("\t\t\t\tbase[property] = value;\n");
				sb.Append("\t\t\t}\n");
				sb.Append("\t\t}\n");
				sb.Append("\t\t#endregion\n");
			}
			return sb.ToString();
		}
		#endregion
		#region GetInterfaces
		public System.Collections.IList GetInterfaces()
		{
Map inters = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if ((rel is Inherit)&&((rel as Inherit).InheritInterfaces))
				{
					System.Collections.IList inDictionary = ((Entity)rel.B).GetInterfaces();
					foreach (Interface inter in inDictionary)
					{
						inters.Add(inter.GetPhisicalName(), inter);
					}
				}
				else if (rel is Implement)
				{
					Interface inter = rel.B as Interface;
					if (inter != null)
					{
						inters.Add(inter.GetPhisicalName(), inter);
					}
				}
			}
			return inters;
		}
		#endregion
		#region GetConstants
		public System.Collections.IList GetConstants()
		{
Map constants = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					System.Collections.IList inherited_constants = ((Entity)rel.B).GetConstants();
					for (int k = 0; k < inherited_constants.Count; k++)
					{
						constants.Add(((Const)inherited_constants[k]).Name, inherited_constants[k]);
					}
				}
			}
			System.Collections.IList pes = GetPropertyEntities();
			for (int i = 0; i < pes.Count; i++)
			{
				Entity e = (Entity)pes[i];
				System.Collections.IList pe_constants = e.GetConstants();
				foreach (Const c in pe_constants)
				{
					constants.Add(c.Name, c);
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if (obj is Const)
				{
					constants.Add(obj.Name, obj);	
				}
			}
			return constants;
		}
		#endregion
		#region GetPropertyEntities
		public System.Collections.IList GetPropertyEntities()
		{
System.Collections.IList propertyEntities = new System.Collections.ArrayList();
			System.Collections.IList aRelations = this.ARelations;
			for (int i = 0; i < aRelations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)aRelations[i];
				if (rel is Inherit)
				{
					if (rel.B.Name == "PROPERTY_TYPE")
					{
						return propertyEntities;
					}
				}
			}
			System.Collections.IList relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Relation)
				{
					System.Collections.IList inheritRelations = rel.A.ARelations;
					for (int j = 0; j < inheritRelations.Count; j++)
					{
						ICompiledRelation inheritRel = (ICompiledRelation)inheritRelations[j];
						if (inheritRel is Inherit)
						{
							if (inheritRel.B.Name == "PROPERTY_VALUE")
							{
								propertyEntities.Add(rel.A);
							}
						}
					}
				}
			}
			return propertyEntities;
		}
		#endregion
		#region GenerateConstants
		public System.String GenerateConstants()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.IList constants = this.GetConstants();
			for (int i = 0; i < constants.Count; i++)
			{
				sb.Append((constants[i] as Const).Generate());
			}
			return sb.ToString();
		}
		#endregion
		#region GetFKIndexLinks
		public System.Collections.IList GetFKIndexLinks()
		{
Map links = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					Inherit inh = rel as Inherit;
					Map inCollection = (Map)((Entity)rel.B).GetFKIndexLinks();
					for (int k = 0; k < inCollection.Count; k++)
					{
						object o = inCollection[k];	
						string key = inCollection.KeyAt(k) as string;
						if (inh.Prefix != null)
							key = inh.Prefix + key;
						links.Add(key, o);
					}
				}
				else if (rel is Relation)
				{
					bool isNull = false;
					if ((((Relation)rel).MakeIndex)&&((Relation)rel).GenerateForA(out isNull))
					{
						Entity B = ((Relation)rel).B as Entity;
						if (B != null)
						{
							string bRole = ((Relation)rel).BRole;
							if ((bRole == null)||(bRole == "")) bRole = B.GetPKName();
							links.Add(bRole, rel.B);
						}
					}
				}
			}
			relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Relation)
				{
					bool isNull = false;
					if ((((Relation)rel).MakeIndex)&&((Relation)rel).GenerateForB(out isNull))
					{
						Entity A = ((Relation)rel).A as Entity;
						if (A != null)
						{
							string aRole = ((Relation)rel).ARole;
							if ((aRole == null)||(aRole == "")) aRole = A.GetPKName();
							links.Add(aRole, rel.A);
						}
					}
				}
			}
			return links;
		}
		#endregion
		#region GetViewAttributes
		public System.Collections.IList GetViewAttributes()
		{
Map attrs = new Map();
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit && ((Inherit)rel).InheritViewAttributes)
				{
					System.Collections.IList inDictionary = ((Entity)rel.B).GetViewAttributes();
					foreach (ViewAttribute a in inDictionary)
					{
						attrs.Add(a.GetSqlName(), a);
					}
				}
				if (rel is Relation)
				{
					Relation relation = (Relation)rel;
					bool isNull = false;
					if (relation.GenerateForA(out isNull) && relation.B is Type && !((Type)relation.B).GenerateTable)
					{
						Type localType = (Type)relation.B;
						string typeSqlName = localType.Name.ToLower();
						if ((relation.BRole != null)&&(relation.BRole != ""))
						{
							typeSqlName = relation.BRole;
						}
						else 
						{
							string typePhisicalName = localType.PhisicalName.ToLower();
							if ((typePhisicalName != null)&&(typePhisicalName != ""))
							{
								typeSqlName = typePhisicalName;
							}
						}
						string csTypeName = Static.GetCSharpObjectName(localType);
						string csType = Static.GetCSharpNativeType(localType.SqlType);
						string csPropertyName = Static.GetCSharpName(typeSqlName);
						
						ViewAttribute view = new ViewAttribute(null);
						view.Name = localType.Name.ToLower();
						if ((relation.BComment != null)&&(relation.BComment != ""))
						{
							view.Name = relation.BComment;
						}
						view.CreatePrivateField = false;
						view.PhisicalName = typeSqlName + "_name";
						view.Type = "varchar(128)";
			//			view.Get = "\t\t\t\treturn this." + csPropertyName + ".ToString();";
						view.Get = "\t\t\t\tif(_" + typeSqlName + ".IsNull) return string.Empty;\n\t\t\t\treturn Enum.GetName(typeof(" + csTypeName + "), " + typeSqlName + ".Value);";
						view.Set = "";
						view.DIOSDefault = "";
						view.DIOSType = "";
						attrs.Add(view.GetSqlName(), view);
					}
					if(relation.BStringAttributes != string.Empty && relation.BStringAttributes != null)
					{
						string[] AttrNames = relation.BStringAttributes.Split(',');
						for(int n = 0; n < AttrNames.Length; n++)
						{
							string AttrName = AttrNames[n].Trim();
							string name = relation.GetNameForA() + "_" + AttrName;
							ViewAttribute v = new ViewAttribute(null);
							v.Name = relation.GetDescriptionForA() + "(" + AttrName + ")";
							v.CreatePrivateField = true;
							v.PhisicalName = name;
							v.Type = "varchar(256)";
							System.Text.StringBuilder SB_Get = new System.Text.StringBuilder();
							SB_Get.Append("\t\t\t\tif(_");
							SB_Get.Append(name);
							SB_Get.Append(".IsNull)");
							SB_Get.Append("\n\t\t\t\t{");
							SB_Get.Append("\n\t\t\t\t\t");
							SB_Get.Append("IObject relObj = this.");
							SB_Get.Append(relation.GetRefName());
							SB_Get.Append(";");
							SB_Get.Append("\n\t\t\t\t\t");
							SB_Get.Append("if(relObj != null)");
							SB_Get.Append("\n\t\t\t\t\t{");
							SB_Get.Append("\n\t\t\t\t\t\t_");
							SB_Get.Append(name);
							SB_Get.Append(" = ");
							SB_Get.Append("(SqlString)relObj[\"");
							SB_Get.Append(AttrName);
							SB_Get.Append("\"]");
							SB_Get.Append(";");
							SB_Get.Append("\n\t\t\t\t\t}");
							SB_Get.Append("\n\t\t\t\t}");
							SB_Get.Append("\n\t\t\t\treturn _");
							SB_Get.Append(name);
							SB_Get.Append(";");
							v.Get =  SB_Get.ToString();
							v.Set = "";
							v.DIOSDefault = "";
							v.DIOSType = "";
							attrs.Add(name, v);
						}
					}
				}
			}
			relations = this.BRelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Relation)
				{
					Relation relation = (Relation)rel;
					bool isNull = false;
					if (relation.GenerateForB(out isNull) && relation.A is Type && !((Type)relation.A).GenerateTable)
					{
						Type localType = (Type)relation.A;
						string typeSqlName = localType.Name.ToLower();
						if ((relation.ARole != null)&&(relation.ARole != ""))
						{
							typeSqlName = relation.ARole;
						}
						else 
						{
							string typePhisicalName = localType.PhisicalName.ToLower();
							if ((typePhisicalName != null)&&(typePhisicalName != ""))
							{
								typeSqlName = typePhisicalName;
							}
						}
						string csTypeName = Static.GetCSharpObjectName(localType);
						string csType = Static.GetCSharpNativeType(localType.SqlType);
						string csPropertyName = Static.GetCSharpName(typeSqlName);
						
						ViewAttribute view = new ViewAttribute(null);
						view.Name = localType.Name.ToLower();
						if ((relation.AComment != null)&&(relation.AComment != ""))
						{
							view.Name = relation.AComment;
						}
						view.CreatePrivateField = false;
						view.PhisicalName = typeSqlName + "_name";
						view.Type = "varchar(128)";
			//			view.Get = "\t\t\t\treturn this." + csPropertyName + ".ToString();";
						view.Get = "\t\t\t\tif(_" + typeSqlName + ".IsNull) return string.Empty;\n\t\t\t\treturn Enum.GetName(typeof(" + csTypeName + "), " + typeSqlName + ".Value);";
						view.Set = "";
						view.DIOSDefault = "";
						view.DIOSType = "";
						attrs.Add(view.GetSqlName(), view);
					}
				}
			}
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				object obj = children[i];
				if (obj is ViewAttribute)
				{
					attrs.Add(((ViewAttribute)obj).GetSqlName(), obj);
				}
				else if (obj is Type)
				{
					Type localType = (Type)obj;
					if (localType.GenerateTable) continue;
					string typeSqlName = localType.Name.ToLower();
					string typePhisicalName = localType.PhisicalName.ToLower();
					if ((typePhisicalName != null)&&(typePhisicalName != ""))
					{
						typeSqlName = typePhisicalName;
					}
					string csTypeName = Static.GetCSharpObjectName(localType);
					string csType = Static.GetCSharpNativeType(localType.SqlType);
					string csPropertyName = Static.GetCSharpName(typeSqlName);
					
					
					ViewAttribute view = new ViewAttribute(null);
					view.Name = localType.Name.ToLower();
					view.CreatePrivateField = false;
					view.PhisicalName = typeSqlName + "_name";
					view.Type = "varchar(128)";
					view.Get = "\t\t\t\tif(_" + typeSqlName + ".IsNull) return string.Empty;\n\t\t\t\treturn Enum.GetName(typeof(" + csTypeName + "), " + typeSqlName + ".Value);";
					view.Set = "";
					view.DIOSDefault = "";
					view.DIOSType = "";
					attrs.Add(view.GetSqlName(), view);
				}
				else if (obj is StateEngine)
				{
					StateEngine local_type = obj as StateEngine;
					ViewAttribute view = new ViewAttribute(null);
					view.Name = local_type.Name.ToLower();
					view.CreatePrivateField = false;
					view.PhisicalName = local_type.PhisicalName.ToLower() + "_name";
					view.Type = "varchar(128)";
					
					string csTypeName = Static.GetCSharpObjectName(local_type);
					string typePhisicalName = local_type.PhisicalName.ToLower();
					view.Get = "\t\t\t\tif(_" + typePhisicalName + ".IsNull) return string.Empty;\n\t\t\t\treturn Enum.GetName(typeof(" + csTypeName + "), " + typePhisicalName + ".Value);";
					view.Set = "";
					view.DIOSDefault = "";
					view.DIOSType = "";
					attrs.Add(view.GetSqlName(), view);
				}
			}
			return attrs;
		}
		#endregion
		#region GenerateWebObject
		public string GenerateWebObject()
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			            System.Text.StringBuilder sbInterface = new System.Text.StringBuilder();
			            System.Text.StringBuilder sbNamespace = new System.Text.StringBuilder();
			
			            Package package = this.Parent as Package;
			            if (package == null) return "Не найдена папка";
			
			            string phisicalName = this.GetSqlName();
			            string pkName = this.GetPKName();
			            string interfaces = this.Interfaces;
			            System.Collections.IList inters = this.GetInterfaces();
			            string comaInter = ", ";
			            foreach (Interface inter in inters)
			            {
			                interfaces += comaInter + inter.GetFullName();
			                comaInter = ", ";
			            }
			            string cSharpName = Static.GetCSharpObjectName(this);
			            string globalName = package.GetGlobalName();
			            bool isAbstract = this.Abstract;
			
			            System.Collections.IList attrs = this.GetCSharpAttributes(false);
			            System.Collections.IList wibsAttrs = this.GetCSharpAttributes(true);
			
			
			            string defUsing = "using System;\r\nusing "
			                + globalName
			                + ".BusinessBase;\r\nusing "
			                + globalName
			                + ".Common;\r\nusing "
			                + globalName
			                + ".Common.Interfaces;\r\nusing "
			                + globalName
			                + ".ObjectLib;\r\nusing Newtonsoft.Json;\r\nusing System.Runtime.Serialization;\r\n";
			            defUsing += this.GetUsing();
			            sbNamespace.Append(package.GetUsing(defUsing));
			
			            sbNamespace.Append("\r\n");
			            sbNamespace.Append("namespace " + package.Namespace + "\r\n{\r\n");
			            sbNamespace.Append(this.GenerateEnums());
			            sb.Append("\t[LocalizedName(\"" + this.Name + "\")]\r\n");
			            sb.Append("\t[DataContract]\r\n");
			            sb.Append("\tpublic ");
			
			            sb.Append("class " + cSharpName + ": " + globalName + ".ObjectLib.Object");
			            if (interfaces != null && interfaces != "")
			            {
			                sb.Append(", ");
			                sb.Append(interfaces);
			            }
			
			            sb.Append("\r\n\t{\r\n");
			            sb.Append("\r\n");
			            //sb.Append(this.AdditionalClasses);
			            //sb.Append("\r\n\r\n");
			
			            if (this.Comment != null && this.Comment != string.Empty)
			            {
			                sb.Append(this.Comment);
			            }
			
			            sbInterface.Append("\t[ClassKeyName(\"" + phisicalName + "\",\"" + pkName + "\")]\r\n");
			            sbInterface.Append("\tpublic interface I" + cSharpName + ": " + globalName + ".Common.Interfaces.IObject" + interfaces + "\r\n");
			            sbInterface.Append("\t{\r\n");
			
			            //Console.WriteLine("формируем конструктор");
			            sb.Append("\t\tpublic " + cSharpName + "():base(){}\r\n\r\n");
			            sb.Append("\t\tpublic " + cSharpName + "(UniStructView v, ObjectFactory f) : base(v, f) { }\r\n\r\n");
			
			            //sb.Append("\t\t#region Implementations\r\n");
			            sb.Append("\t\tpublic const string EntityClassName = \"" + phisicalName + "\";\r\n");
			            //sb.Append("\t\tprotected abstract I" + cSharpName + " AsInterface{get;}\r\n");
			
			            System.Collections.IList methods = this.GetMethods();
			
			            bool hasGenerateInitObjectPropertiesMethod = false;
			            foreach (Method m in methods)
			            {
			                if (m.Name == "InitObjectProperties")
			                {
			                    hasGenerateInitObjectPropertiesMethod = true;
			                }
			            }
			            //Console.WriteLine("формируем свойства");
			            System.Text.StringBuilder sbProperties = new System.Text.StringBuilder();
			            string coma = "";
			            foreach (Attribute attr in attrs)
			            {
			                if (attr.SqlOnly)
			                    continue;
			                string attr_name = attr.Name;
			                string attr_type = attr.Type.Trim().ToLower()
			                .Replace("identity", "")
			                .Replace("number(8)", "int")
			                .Replace("varchar2", "varchar")
			                .Replace("number", "numeric")
			                    ;
			                attr.Type = attr_type;
			                string attr_phisical_name = attr.GetSqlName();
			                string attr_null = attr.Null;
			                string attr_defcs = attr.DIOSDefault; if (attr_defcs == null) attr_defcs = "";
			                string attr_csharp_type = attr.GetCSharpType();
			                string attr_wibs_type = attr.GetDIOSType();
			                bool attr_readonly = attr.ReadOnly;
			
			                sbProperties.Append(coma + attr_phisical_name);
			                coma = ", ";
			
			                sb.Append("\t\t#region " + attr_phisical_name + "\r\n");
			                if (!isAbstract)
			                {
			                    sb.Append("\t\tprotected " + attr_csharp_type + " _" + attr_phisical_name);
			                }
			                if (attr_defcs.Length > 0)
			                {
			                    sb.Append(" = " + attr_defcs);
			                }
			                if (!isAbstract)
			                {
			                    sb.Append(";\r\n");
			                }
			                int maxLength = 0;
			                if (attr_csharp_type == "SqlString")
			                {
			                    string sqlType = attr.Type.Trim().ToLower()
			                .Replace("identity", "")
			                .Replace("number(8)", "int")
			                .Replace("varchar2", "varchar")
			                .Replace("number", "numeric")
			                    ;
			                    if (sqlType == "sysname") maxLength = 128;
			                    if (sqlType.Trim().ToLower() != "varchar(max)" && sqlType.Trim().ToLower() != "nvarchar(max)")
			                    {
			                        if ((sqlType.Trim().ToLower().StartsWith("varchar")) || (sqlType.Trim().ToLower().StartsWith("nvarchar")))
			                        {
			                            string lenStr = sqlType.Trim(new char[] { ' ', '(', ')', 'v', 'a', 'r', 'c', 'h', 'n' });
			                            try
			                            {
			                                maxLength = Int32.Parse(lenStr);
			                            }
			                            catch
			                            {
			                                throw new Exception("Ошибка в типе. Не удалось определить длину поля " + attr.Name);
			                            }
			                        }
			                    }
			                }
			                //if(attr.Description != "PRIMARY KEY")
			                //{
			                sb.Append("\t\t[DataMember]\r\n");
			                //}
			                if (attr.EnumTypeName != null && attr.EnumTypeName != "")
			                {
			                    attr.UseInWebForm = false;
			                    sb.Append("\t\t[EnumPropertyAttribute(typeof(" + attr.EnumTypeName + "))]\r\n");
			                }
			                string objectPropertyAttribute = "\t\t[ObjectPropertyAttribute(\"" + attr_name + "\"";
			                if (attr_null == "not null")
			                {
			                    objectPropertyAttribute += ", true";
			                }
			                else
			                {
			                    objectPropertyAttribute += ", false";
			                }
			                //objectPropertyAttribute += ", false, " + maxLength.ToString() + ")]\r\n";
			                objectPropertyAttribute += ", false, " + maxLength.ToString();
			                if (attr.Description == "PRIMARY KEY")
			                    objectPropertyAttribute += ", " + (!this.HasIdentityKey).ToString().ToLower();
			                else
			                    objectPropertyAttribute += ", " + attr.UseInWebForm.ToString().ToLower();
			                objectPropertyAttribute += ", ";
			                objectPropertyAttribute += (attr.Description == "PRIMARY KEY" ? "true" : "false");
			                objectPropertyAttribute += ")]\r\n";
			                sb.Append(objectPropertyAttribute);
			                sbInterface.Append(objectPropertyAttribute);
			
			                if (isAbstract)
			                {
			                    sb.Append("\t\tpublic abstract " + attr_wibs_type + " " + attr_phisical_name + "{");
			                }
			                else
			                {
			                    sb.Append("\t\tpublic " + attr_wibs_type + " " + attr_phisical_name + "\r\n\t\t{");
			                }
			                if (isAbstract)
			                {
			                    sb.Append("get;set;}\r\n");
			                }
			                else
			                {
			                    string attr_get = attr.Get;
			                    if ((attr_get == null) || (attr_get == ""))
			                    {
			                        if (attr_csharp_type != attr_wibs_type)
			                        {
			                            attr_get = "\t\t\t\treturn (" + attr_wibs_type + ")(int)" + " _" + attr_phisical_name + ";";
			                        }
			                        else
			                        {
			                            attr_get = "\t\t\t\treturn _" + attr_phisical_name + ";";
			                        }
			                    }
			                    string attr_set = attr.Set;
			                    if (!attr_readonly)
			                    {
			                        if ((attr_set == null) || (attr_set == "") || (attr_phisical_name == pkName))
			                        {
			                            attr_set = "";
			                            if (attr_csharp_type != attr_wibs_type)
			                            {
			                                /*attr_set += "\t\t\t\t_" + attr_phisical_name + " = (" + attr_wibs_type + ")value;"; */
			                                attr_set += "\t\t\t\t_" + attr_phisical_name + " = (int)value;";
			                            }
			                            else
			                            {
			                                attr_set = "\t\t\t\tif (!this.changedFields.Contains(\"_"
			                                    + attr_phisical_name
			                                    + "\") && this._"
			                                    + attr_phisical_name
			                                    + " != value)\r\n\t\t\t\t\tthis.changedFields.Add(\"_"
			                                    + attr_phisical_name
			                                    + "\", this._"
			                                    + attr_phisical_name + ");\r\n";
			                                attr_set += "\t\t\t\t_" + attr_phisical_name + " = value;";
			                            }
			                        }
			                    }
			                    sb.Append("\r\n");
			                    sb.Append("\t\t\tget\r\n\t\t\t{\r\n");
			                    sb.Append(attr_get);
			                    sb.Append("\r\n\t\t\t}\r\n");
			                    if (!attr_readonly)
			                    {
			                        sb.Append("\t\t\tset\r\n\t\t\t{\r\n");
			                        sb.Append(attr_set);
			                        sb.Append("\r\n\t\t\t}\r\n");
			                    }
			                    sb.Append("\t\t}\r\n");
			                }
			                sb.Append("\t\t#endregion\r\n");
			
			                sbInterface.Append("\t\t" + attr_wibs_type + " " + attr_phisical_name + "{get;");
			                if (!attr_readonly)
			                {
			                    sbInterface.Append("set;");
			                }
			                sbInterface.Append("}\r\n");
			            }
			
			            //Console.WriteLine("Дополняем ViewAttribute");
			            System.Collections.IList children = this.GetViewAttributes();
			            for (int i = 0; i < children.Count; i++)
			            {
			                ViewAttribute obj = children[i] as ViewAttribute;
			                if (obj == null) continue;
			                string attr_name = obj.Name;
			                string attr_type = obj.Type;
			                string attr_phisical_name = obj.GetSqlName();
			                string attr_default_csharp = obj.DIOSDefault;
			                string attr_csharp_type = obj.GetCSharpType();
			                string attr_get = obj.Get; if (attr_get == null) attr_get = "";
			                string attr_set = obj.Set; if (attr_set == null) attr_set = "";
			
			                sb.Append("\t\t#region " + attr_phisical_name + "\r\n");
			                if (obj.CreatePrivateField)
			                {
			                    sb.Append("\t\tprotected " + attr_csharp_type + " _" + attr_phisical_name);
			                    if ((attr_default_csharp != null) && (attr_default_csharp.Length > 0))
			                    {
			                        sb.Append(" = " + attr_default_csharp);
			                    }
			                    sb.Append(";\r\n");
			                }
			                sb.Append("\t\t[ViewProperty]\r\n");
			                sb.Append("\t\t[DataMember]\r\n");
			                sb.Append("\t\t[ObjectPropertyAttribute(\"" + attr_name + "\", false, " + (!(attr_set.Length > 0)).ToString().ToLower() + ")]\r\n");
			                sbInterface.Append("\t\t[ObjectPropertyAttribute(\"" + attr_name + "\", false, " + (!(attr_set.Length > 0)).ToString().ToLower() + ")]\r\n");
			
			                sb.Append("\t\tpublic " + attr_csharp_type + " " + attr_phisical_name + "\r\n\t\t{");
			                sbInterface.Append("\t\t" + attr_csharp_type + " " + attr_phisical_name + "{get;");
			                if (attr_set.Length > 0)
			                {
			                    sbInterface.Append("set;");
			                }
			                sbInterface.Append("}\r\n");
			
			                if (attr_get == "")
			                {
			                    attr_get = "\t\t\t\treturn _" + attr_phisical_name + ";";
			                }
			                sb.Append("\r\n\t\t\tget\r\n\t\t\t{\r\n");
			                sb.Append(attr_get);
			                sb.Append("\r\n\t\t\t}\r\n");
			                if (attr_set.Length > 0)
			                {
			                    sb.Append("\t\t\tset\r\n\t\t\t{\r\n");
			                    sb.Append(attr_set);
			                    sb.Append("\r\n\t\t\t}\r\n");
			                }
			                sb.Append("\t\t}\r\n");
			
			
			                sb.Append("\t\t#endregion\r\n");
			                sbProperties.Append(", " + attr_phisical_name);
			            }
			
			            //Console.WriteLine("Формирование Constants");
			            sb.Append("\t\t#region Константы\r\n");
			            sb.Append(this.GenerateConstants());
			            sb.Append("\t\t#endregion\r\n");
			
			            //Console.WriteLine("Формирование RefObjects");
			            if (!this.NoRefObjects)
			            {
			                sb.Append("\t\t#region RefObjects\r\n");
			                sb.Append(this.GenerateRef("GenerateRefObjs", ""));
			                sb.Append("\t\t#endregion\r\n");
			                sbInterface.Append(this.GenerateRef("GenerateInterfaceRefObjs", ""));
			            }
			            //Console.WriteLine("Формирование методов");
			            sb.Append("\t\t#region Методы\r\n");
			            sb.Append(this.GenerateMethods());
			            sb.Append("\t\t#endregion\r\n");
			            sbInterface.Append("\t\t#region Методы\r\n");
			            sbInterface.Append(this.GenerateInterfaceMethods());
			            sbInterface.Append("\t\t#endregion\r\n");
			
			            sb.Append("\t\t#region GetUniView()\r\n");
			            sb.Append(this.GenerateGetUniView(wibsAttrs));
			            sb.Append("\t\t#endregion\r\n");
			
			            //Console.WriteLine("Формирование Refs");
			            sb.Append("\t\t#region Refs\r\n");
			            sb.Append(this.GenerateRefs());
			            sb.Append("\t\t#endregion\r\n");
			            sbInterface.Append("\t\t#region Refs\r\n");
			            sbInterface.Append(this.GenerateInterfaceRefs());
			            sbInterface.Append("\t\t#endregion\r\n");
			
			            //Console.WriteLine("Формирование Properties");
			            sb.Append("\t\t#region Properties\r\n");
			            sb.Append(this.GenerateProperties());
			            sb.Append("\t\t#endregion\r\n");
			            sbInterface.Append("\t\t#region Properties\r\n");
			            sbInterface.Append(this.GenerateInterfaceProperties());
			            sbInterface.Append("\t\t#endregion\r\n");
			
			            //Console.WriteLine("Формирование Конечных автоматов");
			            sb.Append(this.GenerateStateEngines());
			            sbInterface.Append("\t\t#region State Engines\r\n");
			            sbInterface.Append(this.GenerateInterfaceStateEngines());
			            sbInterface.Append("\t\t#endregion\r\n");
			
			            //Console.WriteLine("Формирование Расширений");
			            sb.Append(this.GeneratePropertyEntities());
			
			            sb.Append("\t}\r\n");
			
			            sbInterface.Append("\t}\r\n");
			
			            string fileName = package.GetDirectory() + "\\" + cSharpName + ".cs";
			            string result = sbNamespace.ToString() + sb.ToString() + "}";
			            return result;
		}
		#endregion
		#region RegisterForWeb
		public string RegisterForWeb()
		{
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			            sb.Append("declare @object_type int\n");
			            sb.Append("select @object_type = isnull(max(object_type), 0) + 1 from OBJECT_TYPE\n\n");
			
			            string key_name = GetPKName();
			            string cSharpName = Static.GetCSharpObjectName(this);
			            string database_server = "";
			            //string log_database_server = "";
			            string local_namespace = "";
			            if (this.Parent is Package)
			            {
			                database_server = ((Package)this.Parent).GetDatabaseServer();
			                //	log_database_server = ((Package)this.Parent).GetLogDatabaseServer();
			                local_namespace = ((Package)this.Parent).Namespace;
			            }
			
			            string database_name = GetDBName();
			            //string log_database_name = "";
			            //string with_log = "0";
			            //if ((LogDBExtention != null)&&(LogDBExtention != ""))
			            //{
			            //	log_database_name = database_name + LogDBExtention;
			            //	with_log = "1";
			            //}
			
			            sb.Append("\tinsert into OBJECT_TYPE (object_type, name, class_name, table_name,  key_name, type_name, assembly_name, database_name, records_limit)\n");
			            sb.Append("\tvalues (@object_type, '" + this.Name + "','" + this.GetSqlName() + "','" + this.GetSqlName() + "', '" + key_name);
			            sb.Append("', '" + local_namespace + "." + cSharpName + "'" + ", '" + local_namespace + "'");
			            sb.Append(", '" + database_name + "', -1)\n");
			            sb.Append("\tset @object_type = @object_type + 1\n");
			
			
			            SqlExecForm.Exec(sb.ToString());
			            Console.WriteLine(sb.ToString());
			
			            return sb.ToString();
			
		}
		#endregion
		#region ora_Generate
		public string ora_Generate()
		{
            string comma = "";
			            System.Text.StringBuilder sb = new System.Text.StringBuilder();
			            string sqlName = this.GetSqlName();
			
			            sb.Append("declare\r\n");
			            sb.Append("\tex int;\r\n");
			            sb.Append("\tquery_text varchar(1000);\r\n");
			            sb.Append("begin\r\n");
			            sb.Append("\tselect count(*) into ex from user_tables where table_name = '" + sqlName + "';\r\n");
			            sb.Append("\tif ex > 0 then\r\n");
			            sb.Append("\t\tquery_text := 'drop table " + sqlName + "';\r\n");
			            sb.Append("\t\texecute immediate query_text;\r\n");
			            sb.Append("\tend if;\r\n");
			            sb.Append("end;\r\n");
			            sb.Append("go\r\n");
			
			            sb.Append("create table " + sqlName + "\n(");
			            System.Collections.IList attrs = this.GetAttributes();
			            foreach (Attribute attr in attrs)
			            {
			                sb.Append(comma + "\n\t" + attr.GetSqlName() + " ");
			                string AttrType = attr.Type
			                    .Replace("bigint", "NUMBER(20)")
			                    .Replace("int", "NUMBER(9)")
			                    .Replace("bit", "NUMBER(1)")
			                    .Replace("identity", "")
			                    .Replace("datetime", "date")
			                    .Trim();
			                sb.Append(AttrType + " ");
			                sb.Append(attr.Null);
			                string attr_default = attr.Default;
			                //                    if ((attr_default != null) && (attr_default.Length > 0))
			                //                    {
			                //                        sb.Append(" default " + attr_default);
			                //                    }
			                comma = ",";
			            }
			            System.Collections.IList pk_attrs = this.GetPKAttributes();
			            if ((pk_attrs != null) && (pk_attrs.Count > 0))
			            {
			                string cm = "";
			                sb.Append(comma + "\n\t" + "primary key (");
			                for (int i = 0; i < pk_attrs.Count; i++)
			                {
			                    Attribute attr = (Attribute)pk_attrs[i];
			                    sb.Append(cm + attr.GetSqlName());
			                    cm = ", ";
			                }
			                sb.Append(")");
			            }
			            Map fk_attrs = this.GetFKLinks() as Map;
			            if ((fk_attrs != null) && (fk_attrs.Count > 0))
			            {
			                for (int i = 0; i < fk_attrs.Count; i++)
			                {
			                    Entity link = fk_attrs[i] as Entity;
			                    if (link != null)
			                    {
			                        sb.Append(comma + "\n\t" + "foreign key (" + fk_attrs.KeyAt(i) + ") references " + link.GetSqlName());
			                    }
			                }
			            }
			
			            sb.Append("\r\n)");
			
			            sb.Append("\r\ngo\n");
			            //sb.Append("grant " + GetPermissions() + " on " + sqlName + " to " + GetRoleNames() + "\n");
			            //sb.Append("/\n");
			
			            sb.Append(this.GenerateIndexes(this));
			
			            object insertData = this.ExecuteScript("InsertData", this, new object[0]);
			            if (insertData != null)
			            {
			                sb.Append((string)insertData);
			            }
			
			            object generateAdditional = this.ExecuteScript("GenerateAdditional", this, new object[] { false });
			            if (generateAdditional != null)
			            {
			                sb.Append((string)generateAdditional);
			            }
			            else
			            {
			                sb.Append("/* No Additional Script */\n");
			            }
			
			            return sb.ToString()
			                         .Replace("go\n","go\r\n")
			                         .Replace("go\r\n", "/\r\n");;
			
		}
		#endregion
		#endregion
	}
}
