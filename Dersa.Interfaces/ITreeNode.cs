using System;

namespace Dersa.Interfaces
{
	public interface ITreeNode
	{
		string Name{get;set;}
		int Id{get;}
        IChildrenCollection Children { get; }
    }
}
