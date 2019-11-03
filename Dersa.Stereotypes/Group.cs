using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Group: StereotypeBaseE, ICompiledEntity
	{
		public Group(){}

		public Group(IEntity obj)
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
