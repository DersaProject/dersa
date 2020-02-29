using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Dersa.Common;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;

namespace Dersa.Models
{

    public class ActivateStruct
    {
        public ActivateStruct(string login, int id)
        {
            username = login;
            userid = id;
        }
        public string username;
        public int userid;
    }
    public class AccountControllerAdapter
    {
        private string _contextUserName;
        public AccountControllerAdapter(string contextUserName) : base()
        {
            _contextUserName = contextUserName;
        }
        public string Info(string login)
        {
            DersaSqlManager DM = new DersaSqlManager();
            System.Data.DataTable T = DM.ExecuteMethod("DERSA_USER", "GetInfo", new object[] { login });
            T.Rows[0]["email"] = Cryptor.Decrypt(T.Rows[0]["email"].ToString(), DersaUtil.GetDefaultPassword());
            return JsonConvert.SerializeObject(T);
        }
        public string SetUserSettings(string json_params)
        {
            try
            {
                IParameterCollection Params = Util.DeserializeParams(json_params);
                DersaSqlManager DM = new DersaSqlManager();
                string userName = _contextUserName;
                foreach (IParameter Param in Params)
                {
                    try
                    {
                        System.Data.DataTable T = DM.ExecuteMethod("USER_SETTING", "SetValue", new object[] { Param.Name, Param.Value, userName, DersaUtil.GetPassword(userName) });
                        if (T.Rows.Count > 0)
                        {
                            return T.Rows[0][0].ToString();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                return "";
            }
            catch
            {
                throw;
            }
        }
        public string JsSettings(string settingName = null)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                string userName = _contextUserName;
                if (settingName == null)
                {
                    System.Data.DataTable T = DM.ExecuteMethod("USER_SETTING", "List", new object[] { userName, DersaUtil.GetPassword(userName) });
                    var query =
                        from System.Data.DataRow R in T.Rows
                        select new
                        {
                            Name = R["name"],
                            Value = R["value"],
                            ReadOnly = R["ReadOnly"],
                            Type = R["value_type"],
                            ControlType = (int)R["value_type"] == 1 ? "text" : "button",
                            ChildFormAttrs = (int)R["value_type"] == 1 ? null : new
                            {
                                Height = 500,
                                Width = 400,
                                DisplayValue = (int)R["value_type"] == 1 ? R["value"] : "...",
                                InfoLink = (int)R["value_type"] == 1 ? "" : "Account/JsSettings?settingName=" + R["name"].ToString()
                            }

                        };
                    string result = JsonConvert.SerializeObject(query);
                    return result;
                }
                else
                {
                    System.Data.DataTable T = DM.ExecuteMethod("DERSA_USER", "GetTextUserSetting", new object[] { userName, DersaUtil.GetPassword(userName), settingName });
                    var query =
                        from System.Data.DataRow R in T.Rows
                        select new
                        {
                            Name = settingName,
                            Value = R[0],
                            ReadOnly = false,
                            Type = 2,
                            ControlType = "textarea",
                            Height = 200,
                            Width = 300,
                            InfoLink = ""
                        };
                    string result = JsonConvert.SerializeObject(query);
                    return result;
                }
            }
            catch (Exception exc)
            {
                return "";
            }
        }
        public string Register(string login, string password, string name, string email)
        {
            if (string.IsNullOrEmpty(login))
                return "Не заполнено имя пользователя";
            IParameterCollection Params = new ParameterCollection();
            Params.Add("@login", login);
            SqlManager M = new DersaAnonimousSqlManager();
            int checkresult = M.ExecuteIntMethod("DERSA_USER", "Exists", Params);
            if (checkresult > 0)
                return "Пользователь с таким логином уже зарегистрирован";
            Params.Add("@email", Cryptor.Encrypt(email, DersaUtil.GetDefaultPassword()));
            checkresult = M.ExecuteIntMethod("DERSA_USER", "Exists", Params);
            if (checkresult > 0)
                return "Пользователь с таким email уже зарегистрирован";
            try
            {
                Token(login, email);
                System.Data.DataTable T = M.ExecuteMethod("DERSA_USER", "Register", new object[] { login, password, Cryptor.Encrypt(email, DersaUtil.GetDefaultPassword()), name });
                return "";
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
        public string Token(string login, string email = "")
        {
            ActivateStruct S = new ActivateStruct(login, 1);
            string JS = JsonConvert.SerializeObject(S);
            string result = Cryptor.Encrypt(JS, DersaUtil.GetDefaultPassword());
            string token = System.Web.HttpUtility.UrlEncode(result);
            SmtpClient Smtp = new SmtpClient("robots.1gb.ru", 25);
            Smtp.Credentials = new NetworkCredential("u483752", "5b218ad92ui");
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress("info@dersa.ru");
            DersaAnonimousSqlManager DM = new DersaAnonimousSqlManager();
            System.Data.DataTable T = DM.ExecuteMethod("DERSA_USER", "GetInfo", new object[] { login });
            if (email == "")
                if (T.Rows.Count > 0)
                    email = Cryptor.Decrypt(T.Rows[0]["email"].ToString(), DersaUtil.GetDefaultPassword());
            if (email == "")
                return "Undefined email";
            Message.To.Add(new MailAddress(email));
            Message.Subject = "регистрация в проекте DERSA";
            Message.Body = "";// string.Format("Вы успешно зарегистрировались в проекте DERSA. Для активации вашего аккаунта пройдите по ссылке: http://{0}/account/activate?token={1}", HttpContext.Current.Request.Url.Authority, token);

            try
            {
                Smtp.Send(Message);
                return "Success! Letter sent to " + email + "(robots.1gb.ru, 25) ; token = [" + token + "]";
            }
            catch (SmtpException exc)
            {
                return exc.Message;
            }
            return "Unknown error";


        }
        public string Activate(string token)
        {
            string sresult = Cryptor.Decrypt(token, DersaUtil.GetDefaultPassword());
            ActivateStruct S = JsonConvert.DeserializeObject(sresult, typeof(ActivateStruct)) as ActivateStruct;
            IParameterCollection Params = new ParameterCollection();
            Params.Add("@id", S.userid);
            Params.Add("@login", S.username);
            Params.Add("@password", DersaUtil.GetPassword(S.username));
            SqlManager M = new DersaAnonimousSqlManager();

            int checkresult = M.ExecuteIntMethod("DERSA_USER", "Activate", Params);
            return S.username;
        }
        public static string AuthorizeUser(string user_name = "", string password = "")
        {
            string result = "Unknown user name or password.";
            try
            {
                IParameterCollection Params = new ParameterCollection();
                Params.Add("@login", user_name);
                Params.Add("@password", password);
                SqlManager M = new DersaAnonimousSqlManager();
                int checkresult = M.ExecuteIntMethod("DERSA_USER", "CanAuthorize", Params);
                //int checkresult = M.ExecuteSPWithResult("DERSA_USER$CanAuthorize", false, Params);
                if (checkresult == (int)DersaUserStatus.active)
                {
                    //IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                    //authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    //var User = new UserProvider(user_name);
                    //System.Security.Claims.ClaimsIdentity identity = new System.Security.Claims.ClaimsIdentity(User.Identity, null, "ApplicationCookie", null, null);
                    //authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
                    return "";
                }
                switch (checkresult)
                {
                    case (int)DersaUserStatus.registered:
                        result = "Your registration is not completed.";
                        break;

                }
            }
            catch { throw; }
            return result;

        }

        public string Logout()
        {
            //System.Web.HttpContext.Current.GetOwinContext().Authentication.SignOut();
            return "Успешно вышли из системы";
        }

    }
}