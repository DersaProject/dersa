using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Parameter: StereotypeBaseE, ICompiledEntity
	{
		public Parameter(){}

		public Parameter(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string Default = "";
		public System.String Sequence = "";

		#region Ìועמהû
		#endregion
	}
}
