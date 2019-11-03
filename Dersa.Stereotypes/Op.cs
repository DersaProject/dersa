using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Op: StereotypeBaseE, ICompiledEntity
	{
		public Op(){}

		public Op(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public String SQL;

		#region Ìועמהû
		#endregion
	}
}
