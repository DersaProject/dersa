using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class ProblemDefinition: StereotypeBaseE, ICompiledEntity
	{
		public ProblemDefinition(){}

		public ProblemDefinition(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string Description = "";

		#region Ìועמהû
		#endregion
	}
}
