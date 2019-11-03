using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.IO;
using System.Collections;
using Dersa.Common;
using DIOS.Common.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Static: StereotypeBaseE, ICompiledEntity
	{
		public Static(){}

		public Static(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

		#region Методы
		#region GetParamValue
		public static object GetParamValue(string description)
		{
bool HasBase64Code = description.Contains("=B64\"");
			if(HasBase64Code)
			       description = description.Replace("=B64", "ъ");
			else
			       description = description.Replace("=", "ъ");
			string[] descrParts = description.Split('ъ');
			string descrVal = descrParts[1].Replace("\"","").Trim();
			string typeName = descrParts[0].Trim().Split(' ')[0];
			System.Type T = System.Type.GetType(typeName);
			if(T == null)
			    throw new System.Exception("No type for type name " + typeName);
			
			return Dersa.Common.DersaUtil.Convert(descrVal, T);
		}
		#endregion
		#region CompileAndExecuteAditionalMethod
		public static System.Object CompileAndExecuteAditionalMethod(Dersa.Interfaces.ICompiledEntity owner, string returnType, string methodName, string code, string parameters, object[] args, string[] Using)
		{
		if ((code == null)||(code.Length == 0)) return null;
					System.Type ownerType = owner.GetType();
					string stereotypeName = ownerType.Name;
					string objectName = stereotypeName + "_" + methodName;
					if (parameters == null) parameters = "";
					if (parameters == "") args = null;
					
					
					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					sb.Append("using System;\n");
					sb.Append("using System.Linq;\n");
					sb.Append("using System.Runtime.Serialization;\n");
					//sb.Append("using Dersa.Interfaces;\n");
					sb.Append("\n\n");
					sb.Append("namespace DersaStereotypes\n{\n");
					sb.Append("[Serializable()]\n");
					sb.Append("public class " + objectName + ": " + stereotypeName + "\n");
					sb.Append("{\n");
					sb.Append("\tpublic " + returnType + " " + methodName + "(" + parameters + ")\n");
					sb.Append("\t{\n");
					sb.Append(code);
					sb.Append("\t}\n");
					sb.Append("}\n}");
					
					string source = sb.ToString();
					System.Collections.Specialized.StringCollection additionalReferences = new System.Collections.Specialized.StringCollection();
					System.Type testOwnerType = ownerType;
					while ((testOwnerType != null)&&(testOwnerType != typeof(object)))
					{
						string assName = testOwnerType.Assembly.GetName().CodeBase.Substring(8);
						additionalReferences.Add(assName);
						testOwnerType = testOwnerType.BaseType;
					}
					string[] referencedAssemblies = {
			                              "System.dll",
			                              "System.Core.dll",
			                              "System.Data.dll",
			                              "System.XML.dll",
			                              AppDomain.CurrentDomain.BaseDirectory + "bin\\" + "Dios.Interfaces.dll",
			                              AppDomain.CurrentDomain.BaseDirectory + "bin\\" + "Dersa.Interfaces.dll"
			                };
			
					additionalReferences.AddRange(referencedAssemblies);
			                if(Using == null)		
			                    Using = new string[0];
					additionalReferences.AddRange(Using);
			
					referencedAssemblies = new string[additionalReferences.Count];
					additionalReferences.CopyTo(referencedAssemblies, 0);
					
					System.CodeDom.Compiler.CompilerParameters cp = new System.CodeDom.Compiler.CompilerParameters(referencedAssemblies);
					cp.GenerateInMemory  = false;
					
					cp.OutputAssembly = System.IO.Path.GetTempFileName();
					
					System.CodeDom.Compiler.ICodeCompiler codeCompiler = new Microsoft.CSharp.CSharpCodeProvider().CreateCompiler();
					System.CodeDom.Compiler.CompilerResults results = codeCompiler.CompileAssemblyFromSource(cp, source);
					if ((results != null)&&(results.Output.Count > 0))
					{
						System.Text.StringBuilder errorSb = new System.Text.StringBuilder();
						errorSb.Append("Объект: " + owner.Name + ", метод " + methodName + ", ID = " + "???" + "\n\n");
						for (int k = 0; k < results.Output.Count; k++)
						{
							errorSb.Append(results.Output[k] + "\n");
						}
						Console.WriteLine(source);
						throw new Exception(errorSb.ToString());
					}
					System.Reflection.Assembly assembly = results.CompiledAssembly;
					System.Type newObjectType = assembly.GetType("DersaStereotypes." + objectName);
					object newObject = System.Activator.CreateInstance(newObjectType);
					
					System.Reflection.FieldInfo[] fis = newObjectType.GetFields(System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic);
					try
					{
						foreach (System.Reflection.FieldInfo fi in fis)
						{
							System.Reflection.FieldInfo fiOwner = ownerType.GetField(fi.Name, System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic);
							fi.SetValue(newObject, fiOwner.GetValue(owner));
						}
					}
					catch (Exception ex)
					{
						throw new Exception("Ошибка создания экземпляра объекта " + objectName, ex);
					}
					System.Reflection.MethodInfo mi = newObjectType.GetMethod(methodName);
					return mi.Invoke(newObject, args);
			
		}
		#endregion
		#region GetParamValues
		public static System.Collections.Hashtable GetParamValues(string json_params)
		{
Hashtable result = new Hashtable();
			try
			{
			        IParameterCollection Params = DIOS.Common.Util.DeserializeParams(json_params);
			        foreach(IParameter Param in Params)
			        {
			            result[Param.Name] = Param.Value;
			        }
			}
			catch(Exception exc)
			{
			    throw;
			}
			return result;
		}
		#endregion
		#region GetCSharpType
		public static System.String GetCSharpType(System.String typeName)
		{
            string type = (string)Static.GetFieldType(typeName);
			            type = type.Trim().ToLower()
			                .Replace("identity", "")
			                .Replace("number(8)", "int")
			                .Replace("varchar2", "varchar")
			                .Replace("number", "numeric")
			                ;
			            if ((type == "varchar")||(type == "char")||(type == "text")||(type == "sysname")||
						(type == "nvarchar")||(type == "nchar")||(type == "ntext"))	return "SqlString";
					if ((type == "timestamp")||(type == "image")||(type == "binary")||(type == "varbinary"))return "SqlBinary";
					if (type == "tinyint") return "SqlByte";
					if (type == "smallint") return "SqlInt16";
					if (type == "int") return "SqlInt32";
					if (type == "bigint")return "SqlInt64";
					if ((type == "date") || (type == "datetime")||(type == "smalldatetime")) return "SqlDateTime";
					if ((type == "numeric")||(type == "decimal")) return "SqlDecimal";
					if ((type == "money")||(type == "smallmoney")) return "SqlMoney";
					if ((type == "float")||(type == "real")) return "SqlDouble";
					if (type == "bit") return "SqlBoolean";
					if (type == "color") return "SqlTypeFontColor";
					if (type == "uniqueidentifier") return "SqlGuid";
			        if (System.Type.GetType(typeName) != null) return typeName;
			
			        return "wrong type:" + typeName;
		}
		#endregion
		#region GetFieldType
		public static System.String GetFieldType(System.String typeName)
		{
if ((typeName != null)&&(typeName.Length > 0))
			{
				int sIndex = typeName.IndexOf('(');
				if (sIndex > -1)
				{
					return typeName.Substring(0, sIndex);
				}
				return typeName;
			}
			throw new Exception("Тип не определен");
		}
		#endregion
		#region GetCSharpObjectName
		public static System.String GetCSharpObjectName(ICompiled owner)
		{
System.Reflection.FieldInfo fi = owner.GetType().GetField("PhisicalName");
			if (fi == null) return "";
			{
				string phisicalName = (string)fi.GetValue(owner);
				if (phisicalName == "")
				{
					phisicalName = owner.Name;
				}
				return GetCSharpName(phisicalName);
			}
		}
		#endregion
		#region GetCSharpName
		public static System.String GetCSharpName(System.String name)
		{
name = name.ToLower();
			string ret = "";
			for (int i = 0; i < name.Length; i++)
			{
				char buf = name[i];
				if (buf == ' ') buf = '_';
				if (i == 0)
				{
					buf = Char.ToUpper(buf);
				}
				if ((i > 0)&&((name[i - 1] == '_')||(name[i - 1] == ' ')))
				{
					buf = Char.ToUpper(buf);
				}
				if ((i + 1 > 0)&& !((buf == '_')||(buf == ' ')))
				{
					ret += buf;
				}
			}
			return ret;
		}
		#endregion
		#region SaveToFile
		public static System.IO.FileInfo SaveToFile(string fileName, byte[] bts)
		{
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
					{
						fs.Write(bts, 0, bts.Length);
						fs.Flush();
					}
			            System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
			            return fi;
			
		}
		#endregion
		#region SaveToFile
		public static System.IO.FileInfo SaveToFile(string fileName, string str)
		{
return SaveToFile(fileName, str, System.Text.Encoding.GetEncoding(1251));
		}
		#endregion
		#region GetCSharpNativeType
		public static System.String GetCSharpNativeType(System.String typeName)
		{
string type = (string)Static.GetFieldType(typeName);
			if ((type == "varchar")||(type == "char")||(type == "text")||(type == "sysname")||
			(type == "nvarchar")||(type == "nchar")||(type == "ntext"))
			return "string";
			if ((type == "timestamp")||(type == "image")||(type == "binary")||(type == "varbinary"))return "byte[]";
			if (type == "tinyint") return "byte";
			if (type == "smallint") return "short";
			if (type == "int") return "int";
			if (type == "bigint")return "long";
			if ((type == "datetime")||(type == "smalldatetime")) return "DateTime";
			if ((type == "numeric")||(type == "decimal")) return "decimal";
			if ((type == "money")||(type == "smallmoney")) return "decimal";
			if ((type == "float")||(type == "real")) return "double";
			if (type == "bit") return "bool";
			if (type == "uniqueidentifier") return "Guid";
			
			return "wrong type:" + type;
		}
		#endregion
		#region Information
		public static void Information(string information)
		{
//MessageBox.Show(information + ".", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
		}
		#endregion
		#region Warning
		public static void Warning(string warning)
		{
//MessageBox.Show(warning + "!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
		}
		#endregion
		#region Ask
		public static bool Ask(string question)
		{
//if (MessageBox.Show(question + "?", "Подтверждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
			//{
			//	return true;
			//}
			//return false;
			return true;
		}
		#endregion
		#region ErrorMsg
		public static void ErrorMsg(string error)
		{
//MessageBox.Show(error + "!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
			throw new Exception(error);
		}
		#endregion
		#region ExecSql
		public static string ExecSql(string text, string server, string database, string user, string password)
		{
string connectionString = "";
			if ((user.Length == 0)&&(password.Length == 0))
			{
				connectionString = "Integrated Security=SSPI;Persist Security Info=False" +
									";Initial Catalog=" + database +
									";Data Source=" + server +
									";Application Name=Modeler" +
									";Packet Size=4096";
			}
			else
			{
				connectionString = "Data Source=" + server + ";" +
									"Initial Catalog=" + database + ";" +
									"Password=" + password + ";" +
									"Persist Security Info=True;" +
									"User id=" + user + ";" +
									"Packet size=4096";
			}
			
			return ExecSql(connectionString, text);
		}
		#endregion
		#region CompileAndExecuteAditionalMethod
		public static System.Object CompileAndExecuteAditionalMethod(Dersa.Interfaces.ICompiledEntity owner, string returnType, string methodName, string code, string parameters, object[] args)
		{
return CompileAndExecuteAditionalMethod(owner, returnType, methodName, code, parameters, args, null);
		}
		#endregion
		#region CompileAndExecuteAditionalMethods
		public static System.Object[] CompileAndExecuteAditionalMethods(Dersa.Interfaces.ICompiledEntity owner, ExecuteObject[] executeObject)
		{
		System.Type ownerType = owner.GetType();
					string stereotypeName = ownerType.Name;
					string tempFileName = System.IO.Path.GetTempFileName();
					Guid g = Guid.NewGuid();
					string objectName = stereotypeName + "_" + g.ToString().Replace("-", "_");// DateTime.Now.ToString("ddMMyyhhmmssff");
					
					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					sb.Append("using System;\n");
					sb.Append("using System.Runtime.Serialization;\n");
					sb.Append("using Dersa.Interfaces;\n");
					sb.Append("\n\n");
					sb.Append("namespace DersaStereotypes\n{\n");
					sb.Append("[Serializable()]\n");
					sb.Append("public class " + objectName + ": " + stereotypeName + "\n");
					sb.Append("{\n");
					for (int i = 0; i < executeObject.Length; i++)
					{
						ExecuteObject eo = executeObject[i];
						if ((eo != null)&&(eo.Text != null)&&(eo.Text.Length > 0))
						{
							sb.Append("\tpublic " + eo.ReturnType + " " + eo.MethodName + "(" + eo.Parameters + ")\n");
							sb.Append("\t{\n");
							sb.Append(eo.Text);
							sb.Append("\t}\n");
						}
					}
					sb.Append("}\n}");
					
					string source = sb.ToString();
					System.Collections.Specialized.StringCollection additionalReferences = new System.Collections.Specialized.StringCollection();
					System.Type testOwnerType = ownerType;
					while ((testOwnerType != null)&&(testOwnerType != typeof(object)))
					{
						string assName = testOwnerType.Assembly.GetName().CodeBase.Substring(8);
					//	Console.WriteLine(testOwnerType.FullName + " -> " + assName);
						additionalReferences.Add(assName);
						testOwnerType = testOwnerType.BaseType;
					}
					//Console.WriteLine("------");
					//string stereotypesAssembly = ModelerRemote.Interfaces.Util.StereotypesAssembly.GetName().Name;
					
					string[] referencedAssemblies = {"System.dll",
														"System.Data.dll",
														"System.Drawing.dll",
														"System.Windows.Forms.dll",
														"System.XML.dll",
			                                            AppDomain.CurrentDomain.BaseDirectory + "bin\\" + "Dios.Interfaces.dll"
														//System.Windows.Forms.Application.StartupPath + "\\" + stereotypesAssembly + ".dll",
			                                            //System.Windows.Forms.Application.StartupPath + "\\Dersa.Interfaces.dll",
			                                            //System.Windows.Forms.Application.StartupPath + "\\Modeler.Editor.dll"
			        };
					additionalReferences.AddRange(referencedAssemblies);
					referencedAssemblies = new string[additionalReferences.Count];
					additionalReferences.CopyTo(referencedAssemblies, 0);
					
					System.CodeDom.Compiler.CompilerParameters cp = new System.CodeDom.Compiler.CompilerParameters(referencedAssemblies);
					cp.GenerateInMemory  = false;
					
					cp.OutputAssembly = tempFileName;
					
					System.CodeDom.Compiler.ICodeCompiler codeCompiler = new Microsoft.CSharp.CSharpCodeProvider().CreateCompiler();
					System.CodeDom.Compiler.CompilerResults results = codeCompiler.CompileAssemblyFromSource(cp, source);
					if ((results != null)&&(results.Output.Count > 0))
					{
						System.Text.StringBuilder errorSb = new System.Text.StringBuilder();
						errorSb.Append("Объект: " + owner.Name + "\n\n");// + ", метод " + methodName + ", ID = " + "???" + "\n\n");
						for (int k = 0; k < results.Output.Count; k++)
						{
							errorSb.Append(results.Output[k] + "\n");
						}
						Console.WriteLine(source);
						throw new Exception(errorSb.ToString());
					}
					System.Reflection.Assembly assembly = results.CompiledAssembly;
					System.Type newObjectType = assembly.GetType("DersaStereotypes." + objectName);
					object newObject = System.Activator.CreateInstance(newObjectType);
					
					System.Reflection.FieldInfo[] fis = newObjectType.GetFields(System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic);
					try
					{
						foreach (System.Reflection.FieldInfo fi in fis)
						{
							System.Reflection.FieldInfo fiOwner = ownerType.GetField(fi.Name, System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic);
							fi.SetValue(newObject, fiOwner.GetValue(owner));
						}
					}
					catch (Exception ex)
					{
						throw new Exception("Ошибка создания экземпляра объекта " + objectName, ex);
					}
					object[] returnObjects = new object[executeObject.Length];
					
					for (int i = 0; i < executeObject.Length; i++)
					{
						ExecuteObject eo = executeObject[i];
						if (eo != null)
						{
							System.Reflection.MethodInfo mi = newObjectType.GetMethod(eo.MethodName);
							if (mi != null)
							{
								returnObjects[i] =  mi.Invoke(newObject, eo.Args);
							}
						}
					}
					return returnObjects;
			
		}
		#endregion
		#region CountChar
		public static int CountChar(string str, char ch)
		{
if (str == null) return 0;
			return str.Split(ch).Length - 1;
		}
		#endregion
		#region CompileAndExecuteAditionalMethod
		public static System.Object CompileAndExecuteAditionalMethod(Dersa.Interfaces.ICompiledEntity owner, string returnType, string methodName, string code)
		{
return CompileAndExecuteAditionalMethod(owner, returnType, methodName, code, null, null);
		}
		#endregion
		#region Input
		public static bool Input(string description, ref string value)
		{
//	if (InputBox.Show(description, "Введите значение", ref value) == DialogResult.OK)
			//	{
			//		return true;
			//	}
			//	return false;
			return true;
		}
		#endregion
		#region GetCLRNativeType
		public static System.String GetCLRNativeType(System.String typeName)
		{
string type = (string)Static.GetFieldType(typeName);
			if ((type == "varchar")||(type == "char")||(type == "text")||(type == "sysname")||
			(type == "nvarchar")||(type == "nchar")||(type == "ntext"))
			return "String";
			if ((type == "timestamp")||(type == "image")||(type == "binary")||(type == "varbinary"))return "Binary";
			if (type == "tinyint") return "Byte";
			if (type == "smallint") return "Int16";
			if (type == "int") return "Int32";
			if (type == "bigint")return "Int64";
			if ((type == "datetime")||(type == "smalldatetime")) return "DateTime";
			if ((type == "numeric")||(type == "decimal")) return "Decimal";
			if ((type == "money")||(type == "smallmoney")) return "Decimal";
			if ((type == "float")||(type == "real")) return "Double";
			if (type == "bit") return "Boolean";
			if (type == "color") return "TypeFontColor";
			if (type == "uniqueidentifier") return "Guid";
			
			return "wrong type:" + type;
		}
		#endregion
		#region GetXsdType
		public static System.String GetXsdType(System.String typeName)
		{
string type = (string)Static.GetFieldType(typeName);
			if ((type == "varchar")||(type == "char")||(type == "text")||(type == "sysname")||
			(type == "nvarchar")||(type == "nchar")||(type == "ntext"))
			return "xsd:string";
			if ((type == "timestamp")||(type == "image")||(type == "binary")||(type == "varbinary"))return "xsd:base64Binary";
			if (type == "tinyint") return "xsd:byte";
			if (type == "smallint") return "xsd:short";
			if (type == "int") return "xsd:int";
			if (type == "bigint")return "xsd:long";
			if ((type == "datetime")||(type == "smalldatetime")) return "xsd:dateTime";
			if ((type == "numeric")||(type == "decimal")) return "xsd:decimal";
			if ((type == "money")||(type == "smallmoney")) return "xsd:decimal";
			if ((type == "float")||(type == "real")) return "xsd:double";
			if (type == "bit") return "xsd:boolean";
			if (type == "uniqueidentifier") return "guid";
			
			return "wrong type:" + type;
		}
		#endregion
		#region SaveToFile
		public static System.IO.FileInfo SaveToFile(string fileName, string str, System.Text.Encoding ec)
		{
byte[] strBytes = ec.GetBytes(str);
			return SaveToFile(fileName, strBytes);
		}
		#endregion
		#region ExecSql
		public static string ExecSql(string connectionString, string text)
		{
/*
			string connectionString = "";
			if ((user.Length == 0)&&(password.Length == 0))
			{
				connectionString = "Integrated Security=SSPI;Persist Security Info=False" +
									";Initial Catalog=" + database +
									";Data Source=" + server +
									";Application Name=Modeler" +
									";Packet Size=4096";
			}
			else
			{
				connectionString = "Data Source=" + server + ";" +
									"Initial Catalog=" + database + ";" +
									"Password=" + password + ";" +
									"Persist Security Info=True;" +
									"User id=" + user + ";" +
									"Packet size=4096";
			}*/
			
			string[] splits = text.Split('\n');
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.Collections.Specialized.StringCollection commandStrings = new System.Collections.Specialized.StringCollection();
			foreach(string s in splits)
			{
				if (s.Trim() == "go")
				{
					commandStrings.Add(sb.ToString());
					sb = new System.Text.StringBuilder();
				}
				else
				{
					sb.Append(s + "\n");
				}
			}
			commandStrings.Add(sb.ToString());
			
			System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectionString);
			connection.Open();
			try
			{
				foreach (string commandStr in commandStrings)
				{
					if (commandStr.Trim() != "")
					{
						System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(commandStr, connection);
						command.CommandTimeout = 600;
						command.ExecuteNonQuery(); 
					}
				}
				return "Выполнение завершено без ошибок";
			}
			catch(Exception exc)
			{
				return exc.Message;
			}
			finally
			{
				connection.Close();
			}
			
		}
		#endregion
		#endregion
	}
}
