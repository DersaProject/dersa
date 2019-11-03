using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Xml;
namespace DersaStereotypes
{
	[Serializable()]
	public class CheckBox: FormControl, ICompiledEntity, IFilterControl
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
		public System.String Caption = "";
		public bool ThreeState;

		#region Ìועמהû
		#region GenerateXmlNode
		public override XmlElement GenerateXmlNode(XmlDocument doc, string NamespaceUri)
		{
            base.GenerateXmlNode(doc, NamespaceUri);
            xmlNode.SetAttribute("Content", this.Caption);
            return xmlNode;


		}
		#endregion
		#endregion
	}
}
