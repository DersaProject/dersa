using System;

namespace Dersa.Interfaces
{
	public interface IObject: IComparable, IDisposable, ICloneable
	{
		int Id{get;}
		string NativeName{get;}
		string Changer{get;}
		DateTime Occur{get;}
		//void New();
		//void Load();
		//void Drop();
		//void Post();
		//void Cancel();
		object this[string propertyName]{get;set;}
		void SetProperty(string propertyName, object propertyValue);
		string ToDisplayString();
		bool IsModified{get;}
	}
}
