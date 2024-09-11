using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;

namespace Dersa.Common
{
    public class MessageManager
    {
        public static string CurrentMessageKey
        {
            get
            {
                HttpCookie editKeyCookie = HttpContext.Current.Request.Cookies["messageKey"];
                if (editKeyCookie == null)
                    return null;
                return editKeyCookie.Value;
            }
            set
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("messageKey", value));
            }
        }

        public static string SetNewKeyForLoginIfEmpty()
        {
            string loginKey = CurrentMessageKey;
            if (!string.IsNullOrEmpty(loginKey))
                return loginKey;
            loginKey = Guid.NewGuid().ToString();
            CurrentMessageKey = loginKey;
            return loginKey;
        }
    }

}
