using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Nancy;
using Nancy.Authentication.Forms;

namespace Dersa_N
{
    public class LoginInfo
    {
        public LoginInfo()
        { }
        public LoginInfo(string _login)
        {
            login = _login;
        }
        public string login;
        public string password;
    }

    public class UserDatabase : IUserMapper
    {
        public static string userName;
        private static List<Tuple<string, string, Guid>> users = new List<Tuple<string, string, Guid>>();

        static UserDatabase()
        {
            users.Add(new Tuple<string, string, Guid>("localuser", "pwd", new Guid("01BCEC7B-A9CE-46A6-8E16-374B6898E73E")));
            users.Add(new Tuple<string, string, Guid>("lanit", "pwd", new Guid("7A9852E7-7E3A-4147-896A-867235E59623")));
            users.Add(new Tuple<string, string, Guid>("lanitadmin", "pwd", new Guid("C656FAB5-B2F3-4710-854F-8173AB4AE213")));
        }

        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var userRecord = users.FirstOrDefault(u => u.Item3 == identifier);

            return userRecord == null
                       ? null
                       : new ClaimsPrincipal(new GenericIdentity(userRecord.Item1));
        }

        public static Guid? ValidateUser(string username, string password)
        {
            var userRecord = users.FirstOrDefault(u => u.Item1 == username && u.Item2 == password);

            if (userRecord == null)
            {
                return null;
            }

            return userRecord.Item3;
        }
    }
}