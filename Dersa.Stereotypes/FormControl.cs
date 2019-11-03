using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Xml;
namespace DersaStereotypes
{
	[Serializable()]
	public class FormControl: StereotypeBaseE, ICompiledEntity
	{
		public FormControl(){}

		public FormControl(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public bool ReadOnly = false;
		public XmlElement xmlNode;
		public string Enabler = "";
		public string EnablerProperty = "IsChecked";
		#region Height
		public int Height;
		public int height
		{
			get
			{
				return Height;
			}
			set
			{
				Height = value;
			}
		}
		#endregion
		#region Width
		public int Width;
		public int width
		{
			get
			{
				return Width;
			}
			set
			{
				Width = value;
			}
		}
		#endregion
		#region X
		public int X;
		public int x
		{
			get
			{
				return X;
			}
			set
			{
				X = value;
			}
		}
		#endregion
		#region Y
		public int Y;
		public int y
		{
			get
			{
				return Y;
			}
			set
			{
				Y = value;
			}
		}
		#endregion

		#region Ìועמהû
		#region GenerateXmlNode
		public virtual XmlElement GenerateXmlNode(XmlDocument doc, string NamespaceUri)
		{
            string typeName = this.GetType().Name;
            if(this.IsExtended())
                     typeName = "dios:" + typeName;
            xmlNode = doc.CreateElement(typeName, NamespaceUri);
            xmlNode.SetAttribute("Name", this.Name);
            xmlNode.SetAttribute("VerticalAlignment", "Top");
            xmlNode.SetAttribute("HorizontalAlignment", "Left");
            xmlNode.SetAttribute("Margin", this.X.ToString() + "," + this.Y.ToString() + ",0,0");
            xmlNode.SetAttribute("Width", this.Width.ToString());
            if (!string.IsNullOrEmpty(this.Enabler) && !string.IsNullOrEmpty(this.EnablerProperty))
            {
                xmlNode.SetAttribute("IsEnabled", "{Binding ElementName=" + this.Enabler + ", Path=" + this.EnablerProperty + "}");
            }
            IFilterControl FC = this as IFilterControl;
            if (FC != null && !string.IsNullOrEmpty(FC.dataField))
            {
                string DataField = FC.dataField;
                string Predicate = "=";
                IFilterControlWithPredicate FCP = this as IFilterControlWithPredicate;
                if (FCP != null && !string.IsNullOrEmpty(FCP.predicate))
                {
                    Predicate = FCP.predicate;
                }
                string Tag = "{}{\"DataField\":\"" + DataField + "\",\"Predicate\":\"" + Predicate + "\"}";
                xmlNode.SetAttribute("Tag", Tag);
            }
            return xmlNode;
		}
		#endregion
		#region IsExtended
		public virtual bool IsExtended()
		{
return false;
		}
		#endregion
		#endregion
	}
}
