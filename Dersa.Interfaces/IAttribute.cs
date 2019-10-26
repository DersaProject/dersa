using System;

namespace Dersa.Interfaces
{
	public interface IAttribute: IBaseObject, ICodeObject  
	{
		string Value{get;}
		string Type{get;}
		string AccessModifier{get;}
		object GetValue();
	}
}
