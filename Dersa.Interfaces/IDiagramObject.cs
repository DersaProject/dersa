using System;

namespace Dersa.Interfaces
{
	/// <summary>
	/// Summary description for IDiagramObject.
	/// </summary>
	public interface IDiagramObject: IObject
	{
		IDiagram Diagram{get;}
	}
}
