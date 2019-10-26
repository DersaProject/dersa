using System;
using System.Collections;

namespace Dersa.Interfaces
{
	public interface IUserContext: IDisposable, IDictionary
	{
		Guid Guid{get;}
		//IUserSponsor UserSponsor{get;}
		//IUserSponsor UserContextSponsor{get;}
		string UserName{get;} 
		string Password{get;} 
		string ApplicationName{get;}
		string UserHostName{get;}
		DateTime LastAccess{get;}
		AuthentificationType AuthentificationType {get;}
		void Notify(string message);
		void RegisterForLeasing(Guid contextGuid);
		void UnregisterForLeasing(Guid contextGuid);
		void Deauthentificate();
		//System.DirectoryServices.DirectoryEntry GetLdapInfo();
	}
}
