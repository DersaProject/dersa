using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Xml;
namespace DersaStereotypes
{
	[Serializable()]
	public class ListBox: FormControl, ICompiledEntity, IFilterControlWithPredicate
	{
		public ListBox(){}

		public ListBox(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public int MinHeight = 64;
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
		public bool Multiselect;
		public string Items = "";

		#region Ìועמהû
		#region GenerateXmlNode
		public override XmlElement GenerateXmlNode(XmlDocument doc, string NamespaceUri)
		{
            base.GenerateXmlNode(doc, NamespaceUri);
            if(this.Height > 0)
            {
                xmlNode.SetAttribute("Height", this.Height.ToString());
            }
            if(!string.IsNullOrEmpty(this.Items))
            {
                string[] itemsArray = Items.Split('\n');
                for(int i = 0; i < itemsArray.Length; i++)
                {
                    XmlElement itemNode = doc.CreateElement("ListBoxItem", NamespaceUri);
                    itemNode.SetAttribute("Content", itemsArray[i]);
                    xmlNode.AppendChild(itemNode);
                }
            }
            return xmlNode;
		}
		#endregion
		#endregion
	}
}
