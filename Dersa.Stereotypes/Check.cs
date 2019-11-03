using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Check: StereotypeBaseE, ICompiledEntity
	{
		public Check(){}

		public Check(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Description = "";
		public System.String Constraint = "";

		#region Ìועמהû
		#endregion
	}
}
