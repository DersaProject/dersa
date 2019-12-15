using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class ViewAttribute: StereotypeBaseE, ICompiledEntity
	{
		public ViewAttribute(){}

		public ViewAttribute(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String DIOSDefault = "";
		public System.String Get = "";
		public System.String PhisicalName = "";
		public System.String Set = "";
		public System.String Type = "int";
		public System.String DIOSType = "";
		public System.Boolean CreatePrivateField = true;

		#region Ìועמהû
		#region GetSqlName
		public System.String GetSqlName()
		{
string s = this.PhisicalName;
			if (s.Length > 0)
			{
				return s;
			}
			else
			{
				return this.Name;
			}
		}
		#endregion
		#region GetCSharpType
		public System.String GetCSharpType()
		{
string typeCSharp = this.DIOSType;
if (typeCSharp.Length > 0)
	return typeCSharp;
else
	return Static.GetCSharpType(this.Type);
		}
		#endregion
		#endregion
	}
}
