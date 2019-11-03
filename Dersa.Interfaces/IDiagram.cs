using System;

namespace Dersa.Interfaces
{
	/// <summary>
	/// Summary description for IDiagram.
	/// </summary>
	public interface IDiagram: IBaseObject
	{
		IChildrenCollection DiagramEntities{get;}
		IChildrenCollection DiagramRelations{get;}
		IDersaEntity Entity{get;}
		IChildrenCollection CreateRelationsForDiagramEntity(int diagramEntityId);
		IDiagramEntity CreateDiagramEntity(int entityId, int diagramEntityID, int X, int Y);
		IDiagramEntity CreateUpLevelDiagramEntity(int entityId, int X, int Y);
		IDiagramObject FindRepresentation(IStereotypedObject sObj);
		void Move(int parentID);
	}
}
