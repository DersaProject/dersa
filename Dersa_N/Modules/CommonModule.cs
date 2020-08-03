using Nancy;
using Nancy.Extensions;
using Nancy.Authentication.Forms;
using System;
using System.Dynamic;

namespace Dersa_N
{
    public class CommonModule : NancyModule
    {
        public CommonModule()
        {
            Get("/Login", args =>
            {
                dynamic model = new ExpandoObject();
                model.Errored = this.Request.Query.error.HasValue;

                return View["login", model];
            });
            Post("/login", args => {
                var userGuid = UserDatabase.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);

                if (userGuid == null)
                {
                    return this.Context.GetRedirect("~/login?error=true&username=" + (string)this.Request.Form.Username);
                }

                DateTime? expiry = null;
                if (this.Request.Form.RememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                return this.LoginAndRedirect(userGuid.Value, expiry);
            });
            Get("/logout", args => {
                return this.LogoutAndRedirect("~/");
            });

        }
    }
}
