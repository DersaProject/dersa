using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace Dersa_SC
{
    public class ModuleCompiler
    {
        public static void CompileModule()
        {
            try
            {
                string DestFileName = AppDomain.CurrentDomain.BaseDirectory + "\\DersaSelfHost.dll";
                string backupFileName = AppDomain.CurrentDomain.BaseDirectory + "..\\Backup\\DersaSelfHost.dll";
                FileInfo fi = new FileInfo(DestFileName);
                FileInfo fiB = new FileInfo(backupFileName);
                if (fi.Exists)
                {
                    if(fiB.Exists)
                        fiB.MoveTo(backupFileName + "." 
                            + DateTime.Now.Year.ToString()
                            + DateTime.Now.Month.ToString()
                            + DateTime.Now.Day.ToString()
                            + DateTime.Now.Hour.ToString()
                            + DateTime.Now.Minute.ToString()
                            + DateTime.Now.Second.ToString());
                    fi.MoveTo(backupFileName);
                }

                List<string> fn = new List<string>(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\..\\Modules", "*.cs"));
                fn.Add(AppDomain.CurrentDomain.BaseDirectory + "\\..\\Startup.cs");
                string[] fileNamesForCompile = fn.ToArray();
                System.Collections.Specialized.StringCollection additionalReferences = new System.Collections.Specialized.StringCollection();
                string[] referencedAssemblies = {
                                                        "Microsoft.CSharp.dll",
                                                        "System.dll",
                                                        "System.Runtime.dll",
                                                        "System.Configuration.dll",
                                                        "System.Collections.dll",
                                                        "System.Data.dll",
                                                        "System.Core.dll",
                                                        "System.Xml.dll"};
                additionalReferences.AddRange(referencedAssemblies);
                additionalReferences.AddRange(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"));
                referencedAssemblies = new string[additionalReferences.Count];
                additionalReferences.CopyTo(referencedAssemblies, 0);

                System.CodeDom.Compiler.CompilerParameters cp = new System.CodeDom.Compiler.CompilerParameters(referencedAssemblies);
                cp.GenerateInMemory = false;

                cp.OutputAssembly = DestFileName;

                var codeCompiler = Microsoft.CSharp.CSharpCodeProvider.CreateProvider("CSharp");
                System.CodeDom.Compiler.CompilerResults results = codeCompiler.CompileAssemblyFromFile(cp, fileNamesForCompile);

                System.Type newObjectType = null;
                if ((results != null) && (results.Output.Count > 0))
                {
                    System.Text.StringBuilder errorSb = new System.Text.StringBuilder();
                    errorSb.Append("Errors\n");
                    for (int k = 0; k < results.Output.Count; k++)
                    {
                        errorSb.Append(results.Output[k] + "\n");
                    }
                    string errDescr = errorSb.ToString();
                    string logFileName = AppDomain.CurrentDomain.BaseDirectory + "..\\Log\\" + Guid.NewGuid().ToString();
                    using (StreamWriter SW = new StreamWriter(logFileName))
                    {
                        SW.Write(errDescr);
                    }

                    Console.WriteLine("При компиляции возникли ошибки. Информация сохранена в файл (см. папку Log)");
                    //DIOS.Common.Logger.LogStatic(errDescr.Replace("\n", "\n\r***************************"));
                    //return "{alert('При компиляции возникли ошибки (см. лог).');}";
                }
                else
                {
                    System.Reflection.Assembly assembly = results.CompiledAssembly;
                    newObjectType = assembly.GetType("Dersa_N.Program");
                }
                if (results != null)
                    results.TempFiles.Delete();
                if (newObjectType == null)
                {
                    if (!fiB.Exists)
                        Console.WriteLine("Бэкапа нет");
                    else
                    {
                        fiB.CopyTo(DestFileName);
                        System.Reflection.Assembly bAsm = Assembly.LoadFrom(DestFileName);
                        if (bAsm != null)
                        {
                            newObjectType = bAsm.GetType("Dersa_N.Program");
                            Console.WriteLine("Загрузили сборку из бэкапа");
                        }
                        else
                            Console.WriteLine("Не удалось загрузить сборку из бэкапа");
                    }
                }
                if (newObjectType != null)
                {
                    MethodInfo method = null;
                    MethodInfo[] methods = newObjectType.GetMethods(BindingFlags.Static | BindingFlags.Public);
                    foreach (MethodInfo m in methods)
                    {
                        if (m.Name == "Run")
                        {
                            method = m;
                            break;
                        }
                    }
                    if (method != null)
                    {
                        method.Invoke(null, new object[0]);
                        //new FileInfo(DestFileName).Delete();
                    }
                }
                else
                {
                    Console.WriteLine("Не удалось запустить программу");
                    Console.ReadKey();
                }
                //File.Copy(DestFileName, BackupFileName, true);
                //File.Copy(FileName, DestFileName, true);
                //return "{alert('Библиотека стереотипов скомпилирована');}";
            }
            catch (Exception exc)
            {
                while (exc.InnerException != null)
                    exc = exc.InnerException;
                Console.WriteLine(exc.Message);
                Console.ReadLine();
                //DIOS.Common.Logger.LogStatic(exc.Message);
                //return "{alert('Возникли ошибки (см. лог).');}";
            }
        }
    }
}
