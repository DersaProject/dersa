using System;
using DIOS.Common.Interfaces;
using DIOS.Common;

namespace Dersa.Common
{
	public enum DersaUserStatus: int {active = 2, blocked = 3, unauthorized = 0, registered = 1, anonimous = 5, not_dersauser = 4}
}