using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class DBMS: StereotypeBaseE, ICompiledEntity
	{
		public DBMS(){}

		public DBMS(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String ReplData = "";

		#region Ìועמהû
		#endregion
	}
}
