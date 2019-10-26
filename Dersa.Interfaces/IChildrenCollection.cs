using System;
using System.Collections;

namespace Dersa.Interfaces
{
	public interface IChildrenCollection: ICollection, IList
	{
		new IStereotypedObject this[int index]{get;set;}
	}
}
