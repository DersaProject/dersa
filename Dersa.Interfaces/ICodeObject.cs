using System;

namespace Dersa.Interfaces
{
	public interface ICodeObject: IObject 
	{
		string Code{get;}
		IBaseClass BaseClass{get;}
		ValueType ValueType{get;}
	}
}
