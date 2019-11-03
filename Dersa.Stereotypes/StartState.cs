using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class StartState: StereotypeBaseE, ICompiledEntity
	{
		public StartState(){}

		public StartState(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Value = "";
		public System.String StateName = "";

		#region Ìועמהû
		#endregion
	}
}
