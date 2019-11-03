using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Configuration: StereotypeBaseE, ICompiledEntity
	{
		public Configuration(){}

		public Configuration(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public bool Active = true;

		#region Ìועמהû
		#region GetSetting
		public string GetSetting(string settingName)
		{
System.Collections.IList children = this.Children;
for (int i = 0; i < children.Count; i++)
{
	ICompiledEntity obj = (ICompiledEntity)children[i];
	if ((obj is Const)&&(obj.Name == settingName))
	{
		return ((Const)obj).Value;
	}
}
return children.Count.ToString();
		}
		#endregion
		#endregion
	}
}
