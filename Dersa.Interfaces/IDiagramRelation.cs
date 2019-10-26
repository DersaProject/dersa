using System;

namespace Dersa.Interfaces
{
	/// <summary>
	/// Summary description for IDiagramRelation.
	/// </summary>
	public interface IDiagramRelation: IDiagramObject
	{
		IRelation Relation{get;}
		IDiagramEntity DiagramEntityA{get;}
		IDiagramEntity DiagramEntityB{get;}
		IPoint[] Points{get;set;}
		string[] Hints();
		void FinishCreation(int bId);
	}
}
