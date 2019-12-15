using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class EditForm: StereotypeBaseE, ICompiledEntity
	{
		public EditForm(){}

		public EditForm(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

		#region Методы
		#region GenerateForm
		public string GenerateForm()
		{
            string cSharpName = "ParentObjName";

            BrowseForm browser = this.Parent as BrowseForm;
            Package package = null;
            if (browser == null)
                throw new Exception("Не определена форма просмотра.");
            else
            {
                cSharpName = Static.GetCSharpObjectName(browser);
                package = browser.GetPackage();
            }
            string FormName = cSharpName + "EditForm";
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
            if (package != null)
                sb.Append(package.Namespace);
            sb.Append("\n");
            sb.Append("{\n");

            sb.Append("\tpublic class ");
            sb.Append(FormName);
            sb.Append(" : ObjEditForm\n");
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
                    if(tb.ReadOnly)
                        sb.Append("\t\t\tthis." + obj.Name + ".ReadOnly = true;\n");
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
            //Static.SaveToFile(FileName, sb.ToString());
            //Console.WriteLine("Сформирован: " + FileName);

            /*
            using System;
            using WIBS.Common;
            using WIBS.Common.Interfaces;
            using WIBS.Common.Interfaces.Forms;
            using WIBS.VisualLibrary.Controls;
            using WIBS.Client.Data;
            using WIBS.Client.Browsing;
            using System.Windows.Forms;

            namespace WIBS.Modules.Projects
            {
                public class ProjRatingEditForm : ObjEditForm
                {
                    #region Implementations
                    private System.ComponentModel.Container components = null;
                    #endregion
                    public ProjRatingEditForm()
                    {
                        InitializeComponent();
                    }

                    #region Windows Form Designer generated code
                    private void InitializeComponent()
                    {
                        this.Name = "ProjRatingEditForm";
                        this.Text = "Оценка результата проекта";
                        this.ResumeLayout(false);
                    }
                    #endregion
                    #region Methods
                    protected override void Dispose( bool disposing )
                    {
                        if( disposing )
                        {
                            if(components != null)
                                components.Dispose();
                        }
                        base.Dispose( disposing );
                    }

                    #endregion
                }
            }*/

		}
		#endregion
		#region sql_Generate
		public System.String sql_Generate(System.Boolean dialog, System.Text.StringBuilder sb)
		{
BrowseForm browser = this.Parent as BrowseForm;
			if(browser == null)
				throw new Exception("Не определена форма просмотра.");
			string FormName = Static.GetCSharpObjectName(browser) + "EditForm";
				
			string result = browser.GenerateSQL(dialog, FormName, 3, sb == null);
			if(sb != null)
				sb.Append(result);
			return result;	
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
