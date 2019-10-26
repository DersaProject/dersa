using System;
using System.Collections;

namespace Dersa.Interfaces
{
	public interface ICompiledEntity: ICompiled
	{
		ICompiledEntity Parent{get;}
		IList ARelations{get;}
		IList BRelations{get;}
		IList Children{get;} 
		void SetParent(ICompiledEntity parent);
		void SetARelations(IList aRelations);
		void SetBRelations(IList aRelations);
		void SetChildren(IList children);
	}
}
