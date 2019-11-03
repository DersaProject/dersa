using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;
using System.Text;


namespace DersaStereotypes
{
	[Serializable()]
	public class BrowseForm: StereotypeBaseE, ICompiledEntity
	{
		public BrowseForm(){}

		public BrowseForm(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String PhisicalName = "";
		public System.String ManualFormName = "";

		#region Методы
		#region GenerateXamlFormInitializer
		public string GenerateXamlFormInitializer()
		{
StringBuilder sb = new StringBuilder();
bool redefineColumns = false;
string objectName = this.PhisicalName;
Entity objectEntity = null;
System.Collections.IList attrs = null;

System.Collections.IList relations = this.ARelations;
for (int i = 0; i < relations.Count; i++)
{
	ICompiledRelation rel = (ICompiledRelation)relations[i];
	if (rel is Relation && rel.B is Entity)
	{
             redefineColumns = true;
             objectEntity = (rel.B as Entity);
             objectName = objectEntity.GetSqlName();
             attrs = objectEntity.GetAttributes();
             break;
        }
}

sb.Append("using System;\r\n");
sb.Append("using System.Collections.Generic;\r\n");
sb.Append("using System.Linq;\r\n");
sb.Append("using System.Text;\r\n");
sb.Append("using System.Threading.Tasks;\r\n");
sb.Append("using System.ComponentModel;\r\n");
sb.Append("using Windows.UI.Xaml.Controls;\r\n");
sb.Append("using Windows.UI.Xaml.Data;\r\n");
sb.Append("using DIOS.Web.XamlExtensions;\r\n\r\n");


sb.Append("namespace GridOrganizer\r\n");
sb.Append("{\r\n");
sb.Append("    public class " + objectName + "Struct : DIOSObject\r\n");
sb.Append("    {\r\n");
if(redefineColumns)
{
     for (int i = 0; i < attrs.Count; i++)
     {
         string attrName = (attrs[i] as Attribute).GetSqlName();
         if(attrName == "name")
               attrName = "_name";
         sb.Append("        public string " + attrName +  " { get; set; }\r\n");
     }
}
else
    sb.Append("        public string " + objectName.ToLower() +  " { get; set; }\r\n");
sb.Append("    }\r\n\r\n");

sb.Append("    class " + objectName + " : PageInitializer\r\n");
sb.Append("    {\r\n");
sb.Append("        public override string ObjectClassName\r\n");
sb.Append("        {\r\n");
sb.Append("            get\r\n");
sb.Append("            {\r\n");
sb.Append("                return \"" + objectName + "\";\r\n");
sb.Append("            }\r\n");
sb.Append("        }\r\n");
sb.Append("        public override INotifyPropertyChanged GetObjectCollection(string Where)\r\n");
sb.Append("        {\r\n");
sb.Append("            DIOSObjectCollection<" + objectName + "Struct> col = new DIOSObjectCollection<" + objectName + "Struct>(ObjectClassName, Where, \"\");\r\n");
sb.Append("            this.objectCollection = col;\r\n");
sb.Append("            return col;\r\n");
sb.Append("        }\r\n\r\n");

sb.Append("        public override bool GenerateColumns(DataGrid dataGrid)\r\n");
sb.Append("        {\r\n");

if(redefineColumns)
{
     sb.Append("            Binding colBinding = null;\r\n\r\n");

     for (int i = 0; i < attrs.Count; i++)
     {
         string sqlName = (attrs[i] as Attribute).GetSqlName();
         if(sqlName == "name")
               sqlName = "_name";
         string interfaceName = (attrs[i] as Attribute).Name;
         sb.Append("            DataGridTextColumn  " + sqlName +  "Column = new DataGridTextColumn();\r\n");
         sb.Append("            " + sqlName +  "Column.Header = \"" + interfaceName + "\";\r\n");
         sb.Append("            colBinding = new Binding(\"" + sqlName +  "\");\r\n");
         sb.Append("            dataGrid.Columns.Add(" + sqlName +  "Column);\r\n");
         sb.Append("            " + sqlName +  "Column.SetBinding(DataGridBoundColumn.BindingProperty, colBinding);\r\n\r\n");
     }

     sb.Append("            return true;\r\n");
}
else
{
sb.Append("            return false;\r\n");
}
sb.Append("        }\r\n");
sb.Append("    }\r\n");
sb.Append("}\r\n");

return sb.ToString();

		}
		#endregion
		#region GetChildForms
		public System.Collections.IList GetChildForms()
		{
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
            return ChildBrowsers;
		}
		#endregion
		#region GenerateFormText
		public virtual string GenerateFormText()
		{
            string FormName = this.GetFormName();
            //string FileName = FormName + ".cs";

            Package package = this.GetPackage();

            //FileName = package.GetDirectory() + "\\" + FileName;
            //FileName = AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\" + FileName;


            System.Collections.IList ChildBrowsers = new Map();
            System.Collections.IList children = this.Children;
            for (int i = 0; i < children.Count; i++)
            {
                ICompiledEntity obj = (ICompiledEntity)children[i];
                if (obj is BrowseForm)
                {
                    ChildBrowsers.Add(obj);
                }
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("using System;\n");
            sb.Append("using System.Windows.Forms;\n");
            sb.Append("using DIOS.Client.Browsing;\n");
            sb.Append("using DIOS.Common.Interfaces.Forms;\n");
            sb.Append("using DIOS.VisualLibrary.Controls;\n");
            sb.Append("using DIOS.VisualLibrary.Docking;\n");
            sb.Append("using DIOS.Client.Data;\n");
            sb.Append("using DIOS.Common;\n\n");

            sb.Append("namespace ");
            if (package != null)
                sb.Append(package.Namespace);
            sb.Append("\n");
            sb.Append("{\n");

            sb.Append("\tpublic class ");
            sb.Append(FormName);
            sb.Append(" : GridBrowser\n");
            sb.Append("\t{\n");
            sb.Append("\t\tprivate System.ComponentModel.Container components = null;\n");

            sb.Append("\t\tpublic ");
            sb.Append(FormName);
            sb.Append("()\n");

            sb.Append("\t\t{\n");
            sb.Append("\t\t\tInitializeComponent();\n");
            sb.Append("\t\t\tif (!this.DesignMode)\n");
            sb.Append("\t\t\t{\n");
            sb.Append("\t\t\t\tthis.Obj = new Obj(\"");
            sb.Append(this.PhisicalName);
            sb.Append("\");\n");
            sb.Append("\t\t\t}\n");
            sb.Append("\t\t}\n\n\n\n");


            sb.Append("\t\t#region Windows Form Designer generated code\n");
            sb.Append("\t\tprivate void InitializeComponent()\n");
            sb.Append("\t\t{\n");
            sb.Append("\t\t\t//\n");
            sb.Append("\t\t\t//");
            sb.Append(FormName);
            sb.Append("\n\t\t\t//\n");
            sb.Append("\t\t\tthis.components = new System.ComponentModel.Container();\n");
            sb.Append("\t\t\tthis.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;\n");
            sb.Append("\t\t\tthis.Name = \"");
            sb.Append(FormName);
            sb.Append("\";\n");
            sb.Append("\t\t\tthis.Text = \"");
            sb.Append(this.Name);
            sb.Append("\";\n");
            sb.Append("\t\t}\n");
            sb.Append("\t\t#endregion\n\n");
            sb.Append("\t\t#region Methods\n\n");

            if (ChildBrowsers.Count > 0)
            {
                sb.Append("\t\tprotected override void OnAddContentsAllways()\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\tbase.OnAddContentsAllways();\n");

                for (int c = 0; c < ChildBrowsers.Count; c++)
                {
                    BrowseForm bf = ChildBrowsers[c] as BrowseForm;
                    string bfName = Static.GetCSharpObjectName(bf) + "_form";
                    sb.Append("\n\t\t\tGridBrowser ");
                    //sb.Append(bf.GetFormName());
                    //            GridBrowser IgsAbonentStateFlag_form = ClientFormManager.GetForm("IGS_ABONENT_STATE_FLAG", FormType.ftBrowse) as GridBrowser;

                    //sb.Append(" ");
                    sb.Append(bfName);
                    //sb.Append(" = new ");
                    sb.Append(" = ClientFormManager.GetForm(\"");
                    sb.Append(bf.PhisicalName);
                    sb.Append("\", FormType.ftBrowse) as GridBrowser;\n");
                    //sb.Append("();\n");
                    sb.Append("\t\t\tif (");
                    sb.Append(bfName);
                    sb.Append(" != null)\n");
                    sb.Append("\t\t\t{\n");
                    sb.Append("\t\t\t\t");
                    sb.Append(bfName);
                    sb.Append(".EnableEditForm = true;\n");
                    sb.Append("\t\t\t\t");
                    sb.Append(bfName);
                    sb.Append(".Obj.DependsOn(this.Obj);\n");
                    sb.Append("\t\t\t\tContent c = manager.Contents.Add(");
                    sb.Append(bfName);
                    sb.Append(", ");
                    sb.Append(bfName);
                    sb.Append(".Text);\n");
                    sb.Append("\t\t\t\tthis.manager.AddContentWithState(c, State.DockRight);\n");
                    sb.Append("\t\t\t}\n");
                }
                sb.Append("\t\t}\n");
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
		#region Test
		public void Test()
		{
string cSharpName = Static.GetCSharpObjectName(this);
			string FormName = cSharpName + "BrowseForm";
			string FileName = FormName + ".cs";
			
			ICompiledEntity parent = this;
			while(parent != null && parent as Package == null)
			{
				parent = parent.Parent;
			}
			Package package = parent as Package;
			
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
			sb.Append("\n");   //DIOS.Modules.Printing
			sb.Append("{\n");
			
			sb.Append("\tpublic class ");
			sb.Append(FormName);
			sb.Append(" : GridBrowser\n");
			sb.Append("\t{\n");
			sb.Append("\t\tprivate System.ComponentModel.Container components = null;\n");
			
			sb.Append("\t\tpublic ");
			sb.Append(FormName);
			sb.Append("()\n");
			
			sb.Append("\t\t{\n");
			sb.Append("\t\t\tInitializeComponent();\n");
			sb.Append("\t\t\tif (!this.DesignMode)\n");
			sb.Append("\t\t\t{\n");
			sb.Append("\t\t\t\tthis.Obj = new Obj(\"");
			sb.Append(this.PhisicalName);
			sb.Append("\");\n");
			sb.Append("\t\t\t}\n");
			sb.Append("\t\t}\n\n\n\n");
			
			
			sb.Append("\t\t#region Windows Form Designer generated code\n");
			sb.Append("\t\tprivate void InitializeComponent()\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t//\n"); 
			sb.Append("\t\t//"); 
			sb.Append(FormName);
			sb.Append("\n\t\t//\n"); 
			sb.Append("\t\t\tthis.components = new System.ComponentModel.Container();\n");
			sb.Append("\t\t\tthis.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;\n");
			sb.Append("\t\t\tthis.Name = \"");
			sb.Append(FormName);
			sb.Append("\";\n");
			sb.Append("\t\t\tthis.Text = \"");
			sb.Append(this.Name);
			sb.Append("\";\n");
			sb.Append("\t\t}\n");
			sb.Append("\t\t#endregion\n");
			sb.Append("\t\t#region Methods\n");
			
			
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
			//Console.WriteLine(sb.ToString());
			
			Static.SaveToFile(FileName, sb.ToString());
			Console.WriteLine("Сформирован: " + FileName);
			
		}
		#endregion
		#region GenerateForm
		public FileInfo GenerateForm()
		{
            string FormName = this.GetFormName();
            string FileName = FormName + ".cs";
            FileName = AppDomain.CurrentDomain.BaseDirectory + "\\TempFiles\\" + FileName;
            string formCode = GenerateFormText().Replace("WIBS", "DIOS");
            return Static.SaveToFile(FileName, formCode);
		}
		#endregion
		#region GenerateSQL
		public System.String GenerateSQL(System.Boolean dialog, string FormName, System.Int32 FormType, System.Boolean DoDeclareVars)
		{
string AssemblyName = this.GetNameSpace();
			string TypeName = AssemblyName + "." + FormName;
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if(DoDeclareVars)
				sb.Append("declare @object_type int, @form int\n");	
			sb.Append("select @object_type = object_type from OBJECT_TYPE where class_name = '");
			sb.Append(this.PhisicalName);
			sb.Append("'\n\n"); 
			sb.Append("if @object_type is null \n\traiserror('Не найден класс', 16, -1)\n");
			sb.Append("else begin\n");
			sb.Append("\tselect @form = null\n");
			sb.Append("\tselect @form = form from FORM where assembly_name = '");
			sb.Append(AssemblyName);
			sb.Append("' and type_name = '");
			sb.Append(TypeName);
			sb.Append("'\n");
			sb.Append("\tif @form is null begin\n");
			sb.Append("\t\tselect @form = max(form) + 1 from FORM\n");
			sb.Append("\t\tinsert FORM(form, object_type, is_default, assembly_name, type_name, form_type)\n");
			sb.Append("\t\tvalues (@form, @object_type, 1, '");
			sb.Append(AssemblyName);
			sb.Append("', '");
			sb.Append(TypeName);
			sb.Append("', ");
			sb.Append(FormType.ToString());
			sb.Append(")\n");
			sb.Append("\tend\n");
			sb.Append("end\n");
			
			string result = sb.ToString();
			//if (dialog)
			//{
			//	SqlExecForm.Exec(result, "", "DIOS_common");
			//	return null;
			//}
			/*
			else
			{
				Console.WriteLine(result);
			}*/
			return result;
		}
		#endregion
		#region GetNameSpace
		public System.String GetNameSpace()
		{
Package p = this.GetPackage();
			if(p != null)
				return p.Namespace;
			return "Undefined Namespace";
		}
		#endregion
		#region GetPackage
		public Package GetPackage()
		{
ICompiledEntity parent = this;
			while(parent != null && parent as Package == null)
			{
				parent = parent.Parent;
			}
			return parent as Package;
			
		}
		#endregion
		#region sql_Generate
		public System.String sql_Generate(System.Boolean dialog, System.Text.StringBuilder sb)
		{
string FormName = GetFormName();
			string result = this.GenerateSQL(dialog, FormName, 1, sb == null);
			if(sb != null)
				sb.Append(result);
			return result;
		}
		#endregion
		#region sql_GenerateForChilds
		public void sql_GenerateForChilds(System.Text.StringBuilder sb)
		{
System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity f = (ICompiledEntity)children[i];
				if (f is BrowseForm)
				{
					BrowseForm browser = f as BrowseForm;
					browser.sql_Generate(false, sb);
					browser.sql_GenerateForChilds(sb);
				}
				if (f is EditForm)
				{
					(f as EditForm).sql_Generate(false, sb);
				}
				if (f is FilterForm)
				{
					(f as FilterForm).sql_Generate(false, sb);
				}
			}
		}
		#endregion
		#region GetFormName
		public System.String GetFormName()
		{
if(ManualFormName != string.Empty)
				return ManualFormName;
			string cSharpName = Static.GetCSharpObjectName(this);
			string FormName = cSharpName + "BrowseForm";
			return FormName;
		}
		#endregion
		#region sqlGenerate
		public string sqlGenerate()
		{
            return sql_Generate(true, null);
		}
		#endregion
		#endregion
	}
}
