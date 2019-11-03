using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Xml;
namespace DersaStereotypes
{
	[Serializable()]
	public class TextBoxEx: TextBox, ICompiledEntity, IFilterControlWithPredicate
	{
		public TextBoxEx(){}

		public TextBoxEx(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

		#region Ìועמהû
		#region GenerateXmlNode
		public override XmlElement GenerateXmlNode(XmlDocument doc, string NamespaceUri)
		{
            GenerateBaseXmlNode(doc, NamespaceUri);
            if (this.Multiline && this.Height > 0)
            {
                xmlNode.SetAttribute("Height", this.Height.ToString());
            }
            xmlNode.SetAttribute("Text", "{Binding FilterControllerValue, Mode=TwoWay, RelativeSource={RelativeSource Mode=Self}}");
            return xmlNode;
		}
		#endregion
		#region IsExtended
		public override bool IsExtended()
		{
return true;
		}
		#endregion
		#endregion
	}
}
