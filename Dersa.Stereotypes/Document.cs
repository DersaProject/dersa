using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Document: StereotypeBaseE, ICompiledEntity
	{
		public Document(){}

		public Document(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Text = "";

		#region Ìועמהû
		#endregion
	}
}
