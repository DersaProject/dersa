using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace DersaStereotypes
{
	[Serializable()]
	public class Package: StereotypeBaseE, ICompiledEntity
	{
		public Package(){}

		public Package(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String BaseDir = "";
		public System.String Namespace = "";
		public System.String DirRelational = "";
		public System.String Using = "";
		public System.String db_name = "";
		public System.String ApplicationServerName = "";
		public System.String Permissions = "";
		public System.String RoleNames = "";
		public System.String DatabaseServer = "ACER";
		public System.String GlobalName = "";
		public System.String LogDatabaseServer = "ACER";
		public System.String AssemblyName = "";
		public System.Boolean Active = true;

		#region Методы
		#region GetAssemblyProducer
		public Package GetAssemblyProducer()
		{
            Package result = this;
			            if (string.IsNullOrEmpty(this.AssemblyName))
			            {
			                result = null;
			                if (this.Parent != null && this.Parent is Package)
			                    return (this.Parent as Package).GetAssemblyProducer();
			            }
			            return result;
			
		}
		#endregion
		#region UploadObjects
		public string UploadObjects(object[] Params)
		{
            int id = this.Id;
			            return "window.open(\"Node/UploadContent?id=" + id.ToString() + "\")";
			
		}
		#endregion
		#region GetDirectory
		public System.String GetDirectory()
		{
string dir = this.GetBaseDir();
			string dirRelational = this.GetDirRelational();
			
			string returnDir = dir + dirRelational;
			
			returnDir = returnDir.Replace("\\\\", "\\");
			return returnDir;
		}
		#endregion
		#region GetDBName
		public System.String GetDBName()
		{
string name = "";
			name = db_name;
			if (name.Length == 0)
			{
				if (Parent is Package)
				{
					return (Parent as Package).GetDBName();
				}
			}
			return name;
		}
		#endregion
		#region GetConfiguration
		public Configuration GetConfiguration(System.String name)
		{
System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
			        DIOS.Common.Logger.LogStatic("child name = " + obj.Name);
				if ((obj is Configuration)&&(obj.Name == name))
				{
					return (Configuration)obj;
				}
			}
			ICompiledEntity parent = this.Parent;
			if ((parent != null)&&(parent is Package))
			{
				Configuration result = ((Package)parent).GetConfiguration(name);
				if (result != null) return result;
			}
			
			return null;
		}
		#endregion
		#region GetGlobalName
		public System.String GetGlobalName()
		{
string name = GlobalName;
			if (name.Length == 0)
			{
				if (Parent is Package)
				{
					return (Parent as Package).GetGlobalName();
				}
			}
			return name;
		}
		#endregion
		#region GetDatabaseServer
		public System.String GetDatabaseServer()
		{
string name = DatabaseServer;
			if (name.Length == 0)
			{
				if (Parent is Package)
				{
					return (Parent as Package).GetDatabaseServer();
				}
			}
			return name;
		}
		#endregion
		#region GetLogDatabaseServer
		public System.String GetLogDatabaseServer()
		{
string name = LogDatabaseServer;
			if (name.Length == 0)
			{
				if (Parent is Package)
				{
					return (Parent as Package).GetLogDatabaseServer();
				}
			}
			return name;
		}
		#endregion
		#region GetApplicationServerName
		public System.String GetApplicationServerName()
		{
string name = ApplicationServerName;
			if (name.Length == 0)
			{
				if (Parent is Package)
				{
					return (Parent as Package).GetApplicationServerName();
				}
			}
			return name;
		}
		#endregion
		#region GetBaseDir
		public System.String GetBaseDir()
		{
string name = BaseDir;
			if (name.Length == 0)
			{
				if (Parent is Package)
				{
					return (Parent as Package).GetBaseDir();
				}
			}
			return name;
		}
		#endregion
		#region GetDirRelational
		public System.String GetDirRelational()
		{
string name = DirRelational;
			if (name.Length == 0)
			{
				if (Parent is Package)
				{
					return (Parent as Package).GetDirRelational();
				}
			}
			return name;
		}
		#endregion
		#region GetUsing
		public System.String GetUsing(string usingDefault)
		{
System.Collections.IList usingList = GetUsingList(usingDefault);
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < usingList.Count; i++)
			{
				sb.Append(usingList[i].ToString() + ";\n");
			}
			return sb.ToString();
		}
		#endregion
		#region GetUsingList
		public System.Collections.IList GetUsingList(string usingDefault)
		{
string usingString = this.Using;
			
			if (usingDefault != null && usingDefault != "") 
			usingString = usingDefault + usingString;
			
			string[] usingParsed = usingString.Split(';', '\n');
			System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
			
			for (int i = 0; i < usingParsed.Length; i++)
			{
				string thisUsing = usingParsed[i];
				if (!sc.Contains(thisUsing) && thisUsing.Trim() != "")
				{
					sc.Add(thisUsing);
				}
			}
			
			if (Parent is Package)
			{
				System.Collections.IList parentSc = ((Package)Parent).GetUsingList(null);
				for (int i = 0; i < parentSc.Count; i++)
				{
					string parentUsing = parentSc[i].ToString();
					if (!sc.Contains(parentUsing))
					{
						sc.Add(parentUsing);
					}
				}
			}
			return sc;
		}
		#endregion
		#region GenerateAllObjects
		public FileInfo GenerateAllObjects(Package producer)
		{
            DIOS.Common.Logger.LogStatic("started GenerateAllObjects, package " + this.Id.ToString());
			            if (producer == null)
			                producer = this.GetAssemblyProducer();
			            if (producer == null)
			                return null;
			            if (producer != this)
			                return producer.GenerateAllObjects(producer);
			            //вот теперь мы точно знаем, что эта папка - та самая, которую надо скомпилировать
			
			            string FileName = AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + this.AssemblyName + ".dll";
			
			            System.Collections.ArrayList ChildEntities = new System.Collections.ArrayList();
			            System.Collections.IList children = this.Children;
			            for (int i = 0; i < children.Count; i++)
			            {
			                ICompiledEntity obj = (ICompiledEntity)children[i];
			                if (obj is Entity)
			                {
			                    ChildEntities.Add(obj);
			                }
			                //else if (obj is Package)
			                //{
			                //    System.Collections.IList packageChildren = (obj as Package).GetChildEntities();
			                //    for (int c = 0; c < packageChildren.Count; c++)
			                //    {
			                //        Entity packageChild = packageChildren[c] as Entity;
			                //        if (packageChild != null)
			                //        {
			                //            ChildEntities.Add(packageChild);
			                //        }
			                //    }
			                //}
			            }
			
			            string[] fileNamesForCompile = new string[ChildEntities.Count];
			            for (int c = 0; c < ChildEntities.Count; c++)
			            {
			                string classCodeFileName = "";
			                string source = "";
			                Entity ent = ChildEntities[c] as Entity;
			                if (ent != null)
			                {
			                    //CachedObjects.GetCachedEntities()[ent.Id] = null;
			                    //CachedObjects.GetCachedCompiledInstances()["Entity" + ent.Id.ToString()] = null;
			                    ent.Reinitialize();
			                    DIOS.Common.Logger.LogStatic("add Entity" + ent.Id.ToString() + "; index = " + c.ToString());
			                    classCodeFileName = AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + Static.GetCSharpObjectName(ent) + ".cs";
			                    source = ent.GenerateWebObject();
			                }
			                Stream SW = File.Create(classCodeFileName);
			                byte[] txtbts = System.Text.Encoding.GetEncoding(1251).GetBytes(source);
			                SW.Write(txtbts, 0, txtbts.Length);
			                SW.Flush();
			                SW.Close();
			                fileNamesForCompile[c] = classCodeFileName;
			            }
			            System.Collections.Specialized.StringCollection additionalReferences = new System.Collections.Specialized.StringCollection();
			            string[] referencedAssemblies = {"System.dll",
			                                                        "System.Runtime.Serialization.dll",
			                                                        "System.Collections.dll",
			                                                        "System.Data.dll",
			                                                        "System.Xml.dll",
										"Microsoft.CSharp.dll",
										"System.Dynamic.Runtime.dll",
										"System.Core.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "Newtonsoft.Json.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "DIOS.BusinessBase.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "DIOS.Common.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "DIOS.Interfaces.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "DIOS.SqlManager.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "DIOS.dll"
			                        };
			            additionalReferences.AddRange(referencedAssemblies);
			            referencedAssemblies = new string[additionalReferences.Count];
			            additionalReferences.CopyTo(referencedAssemblies, 0);
			
			            System.CodeDom.Compiler.CompilerParameters cp = new System.CodeDom.Compiler.CompilerParameters(referencedAssemblies);
			            cp.GenerateInMemory = false;
			
			            cp.OutputAssembly = FileName;
			
			            var codeCompiler = Microsoft.CSharp.CSharpCodeProvider.CreateProvider("CSharp");
			            System.CodeDom.Compiler.CompilerResults results = codeCompiler.CompileAssemblyFromFile(cp, fileNamesForCompile);
			            if ((results != null) && (results.Output.Count > 0))
			            {
			                System.Text.StringBuilder errorSb = new System.Text.StringBuilder();
			                errorSb.Append("Errors\n");
			                for (int k = 0; k < results.Output.Count; k++)
			                {
			                    errorSb.Append(results.Output[k] + "\n");
			                }
			                throw new Exception(errorSb.ToString());
			            }
			            byte[] bts = File.ReadAllBytes(FileName);
			            File.Delete(FileName);
			            FileName = AppDomain.CurrentDomain.BaseDirectory + "TempFiles\\Download\\" + this.AssemblyName + ".dll";
			            string zipName = AppDomain.CurrentDomain.BaseDirectory + "TempFiles\\" + this.AssemblyName + ".zip";
			            results.TempFiles.Delete();
			            //for (int n = 0; n < fileNamesForCompile.Length - 1; n++)
			            //{
			            //    File.Delete(fileNamesForCompile[n]);
			            //}
			
			            FileInfo fi = Static.SaveToFile(FileName, bts);
			            FastZip fZip = new FastZip();
			            fZip.CreateZip(zipName, AppDomain.CurrentDomain.BaseDirectory + "TempFiles\\Download\\", false, "");
			            fi.Delete();
			            return new FileInfo(zipName);
		}
		#endregion
		#region sql_GenerateTriggers
		public System.String sql_GenerateTriggers(System.Boolean dialog)
		{
/*
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity o = (ICompiledEntity)children[i];
				if ((o is Entity)&&(!((Entity)o).Abstract))
				{
					result.Append(((Entity)o).sql_GenerateTriggers(false));
				}
				if (o is Package)
				{
					result.Append(((Package)o).sql_GenerateTriggers(false));
				}
			}
			
			string sOut = result.ToString();
			if (dialog)
			{
				SqlExecForm.Exec(sOut, this.GetDatabaseServer(), this.GetDBName());
				return "";
			}
			return sOut;
			*/
			return "";
		}
		#endregion
		#region GetEntities
		public System.Collections.IList GetEntities()
		{
/*
			System.Collections.IList children = this.Children;
			System.Collections.Generic.List<Entity> list = new System.Collections.Generic.List<Entity>();
			foreach (object o in children)
			{
				if (o is Entity && !((Entity)o).Abstract)
				{
					list.Add((Entity)o);
				}
				if (o is Package && ((Package)o).Active)
				{
					list.AddRange(((Package)o).GetEntities());	
				}
			}
			return list;*/
			Map entities = new Map();  
			System.Collections.IList relations = this.ARelations;  
			for (int i = 0; i < relations.Count; i++)  
			{   
				ICompiledRelation rel = (ICompiledRelation)relations[i];   
				if (rel is Inherit)   
				{    
					System.Collections.IList inheritEntities = (rel.B as Xsd).GetEntities();    
					foreach(Entity e in inheritEntities)    
					{     
						entities.Add(e.GetSqlName(), e);    
					}   
				}  
			}  
			System.Collections.IList children = this.Children;  
			for (int i = 0; i < children.Count; i++)  
			{   
				Entity obj = children[i] as Entity;   
				if (obj == null) continue;   
				entities.Add(obj.GetSqlName(), obj);  
			}  
			return entities;
		}
		#endregion
		#region sql_Generate
		public string sql_Generate()
		{
/*
			var entities = this.GetEntities();
			
			System.Text.StringBuilder text = new System.Text.StringBuilder();
			
			text.AppendLine("--=========== Tables ===========--");
			foreach (Entity e in entities)
			{
				text.AppendLine(e.sql_Generate(false, false, true, true, false));	
			}
			
			text.AppendLine("--=========== References ===========--");
			foreach (Entity e in entities)
			{
				Map fk_attrs = e.GetFKLinks() as Map;
				if (fk_attrs != null && fk_attrs.Count > 0)
				{
					for (int i = 0; i < fk_attrs.Count; i++)
					{
						Entity link = fk_attrs[i] as Entity;
						if (link != null)
						{
							text.AppendFormat("\n\t alter table {0} add constraint {1} foreign key ({2}) references {3}",
							e.GetSqlName(), "fk_" + e.GetSqlName().ToLower() + fk_attrs.KeyAt(i),
							fk_attrs.KeyAt(i), link.GetSqlName());
						}
					}
					text.Append("\ngo\n");
				}
			}
			
			text.AppendLine("--=========== Views ===========--");
			foreach (Entity e in entities)
			{
				View view = e.GetSqlView() as View;
				if(view != null)
					text.AppendLine(view.Generate(false, e));	
			}
			if(dialog)
			{
				SqlExecForm.Exec(text.ToString(), GetDatabaseServer(), "");
				return null;
			}
			return text.ToString();*/
			return "";
		}
		#endregion
		#region sql_Merge
		public System.String sql_Merge(System.Boolean dialog)
		{
/*
			var entities = this.GetEntities();
			var sServer = "arctur";
			var dServer = "KALE";
			var dDatabase = "testCMS";
			System.Text.StringBuilder text = new System.Text.StringBuilder();
			text.AppendLine("--=========== Merges ===========--");
			text.AppendLine("--Disable Constraints");
			text.AppendFormat("EXEC {0}.{1}.dbo.sp_MSforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"\n", dServer, dDatabase);
			text.AppendFormat("EXEC {0}.{1}.dbo.sp_MSforeachtable \"ALTER TABLE ? DISABLE TRIGGER  all\"\n", dServer, dDatabase);
			text.AppendLine("go");
			foreach (Entity e in entities)
			{
				text.AppendLine(e.sql_Merge(false, sServer, dServer, e.GetDBName(), dDatabase));	
			}
			text.AppendLine("--Enable Constraints");
			text.AppendFormat("EXEC {0}.{1}.dbo.sp_MSforeachtable @command1=\"print '?'\", @command2=\"ALTER TABLE ? ENABLE TRIGGER all\"\n", dServer, dDatabase);
			text.AppendFormat("EXEC {0}.{1}.dbo.sp_MSforeachtable @command1=\"print '?'\", @command2=\"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"\n", dServer, dDatabase);
			text.AppendLine("go");
			if(dialog)
			{
				SqlExecForm.Exec(text.ToString(), GetDatabaseServer(), "");
				return null;
			}
			return text.ToString();
			*/
			return "";
		}
		#endregion
		#region GenerateDll
		public FileInfo GenerateDll()
		{
            return GenerateAllObjects(null);
			
		}
		#endregion
		#endregion
	}
}
