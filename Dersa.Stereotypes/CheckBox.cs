using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Xml;
namespace DersaStereotypes
{
	[Serializable()]
	public class CheckBox: FormControl, ICompiledEntity, IEditControl
	{
		public CheckBox(){}

		public CheckBox(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public bool ThreeState;
		public System.String Caption = "";
		#region DataField
		public System.String DataField = "";
		public System.String dataField
		{
			get
			{
				return DataField;
			}
			set
			{
				DataField = value;
			}
		}
		#endregion

		#region Ìועמהû
		#region GenerateCS
		public string GenerateCS()
		{
		    System.Text.StringBuilder sb = new System.Text.StringBuilder();
			            sb.Append("\t\t\t//\r\n");
			            sb.Append("\t\t\t// " + this.Name + "\r\n");
			            sb.Append("\t\t\t//\r\n");
			            sb.Append("\t\t\tthis." + this.Name + " = new CheckBoxEx();\r\n");
			            if (!string.IsNullOrEmpty(this.Caption))
			                sb.Append("\t\t\tthis." + this.Name + ".Text = \"" + this.Caption + "\";\r\n");
			            sb.Append("\t\t\tthis." + this.Name + ".DataField = \"" + this.DataField + "\";\r\n");
			            if(this.ReadOnly)
			                sb.Append("\t\t\tthis." + this.Name + ".ReadOnly = true;\r\n");
			            sb.Append("\t\t\tthis." + this.Name + ".Location = new System.Drawing.Point(" + this.X.ToString() + ", " + this.Y.ToString() + ");\r\n");
			            sb.Append("\t\t\tthis." + this.Name + ".Name = \"" + this.Name + "\";\r\n");
			            sb.Append("\t\t\tthis." + this.Name + ".Size = new System.Drawing.Size(" + this.Width.ToString() + ", 25);\r\n");
					    sb.Append("\t\t\tthis.Controls.Add(this." + this.Name + ");\r\n");
					    sb.Append("\t\t\tthis.Controls.SetChildIndex(this." + this.Name + ", 0);\r\n");
			
			            return sb.ToString();
		}
		#endregion
		#region GenerateXmlNode
		public XmlElement GenerateXmlNode(XmlDocument doc, string NamespaceUri)
		{
            base.GenerateXmlNode(doc, NamespaceUri);
			            xmlNode.SetAttribute("Content", this.Caption);
			            return xmlNode;
			
			
		}
		#endregion
		#region CSTypeName
		public string CSTypeName()
		{
return "DIOS.VisualLibrary.Controls.CheckBoxEx";
		}
		#endregion
		#endregion
	}
}
