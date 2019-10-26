using System;

namespace Dersa.Interfaces
{
	public interface IBaseClass: IBaseObject
	{
		//new object this[string attributeName]{get;}
		IAttribute GetAttribute(int ID);
		IAttribute GetAttribute(string attributeName);
		IChildrenCollection Attributes{get;}
	}
}
