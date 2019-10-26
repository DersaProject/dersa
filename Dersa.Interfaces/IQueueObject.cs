using System;

namespace Dersa.Interfaces
{
	public interface IQueueObject
	{
		string ObjectType{get; set;}
		int ObjectID{get; set;}
		Property[] Properties{get; set;}
		object GetProperty(PropertyKey key);
		ObjectOperation Operation{get; set;}
	}
}
