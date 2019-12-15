using System;

namespace Dersa.Interfaces
{
	/// <summary>
	/// Summary description for IDiagramEntity.
	/// </summary>
	public interface IDiagramEntity: IDiagramObject
	{
		int X{get;}
		int Y{get;}
		int Width{get;}
		int Height{get;}
		IDersaEntity Entity{get;}
		int GetChildIndex(int entityId);
		IDiagramEntity Parent{get;}
		int ChildrenIndexOf(IDiagramEntity de);
		void InsertChildren();
		void DeleteChildren();
		IChildrenCollection Children{get;}
	}
}
