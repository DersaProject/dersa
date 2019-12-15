using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Xml;
namespace DersaStereotypes
{
	[Serializable()]
	public class ComboBox: FormControl, ICompiledEntity, IFilterControlWithPredicate
	{
		public ComboBox(){}

		public ComboBox(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
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
		public bool CanEdit;
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
		public string Items = "";

		#region Ìועמהû
		#region GenerateXmlNode
		public override XmlElement GenerateXmlNode(XmlDocument doc, string NamespaceUri)
		{
            base.GenerateXmlNode(doc, NamespaceUri);
            if(!string.IsNullOrEmpty(this.Items))
            {
                string[] itemsArray = Items.Split('\n');
                for(int i = 0; i < itemsArray.Length; i++)
                {
                    XmlElement itemNode = doc.CreateElement("ComboBoxItem", NamespaceUri);
                    itemNode.SetAttribute("Content", itemsArray[i]);
                    xmlNode.AppendChild(itemNode);
                }
            }
            if(this.CanEdit)
            {
                xmlNode.SetAttribute("IsEditable", "True");
            }
            XmlElement filterBindNode = doc.CreateElement("ComboBox.IsReadOnly", NamespaceUri);
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
		#endregion
	}
}
