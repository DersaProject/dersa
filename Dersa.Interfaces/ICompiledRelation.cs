using System;

namespace Dersa.Interfaces
{
	public interface ICompiledRelation: ICompiled
	{
		ICompiledEntity A{get;}
		ICompiledEntity B{get;}
		void SetA(ICompiledEntity a);
		void SetB(ICompiledEntity b);
	}
}
