using System;

namespace Dersa.Interfaces
{
	public interface IBaseObject: IObject
	{
		string Name{get;}
		ObjectState ObjectState{get;}
	}
}
