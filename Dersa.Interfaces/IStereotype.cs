using System;

namespace Dersa.Interfaces
{
	public interface IStereotype//: IBaseClass
	{
		StereotypeType StereotypeType{get;}
		//IChildrenCollection Children{get;}
		//IChildrenCollection Operations{get;}
		//IStereotype Parent{get;}
		//IOperation GetOperation(string theName);
		bool IsAbstract();
		string Extends{get;}
		//byte[]ImageBytes{get;set;}
		//void Move(int parentId);
		//IChildrenCollection GetPath();
		ArrowType ArrowTypeA{get;}
		ArrowType ArrowTypeB{get;}
		//IDersaEntity CreateEntity(int parentId);
		//IRelation CreateRelation(int aID);
		//IAttribute CreateAttribute();
		//IOperation CreateOperation();
		string ViewFormat{get;}
		//string DiagramViewFormat{get;}
		int Rank{get;}
	}
}
