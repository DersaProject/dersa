using System;

namespace Dersa.Interfaces
{
	public interface IStereotypedObject//: IBaseClass
	{
        //IStereotype Stereotype{get;}
        string StereotypeName { get; }
        int Rank { get; }
        string Name { get; }
        int Id { get; }
        ICompiled GetCompiledInstance();
		//IAttribute GetAttributeForView(string attributeName);
		//IAttribute GetAttributeForModify(string attributeName);
		//IChildrenCollection Operations{get;}
	}
}
