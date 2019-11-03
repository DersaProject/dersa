using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Database: StereotypeBaseE, ICompiledEntity
	{
		public Database(){}

		public Database(IEntity obj)
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
