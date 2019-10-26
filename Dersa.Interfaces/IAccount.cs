using System;

namespace Dersa.Interfaces
{
	/// <summary>
	/// Summary description for IAccount.
	/// </summary>
	public interface IAccount: IBaseObject
	{
		AccountType Type{get;}
	}
}
