using System;

namespace Dersa.Interfaces
{
	public interface IRelation: IStereotypedObject
	{
		IEntity A{get;}
		IEntity B{get;}
		ICompiledRelation GetInstance();
		//void FinishCreation(int bId);
		//IDiagramRelation CreateDiagramRelation(int diagramId, int diagramEntityID);
	}
}
