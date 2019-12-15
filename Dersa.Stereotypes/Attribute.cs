using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Attribute: StereotypeBaseE, ICompiledEntity
	{
		public Attribute(){}

		public Attribute(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String PhisicalName = "";
		public System.String Type = "int";
		public System.String Null = "";
		public System.String Default = "";
		public System.String Description = "";
		public System.String Get = "";
		public System.String Set = "";
		public System.Boolean ReadOnly = false;
		public System.Boolean SqlOnly = false;
		public System.String DIOSDefault = "";
		public System.String DIOSType = "";
		public System.Boolean SuppressInView = false;
		public System.Boolean UseInWebForm = false;
		public System.String EnumTypeName = "";
		public string PropertyName = "";

		#region Методы
		#region GetCSharpType
		public System.String GetCSharpType()
		{
return Static.GetCSharpType(this.Type);
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
				return this.Name;//.ToLower();
			}
		}
		#endregion
		#region GetDIOSType
		public System.String GetDIOSType()
		{
string DIOSType = this.DIOSType;
if ((DIOSType != null)&&(DIOSType.Length > 0))
	return DIOSType;
else
	return this.GetCSharpType();

		}
		#endregion
		#region Clone
		public Attribute Clone()
		{
Attribute attr = new Attribute();
attr.Name = this.Name;
attr.Default = this.Default;
attr.Description = this.Description;
attr.Get = this.Get;
attr.SqlOnly = this.SqlOnly;
attr.Null = this.Null;
attr.PhisicalName = this.PhisicalName;
attr.ReadOnly = this.ReadOnly;
attr.Set = this.Set;
attr.SuppressInView = this.SuppressInView;
attr.Type = this.Type;
attr.DIOSDefault = this.DIOSDefault;
attr.DIOSType = this.DIOSType;
return attr;
		}
		#endregion
		#region Generate
		public System.String Generate()
		{
return "";
			/*
			string attr_name = this.Name;
			string attr_type = this.Type;
			string attr_phisical_name = this.GetSqlName();
			string attr_null = this.Null;
			string attr_defcs = this.DIOSDefault; if (attr_defcs == null) attr_defcs = "";
			string attr_csharp_type = this.GetCSharpType();
			string attr_DIOS_type = this.GetDIOSType();
			bool attr_readonly = this.ReadOnly;
			
			sbProperties.Append(coma + attr_phisical_name);
			coma = ", ";
			
			sb.Append("\t\t#region " + attr_phisical_name + "\n");
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
				sb.Append(";\n");
			}
			string objectPropertyAttribute = "\t\t[ObjectPropertyAttribute(\"" + attr_name + "\"";
			if (attr_null == "not null")
			{
				sb.Append("\t\t[NotNull(\"\")]\n");
				sbInterface.Append("\t\t[NotNull(\"\")]\n");
				objectPropertyAttribute += ", true";
			}
			else
			{
				objectPropertyAttribute += ", false";
			}
			if (attr_name != attr_phisical_name)
			{
				sb.Append("\t\t[LocalizedName(\"" + attr_name + "\")]\n");
				sbInterface.Append("\t\t[LocalizedName(\"" + attr_name + "\")]\n");
			}
			objectPropertyAttribute += ")]\n";
			sb.Append(objectPropertyAttribute);
			sbInterface.Append(objectPropertyAttribute);
			
			if (isAbstract)
			{
				sb.Append("\t\tpublic abstract " + attr_DIOS_type + " " + attr_phisical_name + "{");
			}
			else
			{
				sb.Append("\t\tpublic " + attr_DIOS_type + " " + attr_phisical_name + "\n\t\t{");
			}
			if (isAbstract)
			{
				sb.Append("get;set;}\n");
			}
			else
			{
				string attr_get = this.Get;
				if ((attr_get == null)||(attr_get == ""))
				{
					if (attr_csharp_type != attr_DIOS_type)
					{
						attr_get = "\t\t\t\treturn (" + attr_DIOS_type + ")(int)" + " _" + attr_phisical_name + ";";
					}
					else
					{
						attr_get = "\t\t\t\treturn _" + attr_phisical_name + ";"; 
					}
				}
				string attr_set = this.Set;
				if (!attr_readonly)
				{
					if ((attr_set == null)||(attr_set == ""))
					{
						attr_set = "";
						if (attr_csharp_type == "SqlString")
						{
							int maxLength = 0;
							string sqlType = this.Type;
							if (sqlType == "sysname") maxLength = 128;
							if ((sqlType.Trim().ToLower().StartsWith("varchar"))||(sqlType.Trim().ToLower().StartsWith("nvarchar")))
							{
								string lenStr = sqlType.Trim(new char[]{' ', '(', ')', 'v', 'a', 'r', 'c', 'h', 'n'});
								try
								{
									maxLength = Int32.Parse(lenStr);
								}
								catch
								{
									 throw new Exception("Ошибка в типе. Не удалось определить длину поля " + this.Name);
								}
							}
							if (maxLength > 0)
							{
								attr_set = "\t\t\t\tif (value.ToString().Length > " 
									+ maxLength.ToString() + ")\n\t\t\t\t{\n\t\t\t\t\t" 
									+ "throw new SimpleException(\"Длина поля " 
									+ this.Name + " не может быть больше " + maxLength.ToString() 
									+ "\");\n\t\t\t\t}\n";
							}
						}
						if (attr_csharp_type != attr_DIOS_type)
						{
							attr_set += "\t\t\t\t_" + attr_phisical_name + " = (int)value;";
						}
						else
						{
							attr_set += "\t\t\t\t_" + attr_phisical_name + " = value;"; 
						}
					}
				}
				sb.Append("\n");
				sb.Append("\t\t\tget\n\t\t\t{\n");
				sb.Append(attr_get);
				sb.Append("\n\t\t\t}\n");
				if (!attr_readonly)
				{
					sb.Append("\t\t\tset\n\t\t\t{\n");
					sb.Append(attr_set);
					sb.Append("\n\t\t\t}\n");
				}
				sb.Append("\t\t}\n");
			}
			sb.Append("\t\t#endregion\n");
			
			sbInterface.Append("\t\t" + attr_DIOS_type + " " + attr_phisical_name + "{get;");
			if (!attr_readonly)
			{
				sbInterface.Append("set;");
			}
			sbInterface.Append("}\n");*/
		}
		#endregion
		#region GenerateInterface
		public System.String GenerateInterface()
		{
return "";
			/*string attr_name = this.Name;
			string attr_type = this.Type;
			string attr_phisical_name = this.GetSqlName();
			string attr_null = this.Null;
			string attr_defcs = this.DIOSDefault; if (attr_defcs == null) attr_defcs = "";
			string attr_csharp_type = this.GetCSharpType();
			string attr_DIOS_type = this.GetDIOSType();
			bool attr_readonly = this.ReadOnly;
			
			sbProperties.Append(coma + attr_phisical_name);
			coma = ", ";
			
			sb.Append("\t\t#region " + attr_phisical_name + "\n");
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
				sb.Append(";\n");
			}
			string objectPropertyAttribute = "\t\t[ObjectPropertyAttribute(\"" + attr_name + "\"";
			if (attr_null == "not null")
			{
				sb.Append("\t\t[NotNull(\"\")]\n");
				sbInterface.Append("\t\t[NotNull(\"\")]\n");
				objectPropertyAttribute += ", true";
			}
			else
			{
				objectPropertyAttribute += ", false";
			}
			if (attr_name != attr_phisical_name)
			{
				sb.Append("\t\t[LocalizedName(\"" + attr_name + "\")]\n");
				sbInterface.Append("\t\t[LocalizedName(\"" + attr_name + "\")]\n");
			}
			objectPropertyAttribute += ")]\n";
			sb.Append(objectPropertyAttribute);
			sbInterface.Append(objectPropertyAttribute);
			
			if (isAbstract)
			{
				sb.Append("\t\tpublic abstract " + attr_DIOS_type + " " + attr_phisical_name + "{");
			}
			else
			{
				sb.Append("\t\tpublic " + attr_DIOS_type + " " + attr_phisical_name + "\n\t\t{");
			}
			if (isAbstract)
			{
				sb.Append("get;set;}\n");
			}
			else
			{
				string attr_get = this.Get;
				if ((attr_get == null)||(attr_get == ""))
				{
					if (attr_csharp_type != attr_DIOS_type)
					{
						attr_get = "\t\t\t\treturn (" + attr_DIOS_type + ")(int)" + " _" + attr_phisical_name + ";";
					}
					else
					{
						attr_get = "\t\t\t\treturn _" + attr_phisical_name + ";"; 
					}
				}
				string attr_set = this.Set;
				if (!attr_readonly)
				{
					if ((attr_set == null)||(attr_set == ""))
					{
						attr_set = "";
						if (attr_csharp_type == "SqlString")
						{
							int maxLength = 0;
							string sqlType = this.Type;
							if (sqlType == "sysname") maxLength = 128;
							if ((sqlType.Trim().ToLower().StartsWith("varchar"))||(sqlType.Trim().ToLower().StartsWith("nvarchar")))
							{
								string lenStr = sqlType.Trim(new char[]{' ', '(', ')', 'v', 'a', 'r', 'c', 'h', 'n'});
								try
								{
									maxLength = Int32.Parse(lenStr);
								}
								catch
								{
									 throw new Exception("Ошибка в типе. Не удалось определить длину поля " + this.Name);
								}
							}
							if (maxLength > 0)
							{
								attr_set = "\t\t\t\tif (value.ToString().Length > " 
									+ maxLength.ToString() + ")\n\t\t\t\t{\n\t\t\t\t\t" 
									+ "throw new SimpleException(\"Длина поля " 
									+ this.Name + " не может быть больше " + maxLength.ToString() 
									+ "\");\n\t\t\t\t}\n";
							}
						}
						if (attr_csharp_type != attr_DIOS_type)
						{
							attr_set += "\t\t\t\t_" + attr_phisical_name + " = (int)value;";
						}
						else
						{
							attr_set += "\t\t\t\t_" + attr_phisical_name + " = value;"; 
						}
					}
				}
				sb.Append("\n");
				sb.Append("\t\t\tget\n\t\t\t{\n");
				sb.Append(attr_get);
				sb.Append("\n\t\t\t}\n");
				if (!attr_readonly)
				{
					sb.Append("\t\t\tset\n\t\t\t{\n");
					sb.Append(attr_set);
					sb.Append("\n\t\t\t}\n");
				}
				sb.Append("\t\t}\n");
			}
			sb.Append("\t\t#endregion\n");
			
			sbInterface.Append("\t\t" + attr_DIOS_type + " " + attr_phisical_name + "{get;");
			if (!attr_readonly)
			{
				sbInterface.Append("set;");
			}
			sbInterface.Append("}\n");*/
		}
		#endregion
		#region GetCSharpNativeType
		public System.String GetCSharpNativeType()
		{
return Static.GetCSharpNativeType(this.Type);
		}
		#endregion
		#region DropColumn
		public string DropColumn()
		{
string attr_phisical_name = this.GetSqlName();
string table_phisical_name = (this.Parent as Entity).GetSqlName();
string db_name = (this.Parent as Entity).GetDBName();
string script = "if exists(select * from syscolumns where id = OBJECT_ID('" + table_phisical_name + "') and name = '" + attr_phisical_name + "')\n"
+ "\t alter table " + table_phisical_name + " drop column " + attr_phisical_name;
return script;
		}
		#endregion
		#endregion
	}
}
