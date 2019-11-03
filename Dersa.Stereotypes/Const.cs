using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Const: StereotypeBaseE, ICompiledEntity
	{
		public Const(){}

		public Const(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Value = "";
		public System.String Description = "";
		public System.String AccessModifier = "private";
		public System.String Type = "System.String";
		public System.String PhisicalName = "";

		#region Ìועמהû
		#region Generate
		public System.String Generate()
		{
return "\t\t" + AccessModifier + " " + Type + " " + GetName() + " = " + Value + ";\n";
		}
		#endregion
		#region GetName
		public System.String GetName()
		{
if ((PhisicalName == null)||(PhisicalName == ""))
			{
				return Static.GetCSharpName(Name);
			}
			return PhisicalName;
		}
		#endregion
		#endregion
	}
}
