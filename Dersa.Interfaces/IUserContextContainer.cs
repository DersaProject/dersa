using System;

namespace Dersa.Interfaces
{
	[Serializable]
	public class UserContextEventArgs: EventArgs
	{
		public UserContextEventArgs(IUserContext userContext) 
		{
			_userContext = userContext;
		}
		private IUserContext _userContext;
		public IUserContext UserContext
		{
			get {return _userContext;}
		}
	}
	public delegate void UserContextEventHandler(object sender, UserContextEventArgs e);
	public interface IUserContextContainer
	{
		event UserContextEventHandler BeforeAddUserContext;
		event UserContextEventHandler AfterAddUserContext;
		event UserContextEventHandler BeforeRemoveUserContext;
		event UserContextEventHandler AfterRemoveUserContext;
	}
	
}
