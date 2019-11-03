using System;
using System.Collections;

namespace Dersa.Interfaces
{
	public interface IDersaEntity: IStereotypedObject
	{
		IDersaEntity Parent{get;}
		IChildrenCollection ARelations{get;}
		IChildrenCollection BRelations{get;}
		IChildrenCollection Children{get;} 
		IChildrenCollection Diagrams{get;} 
		int Index{get;}
		//int Rank{get;}
		int ParentId{get;}
		IChildrenCollection ARoles{get;}
		IChildrenCollection BRoles{get;}

		ICompiledEntity GetInstance();
		IList ARelationsInstance();
		IList BRelationsInstance();
		IList ChildrenInstance();

		//string GetProperty(string propName);
		//IChildrenCollection GetPath();
		//void Move(int parentID, bool toParent);
		//IDiagram CreateDiagram();
		//bool HasSubItems{get;}
		//IDersaEntity CloneWithChildren(int parentId);
		//void DropWithChildren();
	}
}
