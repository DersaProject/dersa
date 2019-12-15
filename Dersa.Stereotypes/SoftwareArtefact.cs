using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class SoftwareArtefact: StereotypeBaseE, ICompiledEntity
	{
		public SoftwareArtefact(){}

		public SoftwareArtefact(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string Type = "";
		public string Description = "";

		#region Ìועמהû
		#endregion
	}
}
