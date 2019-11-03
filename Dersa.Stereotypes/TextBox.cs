using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Xml;
namespace DersaStereotypes
{
	[Serializable()]
	public class TextBox: FormControl, ICompiledEntity, IFilterControlWithPredicate
	{
		public TextBox(){}

		public TextBox(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string Caption = "";
		#region DataField
		public string DataField = "";
		public string dataField
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
		public bool Multiline;
		#region Predicate
		public string Predicate = "";
		public string predicate
		{
			get
			{
				return Predicate;
			}
			set
			{
				Predicate = value;
			}
		}
		#endregion

		#region Ìועמהû
		#region GenerateXmlNode
		public override XmlElement GenerateXmlNode(XmlDocument doc, string NamespaceUri)
		{
            base.GenerateXmlNode(doc, NamespaceUri);
            if (this.Multiline && this.Height > 0)
            {
                xmlNode.SetAttribute("Height", this.Height.ToString());
            }
            XmlElement filterBindNode = doc.CreateElement("TextBox.IsReadOnly", NamespaceUri);
            xmlNode.AppendChild(filterBindNode);
            XmlElement bindingControllerNode = doc.CreateElement("MultiBinding", NamespaceUri);
            bindingControllerNode.SetAttribute("Converter", "{StaticResource FilterController}");
            filterBindNode.AppendChild(bindingControllerNode);
            XmlElement bindingNode = doc.CreateElement("Binding", NamespaceUri);
            bindingNode.SetAttribute("RelativeSource", "{RelativeSource Mode=Self}");
            bindingNode.SetAttribute("Mode", "OneWay");
            bindingNode.SetAttribute("Path", "Tag");
            bindingControllerNode.AppendChild(bindingNode);
            bindingNode = doc.CreateElement("Binding", NamespaceUri);
            bindingNode.SetAttribute("RelativeSource", "{RelativeSource Mode=Self}");
            bindingNode.SetAttribute("Mode", "OneWay");
            bindingNode.SetAttribute("Path", "Text");
            bindingControllerNode.AppendChild(bindingNode);
            bindingNode = doc.CreateElement("Binding", NamespaceUri);
            bindingNode.SetAttribute("RelativeSource", "{RelativeSource Mode=Self}");
            bindingNode.SetAttribute("Mode", "OneWay");
            bindingNode.SetAttribute("Path", "IsEnabled");
            bindingControllerNode.AppendChild(bindingNode);
            bindingNode = doc.CreateElement("Binding", NamespaceUri);
            bindingNode.SetAttribute("RelativeSource", "{RelativeSource Mode=Self}");
            bindingNode.SetAttribute("Mode", "OneWay");
            bindingNode.SetAttribute("Path", "DataContext");
            bindingControllerNode.AppendChild(bindingNode);
            return xmlNode;
		}
		#endregion
		#region GenerateBaseXmlNode
		protected XmlElement GenerateBaseXmlNode(XmlDocument doc, string NamespaceUri)
		{
            base.GenerateXmlNode(doc, NamespaceUri);
            if (this.Multiline && this.Height > 0)
            {
                xmlNode.SetAttribute("Height", this.Height.ToString());
            }

            return xmlNode;
		}
		#endregion
		#endregion
	}
}
