using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace DersaStereotypes
{
	[Serializable()]
	public class SystemBase: StereotypeBaseE, ICompiledEntity
	{
		public SystemBase(){}

		public SystemBase(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

		#region Методы
		#region GetBaseObject
		public static StereotypeBaseE GetBaseObject(int id)
		{
    try
			    {
			        return StereotypeBaseE.GetSimpleInstance(id);
			    }
			    catch(Exception exc)
			    {
			         return null;
			    }
		}
		#endregion
		#region CompileStereotypes
		public string CompileStereotypes(object[] Params)
		{
            try
			            {
			                string FileName = AppDomain.CurrentDomain.BaseDirectory + "\\Build\\Dersa.Stereotypes.dll";
			                string DestFileName = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\Dersa.Stereotypes.dll";
			                string BackupFileName = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\bak\\Dersa.Stereotypes.dll";
			
			                string[] fileNamesForCompile = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Build\\Stereotypes", "*.cs");
			                System.Collections.Specialized.StringCollection additionalReferences = new System.Collections.Specialized.StringCollection();
			                string[] referencedAssemblies = {"System.dll",
			                                                        "System.Runtime.Serialization.dll",
			                                                        "System.Collections.dll",
			                                                        "System.Data.dll",
			                                                        "System.Core.dll",
			                                                        "System.Xml.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\Build\\" + "DIOS.Common.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\Build\\" + "DIOS.Interfaces.dll",
                                                                    AppDomain.CurrentDomain.BaseDirectory + "\\Build\\" + "Dersa.Interfaces.dll",
                                                                    AppDomain.CurrentDomain.BaseDirectory + "\\Build\\" + "DIOS.SqlManager.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\Build\\" + "ICSharpCode.SharpZipLib.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + "Newtonsoft.Json.dll",
			                                                        AppDomain.CurrentDomain.BaseDirectory + "\\Build\\" + "Dersa.Common.dll"
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
			                    string errDescr = errorSb.ToString();
			                    DIOS.Common.Logger.LogStatic(errDescr.Replace("\n", "\n\r***************************"));
			                    return "{alert('При компиляции возникли ошибки (см. лог).');}";
			                }
			                if (results != null)
			                    results.TempFiles.Delete();
			
			                File.Copy(DestFileName, BackupFileName, true);
			                File.Copy(FileName, DestFileName, true);
			                return "{alert('Библиотека стереотипов скомпилирована');}";
			            }
			            catch(Exception exc)
			            {
			                DIOS.Common.Logger.LogStatic(exc.Message);
			                return "{alert('Возникли ошибки (см. лог).');}";
			            }
		}
		#endregion
		#region DownloadStereotypes
		public FileInfo DownloadStereotypes()
		{
            string zipName = AppDomain.CurrentDomain.BaseDirectory + "Build\\Stereotypes.zip";
			            FastZip fZip = new FastZip();
			            fZip.CreateZip(zipName, AppDomain.CurrentDomain.BaseDirectory + "Build\\Stereotypes\\", false, "");
			            return new FileInfo(zipName);
		}
		#endregion
		#region AllowDrop
		public override bool AllowDrop()
		{
return false;
		}
		#endregion
		#endregion
	}
}
