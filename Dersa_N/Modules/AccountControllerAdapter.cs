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

namespace Dersa_N
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
        public string Info(string login)
        {
            DersaSqlManager DM = new DersaSqlManager();
            System.Data.DataTable T = DM.ExecuteMethod("DERSA_USER", "GetInfo", new object[] { login });
            T.Rows[0]["email"] = Cryptor.Decrypt(T.Rows[0]["email"].ToString(), DersaUtil.GetDefaultPassword());
            return JsonConvert.SerializeObject(T);
        }
        public static string SetUserSettings(string json_params)
        {
            try
            {
                IParameterCollection Params = Util.DeserializeParams(json_params);
                DersaSqlManager DM = new DersaSqlManager();
                string userName = "localuser";
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
        public static string JsSettings(string settingName = null)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                string userName = "localuser";
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
                                InfoLink = (int)R["value_type"] == 1 ? "" : "Account/JsSettings/" + R["name"].ToString()
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
            catch(Exception exc)
            {
                return "";
            }
        }
        //public string Register(string login, string password, string name, string email)
        //{
        //    if(string.IsNullOrEmpty(login))
        //        return "Не заполнено имя пользователя";
        //    IParameterCollection Params = new ParameterCollection();
        //    Params.Add("@login", login);
        //    SqlManager M = new DersaAnonimousSqlManager();
        //    int checkresult = M.ExecuteIntMethod("DERSA_USER", "Exists", Params);
        //    if (checkresult > 0)
        //        return "Пользователь с таким логином уже зарегистрирован";
        //    Params.Add("@email", Cryptor.Encrypt(email, DersaUtil.GetDefaultPassword()));
        //    checkresult = M.ExecuteIntMethod("DERSA_USER", "Exists", Params);
        //    if (checkresult > 0)
        //        return "Пользователь с таким email уже зарегистрирован";
        //    try
        //    {
        //        Token(login, email);
        //        System.Data.DataTable T = M.ExecuteMethod("DERSA_USER", "Register", new object[] { login, password, Cryptor.Encrypt(email, DersaUtil.GetDefaultPassword()), name });
        //        return "";
        //    }
        //    catch(Exception exc) 
        //    {
        //        return exc.Message;
        //    }
        //}
        //public string Token(string login, string email = "")
        //{
        //    ActivateStruct S = new ActivateStruct(login, 1);
        //    string JS = JsonConvert.SerializeObject(S);
        //    string result = Cryptor.Encrypt(JS, DersaUtil.GetDefaultPassword());
        //    string token = System.Web.HttpUtility.UrlEncode(result);
        //    SmtpClient Smtp = new SmtpClient("robots.1gb.ru", 25);
        //    Smtp.Credentials = new NetworkCredential("u483752", "5b218ad92ui");
        //    MailMessage Message = new MailMessage();
        //    Message.From = new MailAddress("info@dersa.ru");
        //    DersaAnonimousSqlManager DM = new DersaAnonimousSqlManager();
        //    System.Data.DataTable T = DM.ExecuteMethod("DERSA_USER", "GetInfo", new object[] { login });
        //    if(email == "")
        //    if (T.Rows.Count > 0)
        //        email = Cryptor.Decrypt(T.Rows[0]["email"].ToString(), DersaUtil.GetDefaultPassword());
        //    if (email == "")
        //        return "Undefined email";
        //    Message.To.Add(new MailAddress(email));
        //    Message.Subject = "регистрация в проекте DERSA";
        //    Message.Body = string.Format("Вы успешно зарегистрировались в проекте DERSA. Для активации вашего аккаунта пройдите по ссылке: http://{0}/account/activate?token={1}", HttpContext.Current.Request.Url.Authority, token);

        //    try
        //    {
        //        Smtp.Send(Message);
        //        return "Success! Letter sent to " + email + "(robots.1gb.ru, 25) ; token = [" + token + "]";
        //    }
        //    catch (SmtpException exc)
        //    {
        //        return exc.Message;
        //    }
        //    return "Unknown error";


        //}
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
        //public static string AuthorizeUser(string user_name = "", string password = "")
        //{
        //    string result = "Unknown user name or password.";
        //    try
        //    {
        //        IParameterCollection Params = new ParameterCollection();
        //        Params.Add("@login", user_name);
        //        Params.Add("@password", password);
        //        SqlManager M = new DersaAnonimousSqlManager();
        //        int checkresult = M.ExecuteIntMethod("DERSA_USER", "CanAuthorize", Params);
        //        //int checkresult = M.ExecuteSPWithResult("DERSA_USER$CanAuthorize", false, Params);
        //        if (checkresult == (int)DersaUserStatus.active)
        //        {
        //            IAuthenticationManager authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
        //            authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //            var User = new UserProvider(user_name);
        //            System.Security.Claims.ClaimsIdentity identity = new System.Security.Claims.ClaimsIdentity(User.Identity, null, "ApplicationCookie", null, null);
        //            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
        //            return "";
        //        }
        //        switch (checkresult)
        //        {
        //            case (int)DersaUserStatus.registered:
        //                result = "Your registration is not completed.";
        //                break;

        //        }
        //    }
        //    catch { throw; }
        //    return result;
            
        //}

        //public string Logout()
        //{
        //    System.Web.HttpContext.Current.GetOwinContext().Authentication.SignOut();
        //    return "Успешно вышли из системы";
        //}


        //public string ChangePasswordByAdmin(string login, string new_password)
        //{
        //    _login = login;
        //    _new_password = new_password;
        //    ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForChangePasswordByAdmin));
        //    return M.MakeResponse();
        //}

        //private string doTrueResponseForChangePasswordByAdmin()
        //{
        //    if (System.Web."localuser" == "")
        //        throw new Exception("You must log in");
        //    SimpleObject u = CommonEnvironment.GetInstance().GetCurrentUser();
        //    if (_login == "")
        //        throw new Exception("User login was not supplied");
        //    var manager = new UserManager();
        //    ApplicationUser user = manager.FindByName(_login);
        //    if (user == null)
        //        throw new Exception(string.Format("Login {0} not found", _login));
        //    manager.RemovePassword(user.Id);
        //    manager.AddPassword(user.Id, _new_password);

        //    return null;
        //}
        //public string ChangeOwnPassword(string old_password, string new_password)
        //{
        //    _login = System.Web."localuser";
        //    _password = old_password;
        //    _new_password = new_password;
        //    ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForChangeOwnPassword));
        //    return M.MakeResponse();
        //}

        //private string doTrueResponseForChangeOwnPassword()
        //{
        //    if (_login == "")
        //        throw new Exception("User login was not supplied");
        //    var manager = new UserManager();
        //    ApplicationUser user = manager.Find(_login, _password);
        //    if (user == null)
        //        throw new Exception(string.Format("Wrong password for login {0}", _login));
        //    manager.ChangePassword(user.Id, _password, _new_password);
        //    return null;
        //}

        //public string RegShort(string user_name = "", string password = "")
        //{
        //    _user_name = user_name;
        //    _password = password;
        //    if (_user_name == null)
        //        _user_name = "";
        //    if (_password == null)
        //        _password = "";
        //    //if (_user_name == "")
        //    //{
        //    //    string ReturnUrl = System.Web.HttpContext.Current.Request.Params["return_url"];
        //    //    System.Web.HttpContext.Current.Response.Redirect("/LoginForm.aspx?return_url=" + ReturnUrl);
        //    //    return "";
        //    //}
        //    ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForRegShort));
        //    return M.MakeResponse();
        //}

        //private string doTrueResponseForRegShort()
        //{
        //    if (_user_name == "")
        //        throw new Exception("User name can not be empty.");
        //    var manager = new UserManager();
        //    var user = new ApplicationUser() { UserName = _user_name };
        //    IdentityResult result = manager.Create(user, _password);
        //    if (result.Succeeded)
        //    {
        //        IdentityHelper.SignIn(manager, user, isPersistent: false);
        //        return string.Format("User {0} has successfully registered.", _user_name);
        //    }
        //    throw new Exception(result.Errors.FirstOrDefault());
        //}

        //public string Register(
        //    string login = "",
        //    string password = "",
        //    string user_name = "",
        //    string email = "",
        //    string address = "",
        //    string organization = "",
        //    string phone = "",
        //    string payment_details = "",
        //    int user_role = 0,
        //    bool validateOnly = true)
        //{
        //    _login = login.Trim();
        //    _password = password.Trim();
        //    _user_name = user_name.Trim();
        //    _email = email.Trim();
        //    _address = address.Trim();
        //    _organization = organization.Trim();
        //    _phone = phone.Trim();
        //    _payment_details = payment_details.Trim();
        //    _user_role = user_role;
        //    _validateOnly = validateOnly;
        //    ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForRegister));
        //    string response = M.MakeResponse();//в первый раз вызовем для установки признака, что ответ содержит ошибки
        //    if (M.ErrorCode != 0 || _error_code == 0)//второй раз надо вызывать только если ошибки нашла "внутренняя программа", а внешняя отработала нормально
        //        return response;                    //мы тогда устанавливаем признак, что ответ есть по сути структурированный отчёт об ошибках
        //    M.SetErrorCode(_error_code);
        //    _validateOnly = true; //на всякий случай
        //    return M.MakeResponse();

        //}

        //private string doTrueResponseForRegister()
        //{
        //    CuksUserManager V = new CuksUserManager();
        //    RegisterResponse R = new RegisterResponse();
        //    R.login = _login;
        //    R.password = _password;
        //    R.email = _email;
        //    R.user_name = _user_name;
        //    R.user_role = _user_role == 0 ? "" : _user_role.ToString();
        //    if (!V.Validate(ref R))
        //        _error_code = 1;
        //    if (_error_code == 0 && !_validateOnly)
        //    {
        //        V.RegisterUser(_login, _password, _user_name, _email, (int)CuksUserRole.customer, _address, _organization, _phone, _payment_details, _user_role);
        //    }

        //    return JsonConvert.SerializeObject(R);

            //string login_status = "";
            //string password_status = "";
            //string user_name_status = "";
            //string email_status = "";
            //string user_role_status = "";

            //DiosSqlManager M = new DiosSqlManager(true);
            //if (_login == "")
            //{
            //    login_status = "login can not be empty";
            //}
            //else
            //{
            //    string loginRegEx = M.GetSysParam("RegExp for login");
            //    System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(loginRegEx);
            //    System.Text.RegularExpressions.Match loginMatch = regEx.Match(_login);
            //    if (!loginMatch.Success || loginMatch.Result("$0") != _login)
            //    {
            //        login_status = "login is invalid";
            //    }
            //}

            //if (_password == "")
            //{
            //    password_status = "password can not be empty";
            //}
            //if (_user_name == "")
            //{
            //    user_name_status = "User name can not be empty";
            //}
            //else
            //{
            //    string usernameRegEx = M.GetSysParam("RegExp for user name");
            //    System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(usernameRegEx);
            //    System.Text.RegularExpressions.Match usernameMatch = regEx.Match(_user_name);
            //    if (!usernameMatch.Success || usernameMatch.Result("$0") != _user_name)
            //    {
            //        user_name_status = "user name is invalid";
            //    }
            //}
            //if (_email == "")
            //{
            //    email_status = "email can not be empty";
            //}
            //else
            //{
            //    string emailRegEx = M.GetSysParam("Email validation RegExp");
            //    System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(emailRegEx);
            //    System.Text.RegularExpressions.Match emailMatch = regEx.Match(_email);
            //    if(!emailMatch.Success)
            //    {
            //        email_status = "email is invalid";
            //    }
            //}
            //if (_user_role == 0)
            //{
            //    user_role_status = "User role must be chosen";
            //}
            //if (login_status != ""
            //    || password_status != ""
            //    || user_name_status != ""
            //    || email_status != ""
            //    || user_role_status != ""
            //    )
            //    _error_code = 1;
            //else if(!_validateOnly)
            //{
            //    var manager = new UserManager();
            //    var user = new ApplicationUser() { UserName = _login };
            //    IdentityResult result = manager.Create(user, _password);
            //    if (!result.Succeeded)
            //        throw new Exception(result.Errors.FirstOrDefault());
            //    else
            //    {
            //        ObjectFactory UF = M.GetFactory("CUKS_USER");
            //        IParameterCollection userParams = new ParameterCollection();
            //        if (_organization != "")
            //        {
            //            ObjectFactory OF = M.GetFactory("CUKS_ORGANIZATION");
            //            ParameterCollection orgParams = new ParameterCollection();
            //            orgParams.Add("name", _organization);
            //            CuksOrganization Org = OF.GetObject(orgParams) as CuksOrganization;
            //            if (Org == null)
            //            {
            //                Org = OF.Create(orgParams) as CuksOrganization;
            //            }
            //            userParams.Add("cuks_organization", Org.cuks_organization);
            //        }
            //        userParams.Add("first_name", _user_name);
            //        userParams.Add("cuks_user_role", _user_role);
            //        userParams.Add("email", _email);
            //        userParams.Add("phone", _phone);
            //        userParams.Add("address", _address);
            //        userParams.Add("payment_details", _payment_details);
            //        userParams.Add("UID", new SqlGuid(new Guid(user.Id)));
            //        //newUser.UID = new SqlGuid(new Guid(user.Id)); этот вариант хуже, потому что новая запись в БД запишется уже в Create при вызове ApplyParams 
            //        CuksUser newUser = UF.Create(userParams) as CuksUser;
            //        IdentityHelper.SignIn(manager, user, isPersistent: false);
            //    }
            //}
            //return JsonConvert.SerializeObject(
            //    new
            //    {
            //        login = login_status,
            //        password = password_status,
            //        user_name = user_name_status,
            //        email = email_status,
            //        user_role = user_role_status
            //    });
        }

    //    public string MyRoles()
    //    {
    //        ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForMyRoles));
    //        return M.MakeResponse();
    //    }
    //    private string doTrueResponseForMyRoles()
    //    {
    //        DiosSqlManager M = new CuksSqlManager();
    //        return M.GetCurrentUserRoles(null);
    //    }

    //    public string MyInfo()
    //    {
    //        ResponseMaker M = new ResponseMaker(new ReceiveResponseHandler(doTrueResponseForMyInfo));
    //        return M.MakeResponse();
    //    }
    //    private string doTrueResponseForMyInfo()
    //    {
    //        //DiosSqlManager M = new CuksSqlManager();
    //        //ObjectFactory F = M.GetFactory("CURRENT_USER");
    //        //ParameterCollection Params = new ParameterCollection();
    //        //Params.Add("UserName", M.UserName);
    //        //SimpleObject u = F.GetObject(Params) as SimpleObject;
    //        string res = CurrentUserInfo();
    //        if (res == null)
    //            throw new Exception("Current user info is unavailable");
    //        return res;
    //    }

    //    public static string CurrentUserInfo()
    //    {
    //        SimpleObject u = CommonEnvironment.GetInstance().GetCurrentUser();
    //        if (u != null)
    //        {
    //            object userPriority = u["priority"];
    //            if (userPriority != null && !((SqlInt32)userPriority).IsNull)
    //            {
    //                userPriority = ((SqlInt32)userPriority).Value;
    //            }
    //            string res = JsonConvert.SerializeObject(new
    //            {
    //                id = ((SqlInt32)u["cuks_user"]).Value,
    //                login = System.Web."localuser",//u["login"].ToString(),
    //                full_name = u["full_name"].ToString(),
    //                role = ((SqlInt32)u["role"]).Value,
    //                priority = userPriority,//((SqlInt32)u["cuks_user_role"]).Value,
    //                cuks_workplace = ((SqlInt32)u["cuks_workplace"]).Value
    //            });
    //            return res;
    //        }
    //        return null;
    //    }
    //}
}