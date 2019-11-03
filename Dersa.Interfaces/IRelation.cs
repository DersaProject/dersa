using System;

namespace Dersa.Interfaces
{
	public interface IRelation: IStereotypedObject
	{
		IDersaEntity A{get;}
		IDersaEntity B{get;}
		ICompiledRelation GetInstance();
		//void FinishCreation(int bId);
		//IDiagramRelation CreateDiagramRelation(int diagramId, int diagramEntityID);
	}
}
