using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DIOS.Common;
using DIOS.Common.Interfaces;
using System.Data.SqlClient;
using System.Web.Security;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations;

namespace Dersa.Models
{
    public class RegistrationInfo
    {
        //public string login;
        //public string password;
        //public string name;
        //public string email;

        [Required]
        public string Login { get; set; }
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
    public class LoginInfo
    {
        public LoginInfo()
        {}
        public LoginInfo(string _login)
        {
            login = _login;
        }
        public string login;
        public string password;
    }
    public class UserProvider : IPrincipal
    {
        private UserIndentity userIdentity { get; set; }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get
            {
                return userIdentity;
            }
        }

        public bool IsInRole(string role)
        {
            if (userIdentity.User == null)
            {
                return false;
            }
            return true;// userIdentity.User.InRoles(role);
        }

        #endregion


        public UserProvider(string name)
        {
            userIdentity = new UserIndentity();
            userIdentity.Init(name);
        }


        public override string ToString()
        {
            return userIdentity.Name;
        }
    }

    public class User
    {
        public string Name;
        public User(string name)
        {
            Name = name;
        }
        public static bool Exists(string login)
        {
            try
            {
                IParameterCollection Params = new ParameterCollection();
                Params.Add("@login", login);
                SqlManager M = new Dersa.Common.DersaAnonimousSqlManager();
                int checkresult = M.ExecuteIntMethod("DERSA_USER", "Exists", Params);
                //int checkresult = M.ExecuteSPWithResult("DERSA_USER$Exists", false, Params);
                return checkresult > 0;
            }
            catch(Exception exc)
            {
                return false;
            }
        }
    }
    public class UserIndentity : IIdentity
    {
        public User User { get; set; }

        public string AuthenticationType
        {
            get
            {
                return typeof(User).ToString();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return User != null;
            }
        }

        public string Name
        {
            get
            {
                if (User != null)
                {
                    return User.Name;
                }
                //иначе аноним
                return "anonym";
            }
        }

        public void Init(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                User = new User(name);
            }
        }
    }

}