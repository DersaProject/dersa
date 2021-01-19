using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class RadioGroup: StereotypeBaseE, ICompiledEntity
	{
		public RadioGroup(){}

		public RadioGroup(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
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

		#region Ìועמהû
		#region GenerateCS
		public string GenerateCS()
		{
return "";
		}
		#endregion
		#endregion
	}
}
