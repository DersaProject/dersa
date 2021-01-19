using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class RadioButton: StereotypeBaseE, ICompiledEntity
	{
		public RadioButton(){}

		public RadioButton(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string Value = "";
		public string Caption = "";
		public bool IsDefault;

		#region Ìועמהû
		#endregion
	}
}
