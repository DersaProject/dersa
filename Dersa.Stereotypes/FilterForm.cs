using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Text;
using System.IO;
using System.Xml;
namespace DersaStereotypes
{
	[Serializable()]
	public class FilterForm: StereotypeBaseE, ICompiledEntity
	{
		public FilterForm(){}

		public FilterForm(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

		#region Методы
		#region GenerateJScript
		public string GenerateJScript(string proc_name, string[] paramNames)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("var form = new mxForm('параметры отчета');");
			sb.Append("var attrsArray = new Array();");
			sb.Append("var attrs = new Object();");
			sb.Append("attrs['Name']='proc_name';");
			sb.Append("attrs['Value']='" + proc_name + "';");
			sb.Append("attrs['WriteUnchanged']=true;");
			sb.Append("attrs['ReadOnly']=true;");
			sb.Append("attrs['Type']=-1;");
			sb.Append("attrs['ControlType']='text';");
			sb.Append("attrsArray[0] = attrs;");
			//int i = 1;
			for (int j = 0; j < paramNames.Length; j ++)
			{
			sb.Append("var attrs = new Object();");
			sb.Append("attrs['Name']='" + paramNames[j] + "';");
			sb.Append("attrs['Value']='';");
			sb.Append("attrs['ReadOnly']=false;");
			sb.Append("attrs['Type']=-1;");
			sb.Append("attrs['ControlType']='text';");
			sb.Append("attrsArray[" +  (j+1).ToString() + "] = attrs;");
			}
			sb.Append("var Props = CreateProperties(form, attrsArray, 'Query/ReportParams', 'exec');");
			sb.Append("var wnd = new mxWindow('параметры отчета',Props, 100, 100, 400, 400, false, true);");
			sb.Append("form.window = wnd;");
			sb.Append("wnd.setVisible(true);");
			
			
			return sb.ToString();
			
		}
		#endregion
		#region sql_Generate
		public System.String sql_Generate(System.Boolean dialog, System.Text.StringBuilder sb)
		{
BrowseForm browser = this.Parent as BrowseForm;
			if(browser == null)
				throw new Exception("Не определена форма просмотра.");
			string FormName = Static.GetCSharpObjectName(browser) + "FilterForm";
				
			string result = browser.GenerateSQL(dialog, FormName, 2, sb == null);
			if(sb != null)
				sb.Append(result);
			return result;	
		}
		#endregion
		#region GenerateForm
		public string GenerateForm()
		{
			string BrowserFormName = "ParentObjName";
						
						BrowseForm browser = this.Parent as BrowseForm;
						Package package = null;
						if(browser == null)
							throw new Exception("Не определена форма просмотра.");
						else
						{
							BrowserFormName = browser.GetFormName();
							package = browser.GetPackage();
						}
						string FormName = BrowserFormName.Replace("Browse", "Filter");
						string FileName = FormName + ".cs";
						FileName = package.GetDirectory() + "\\" + FileName;
						
						System.Text.StringBuilder sb = new System.Text.StringBuilder();
						
						sb.Append("using System;\n");
						sb.Append("using System.Windows.Forms;\n");
						sb.Append("using DIOS.Client.Browsing;\n");
						sb.Append("using DIOS.Common.Interfaces.Forms;\n");
						sb.Append("using DIOS.VisualLibrary.Controls;\n");
						sb.Append("using DIOS.Client.Data;\n");
						sb.Append("using DIOS.Common;\n\n"); 
						
						sb.Append("namespace ");
						if(package != null)
							sb.Append(package.Namespace);
						sb.Append("\n");   
						sb.Append("{\n");
						
						sb.Append("\tpublic class ");
						sb.Append(FormName);
						sb.Append(" : ObjFilterForm\n");
						sb.Append("\t{\n");
			            System.Collections.IList children = this.Children;
			            for (int i = 0; i < children.Count; i++)
			            {
			                ICompiledEntity obj = (ICompiledEntity)children[i];
			                if (obj is TextBox)
			                {
			                    sb.Append("\t\tprivate TextBoxEx " + obj.Name + ";\n");
			                }
			            }
			            sb.Append("\t\tprivate System.ComponentModel.Container components = null;\n");
						
						sb.Append("\t\tpublic ");
						sb.Append(FormName);
						sb.Append("()\n");
						
						sb.Append("\t\t{\n");
						sb.Append("\t\t\tInitializeComponent();\n");
						sb.Append("\t\t}\n\n\n\n");
			
			
			            sb.Append("\t\t#region Windows Form Designer generated code\n");
			            sb.Append("\t\tprivate void InitializeComponent()\n");
			            sb.Append("\t\t{\n");
			
			            for (int i = 0; i < children.Count; i++)
			            {
			                ICompiledEntity obj = (ICompiledEntity)children[i];
			                if (obj is TextBox)
			                {
			                    sb.Append("\t\t\tthis." + obj.Name + " = new DIOS.VisualLibrary.Controls.TextBoxEx();\n");
			                }
			            }
			            sb.Append("\t\t\tthis.SuspendLayout();\n");
			            for (int i = 0; i < children.Count; i++)
			            {
			                ICompiledEntity obj = (ICompiledEntity)children[i];
			                if (obj is TextBox)
			                {
			                    TextBox tb = obj as TextBox;
			                    sb.Append("\t\t\t//\n");
			                    sb.Append("\t\t\t// " + obj.Name + "\n");
			                    sb.Append("\t\t\t//\n");
			                    if (tb.Multiline)
			                        sb.Append("\t\t\tthis." + obj.Name + ".Multiline = true;\n");
			                    if (!string.IsNullOrEmpty(tb.Caption))
			                        sb.Append("\t\t\tthis." + obj.Name + ".Caption = \"" + tb.Caption + "\";\n");
			                    sb.Append("\t\t\tthis." + obj.Name + ".DataField = \"" + tb.DataField + "\";\n");
			                    sb.Append("\t\t\tthis." + obj.Name + ".Location = new System.Drawing.Point(" + tb.X.ToString() + ", " + tb.Y.ToString() + ");\n");
			                    sb.Append("\t\t\tthis." + obj.Name + ".Name = \"" + obj.Name + "\";\n");
			                    sb.Append("\t\t\tthis." + obj.Name + ".Size = new System.Drawing.Size(" + tb.Width.ToString() + ", " + tb.Height.ToString() + ");\n");
			                }
			            }
			
			            sb.Append("\t\t\t//\n");
			            sb.Append("\t\t\t//");
			            sb.Append(FormName);
			            sb.Append("\n\t\t\t//\n");
			            sb.Append("\t\t\tthis.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;\n");
			            for (int i = 0; i < children.Count; i++)
			            {
			                ICompiledEntity obj = (ICompiledEntity)children[i];
			                if (obj is TextBox)
			                {
			                    sb.Append("\t\t\tthis.Controls.Add(this." + obj.Name + ");\n");
			                    sb.Append("\t\t\tthis.Controls.SetChildIndex(this." + obj.Name + ", 0);\n");
			                }
			            }
			            sb.Append("\t\t\tthis.Name = \"");
			            sb.Append(FormName);
			            sb.Append("\";\n");
			            sb.Append("\t\t\tthis.Text = \"");
			            sb.Append(this.Name);
			            sb.Append("\";\n");
			            sb.Append("\t\t\tthis.ResumeLayout(false);\n");
			            sb.Append("\t\t\tthis.PerformLayout();\n");
			            sb.Append("\t\t}\n");
			            sb.Append("\t\t#endregion\n");
			            sb.Append("\t\t#region Methods\n");
			
			            System.Collections.IList meths = this.Children;
			            for (int i = 0; i < meths.Count; i++)
			            {
			                Method m = meths[i] as Method;
			                if(m != null)
			                    sb.Append(m.Generate());
			            }
			
			            sb.Append("\t\tprotected override void Dispose( bool disposing )\n");
						sb.Append("\t\t{\n");
						sb.Append("\t\t\tif( disposing )\n");
						sb.Append("\t\t\t{\n");
						sb.Append("\t\t\t\tif(components != null)\n");
						sb.Append("\t\t\t\t\tcomponents.Dispose();\n");
						sb.Append("\t\t\t}\n");
						sb.Append("\t\t\tbase.Dispose( disposing );\n");
						sb.Append("\t\t}\n");
						sb.Append("\t\t#endregion\n");
						
						
						sb.Append("\t}\n");
						sb.Append("}\n");
						
						return sb.ToString();
			
		}
		#endregion
		#region sqlGenerate
		public string sqlGenerate()
		{
            return sql_Generate(false, null);
			
		}
		#endregion
		#region GenerateXaml
		public string GenerateXaml()
		{
            string ElementsNamespaceUri = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
			            string DiosNamespace = "using:DIOS.Web.XamlExtensions";
			            XmlDocument doc = new XmlDocument();
			            string xmlFolder = AppDomain.CurrentDomain.BaseDirectory + "XmlTemplates\\";
			            doc.Load(xmlFolder + "FilterFormWebTemplate.xml");
			            XmlNamespaceManager xs = new XmlNamespaceManager(doc.NameTable);
			            xs.AddNamespace("l", ElementsNamespaceUri);
			            xs.AddNamespace("dios", DiosNamespace);
			            XmlNode gridNode = doc.SelectSingleNode(".//l:Grid", xs);
			            if (gridNode != null)
			            {
			                System.Collections.IList children = this.Children;
			                for (int i = 0; i < children.Count; i++)
			                {
			                    ICompiledEntity obj = (ICompiledEntity)children[i];
			                    if (obj is FormControl)
			                    {
			                        FormControl objWithCoords = obj as FormControl;
			                        string elementNamespace = ElementsNamespaceUri;
			                        if(objWithCoords.IsExtended())
			                                elementNamespace = DiosNamespace;
			                        XmlElement childElement = objWithCoords.GenerateXmlNode(doc, elementNamespace);
			                        gridNode.AppendChild(childElement);
			                    }
			                }
			            }
			            MemoryStream MS = new MemoryStream();
			            doc.Save(MS);
			            byte[] bts = new byte[MS.Length];
			            MS.Position = 0;
			            MS.Read(bts, 0, bts.Length);
			            return Encoding.Default.GetString(bts);
		}
		#endregion
		#endregion
	}
}
