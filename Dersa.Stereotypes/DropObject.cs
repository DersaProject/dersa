using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class DropObject: StereotypeBaseE, ICompiledEntity
	{
		public DropObject(){}

		public DropObject(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

		#region Ìועמהû
		#endregion
	}
}
