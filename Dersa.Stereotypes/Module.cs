using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace DersaStereotypes
{
	[Serializable()]
	public class Module: StereotypeBaseE, ICompiledEntity
	{
		public Module(){}

		public Module(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Title = "";
		public System.Int32 Upgraded;
		public System.String PathString = "";
		public System.String Alias = "";

		#region Ìועמהû
		#region sql_Generate
		public string sql_Generate()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("declare @object_type int, @form int, @parent int, @module int\n");	
			
			string ModuleName = this.Title;
			if(ModuleName == "")
				ModuleName = this.Name;
			
			string[] Path = this.PathString.Split('.');
			sb.Append("select @parent = module from MODULE where title = '");
			sb.Append(Path[0]);
			sb.Append("' and parent is null\n");	
			
			int p = 1;
			while(p < Path.Length)
			{
				sb.Append("select @parent = module from MODULE where title = '");
				sb.Append(Path[p++]);
				sb.Append("' and parent = @parent\n");	
			}
			
			sb.Append("\n\n");	
			
			System.Collections.IList children = this.Children;
			ICompiledEntity f = (ICompiledEntity)children[0];
			if (f is BrowseForm)
			{
				BrowseForm browser = f as BrowseForm;
				browser.sql_Generate(false, sb);
				
				sb.Append("\n");	
				sb.Append("select @module = module from MODULE where title = '");
				sb.Append(ModuleName);
				sb.Append("' and parent = @parent\n");
				sb.Append("if @module is null begin\n");
				sb.Append("\tselect @module = max(module) + 1 from MODULE\n");
				sb.Append("\tinsert MODULE(module, parent, title, form, upgraded, alias)\n");
				sb.Append("\tvalues (@module, @parent, '");
				sb.Append(ModuleName);
				sb.Append("', @form, ");
				sb.Append(this.Upgraded.ToString());
				sb.Append(", '");
				sb.Append(this.Alias.ToString());
				sb.Append("')\n");
				sb.Append("\tend\n");
				
				browser.sql_GenerateForChilds(sb);
			}
			
			string result = sb.ToString();
			//if (dialog)
			//{
			//	SqlExecForm.Exec(result, "", "DIOS_common");
			//	return null;
			//}
			//else
			//	Console.WriteLine(result);
			
			return result;
		}
		#endregion
		#region GenerateDll
		public FileInfo GenerateDll()
		{
            string ModuleName = this.Alias;
			            string AssemblyName = Guid.NewGuid().ToString() + ".tmp";
			            string FileName = AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + AssemblyName;
			
			            System.Collections.IList ChildBrowsers = new System.Collections.Generic.List < ICompiledEntity > ();
			            System.Collections.IList children = this.Children;
			            for (int i = 0; i < children.Count; i++)
			            {
			                ICompiledEntity obj = (ICompiledEntity)children[i];
			                if (obj is BrowseForm || obj is EditForm || obj is FilterForm)
			                {
			                    if (obj is BrowseForm)
			                    {
			                        System.Collections.IList formChildren = (obj as BrowseForm).GetChildForms();
			                        for (int c = 0; c < formChildren.Count; c++)
			                        {
			                            ICompiledEntity formChild = (ICompiledEntity)formChildren[c];
			                            if (formChild is BrowseForm || formChild is EditForm || formChild is FilterForm)
			                            {
			                                ChildBrowsers.Add(formChild);
			                            }
			                        }
			                    }
			                    ChildBrowsers.Add(obj);
			                }
			            }
			            string[] fileNamesForCompile = new string[ChildBrowsers.Count];
			            for (int c = 0; c < ChildBrowsers.Count; c++)
			            {
			                string formCodeFileName = "";
			                string source = "";
			                BrowseForm bf = ChildBrowsers[c] as BrowseForm;
			                if (bf != null)
			                {
			                    formCodeFileName = AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + bf.GetFormName() + ".cs";
			                    source = bf.GenerateFormText();
			                }
			                EditForm ef = ChildBrowsers[c] as EditForm;
			                if (ef != null)
			                {
			                    formCodeFileName = AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + Static.GetCSharpObjectName(ef.Parent) + "EditForm.cs";
			                    source = ef.GenerateForm();
			                }
			                FilterForm ff = ChildBrowsers[c] as FilterForm; 
			                if (ff != null)
			                {
			                    formCodeFileName = AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + Static.GetCSharpObjectName(ff.Parent) + "FilterForm.cs";
			                    source = ff.GenerateForm();
			                }
			                StreamWriter SW = File.CreateText(formCodeFileName);
			                SW.Write(source);
			                SW.Flush();
			                SW.Close();
			                fileNamesForCompile[c] = formCodeFileName;
			            }
			
			            System.Collections.Specialized.StringCollection additionalReferences = new System.Collections.Specialized.StringCollection();
			
			            string[] referencedAssemblies = {"System.dll",
			                                            "System.Data.dll",
			                                            "System.Drawing.dll",
			                                            "System.Windows.Forms.dll",
			                                            "System.XML.dll",
			                                            AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "DIOS.Client.dll",
			                                            AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "DIOS.VisualLibrary.dll",
			                                            AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "DIOS.Interfaces.dll",
			                                            AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\Build\\" + "DIOS.Client.Interfaces.dll"
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
			            FileName = AppDomain.CurrentDomain.BaseDirectory + "TempFiles\\Download\\" + ModuleName + ".dll";
			            string zipName = AppDomain.CurrentDomain.BaseDirectory + "TempFiles\\" + ModuleName + ".zip";
			            results.TempFiles.Delete();
			            //for (int n = 0; n < fileNamesForCompile.Length; n++)
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
		#endregion
	}
}
