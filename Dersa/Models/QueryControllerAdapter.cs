﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dersa.Common;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Newtonsoft.Json;
using System.Reflection;
using Dersa.Interfaces;
using DersaStereotypes;

namespace Dersa.Models
{
    public class QueryControllerAdapter
    {
        //public static string _query;
        private static Hashtable hashTable = new Hashtable();

        public IParameterCollection GetViewParams(string cshtmlId)
        {
            string json_params = GetString(cshtmlId, false);
            IParameterCollection Params = Util.DeserializeParams(json_params);
            if (Params.Contains("cshtml"))
            {
                string view_name = GetViewName(Params["cshtml"].Value.ToString());
                Params.Remove("cshtml");
                Params.Add("view_name", view_name);
            }
            return Params;
        }

        public string GetViewName(string cshtml)
        {
            cshtml = cshtml.Replace("$gt$", ">").Replace("$lt$", "<");
            string fileName = Guid.NewGuid().ToString() + ".cshtml";
            //Stream S = File.OpenWrite(AppDomain.CurrentDomain.BaseDirectory + "Views/Temp/" + fileName);
            using (Stream S = File.OpenWrite(AppDomain.CurrentDomain.BaseDirectory + "Views/Temp/" + fileName))
            {
                try
                {
                    byte[] bts = System.Text.Encoding.Default.GetBytes(cshtml);
                    S.Write(bts, 0, bts.Length);
                }
                finally
                {
                    S.Flush();
                    S.Close();
                }
            }
            return fileName;
        }
        public static string PutString(string src)
        {
            string userName = "";
            if(HttpContext.Current != null)
                userName = HttpContext.Current.User.Identity.Name;
            string Id = Guid.NewGuid().ToString();
            hashTable[userName+Id] = src;
            return Id;
        }
        public static string GetString(string Id, bool viewSource, string userName = null)
        {
            if (userName == null && HttpContext.Current != null)
                userName = HttpContext.Current.User.Identity.Name;
            string result = hashTable[userName+Id].ToString();
            if(!viewSource)
                result = result.Replace("$lt$", "<").Replace("$gt$", ">");
            hashTable.Remove(Id);
            return result;
        }
        public string GetText(string json_params)
        {
            IParameterCollection Params = Util.DeserializeParams(json_params);
            if (Params.Contains("entity") && Params.Contains("attr_name") && Params.Contains("method_name"))
            {
                try
                {
                    string entityId = Params["entity"].Value.ToString();
                    string methodName = Params["method_name"].Value.ToString();
                    string res = (new NodeControllerAdapter()).ExecMethodResult(int.Parse(entityId), methodName).ToString();
                    var result = new
                    {
                        entityId = entityId,
                        attrName = Params["attr_name"].Value,
                        attrValue = res
                    };
                    return JsonConvert.SerializeObject(result);
                }
                catch (Exception exc)
                {
                    return exc.Message;
                }
            }
            else if (Params.Contains("entity") && Params.Contains("attr_name"))
            {
                string userName = HttpContext.Current.User.Identity.Name;
                string entityId = Params["entity"].Value.ToString();
                string attrName = Params["attr_name"].Value.ToString();
                string attrValue = (new QueryExecuteService()).GetAttrValue(attrName, entityId, userName);
                var result = new
                {
                    entityId = entityId,
                    attrName = attrName,
                    attrValue = attrValue
                };
                return JsonConvert.SerializeObject(result);
            }
            return "";
        }
        public string OpenHtml(string json_params)
        {
            IParameterCollection Params = Util.DeserializeParams(json_params);
            if (Params.Contains("html"))
            {
                string Id = PutString(Params["html"].Value.ToString());
                var result = new { action = "window.open('Query/GetHtml?Id=" + Id + "','user html', 'width=400,height=400,status=1,menubar=1');" };
                return JsonConvert.SerializeObject(result);
            }
            if (Params.Contains("URL"))
            {
                var result = new { action = "window.open('" + Params["URL"].Value.ToString() + "');" };
                return JsonConvert.SerializeObject(result);
            }
            return null;
        }

        public string ReportParams(string json_params)
        {
            //IParameterCollection Params = Util.DeserializeParams(json_params);
            //string args = "";
            //int i = 0;
            //foreach(IParameter param in Params)
            //{
            //    if (i++ > 0)
            //        args += "&";
            //    args += param.Name;
            //    args += "=";
            //    args += param.Value.ToString();
            //}
            var result = new { action = "window.open('/Query/Report?parameters=" + json_params + "');" };
            return JsonConvert.SerializeObject(result);
        }

        public string GetActionForParams(string json_params)
        {
            IParameterCollection Params = Util.DeserializeParams(json_params);
            if (Params.Contains("method_name") && Params.Contains("objectid"))
            {
                object[] extParams = new object[] { json_params };
                try
                {
                    string result = GetAction(Params["method_name"].Value.ToString(), int.Parse(Params["objectid"].Value.ToString()), JsonConvert.SerializeObject(extParams));
                    if (Params.Contains("result_is_already_formatted"))
                        return result;
                    var actionObject = new { action = result };
                    return JsonConvert.SerializeObject(actionObject);
                }
                catch(Exception exc)
                {
                    Logger.LogStatic(exc.Message);
                }
            }
            return null;
        }

        public string GetAction(string MethodName, int id, string paramString = null)
        {//AllowExecuteJSMethod
            string userName = HttpContext.Current.User.Identity.Name;
            StereotypeBaseE target = StereotypeBaseE.GetSimpleInstance(id);
            if(!target.AllowExecuteMethod(userName, MethodName))
                return string.Format("You are not allowed to execute method {0}", MethodName);
            CachedObjects.CachedEntities[id] = null;
            DersaSqlManager M = new DersaSqlManager();
            System.Data.DataTable t = M.GetEntity(id.ToString());
            if (t == null)
                throw new Exception(string.Format("Table is null for entity {0}", id));
            if (t.Rows.Count < 1)
                throw new Exception(string.Format("Table is empty for entity {0}", id));
            Entity ent = new Entity(t, M);
            CachedObjects.CachedCompiledInstances[ent.StereotypeName + id.ToString()] = null;
            foreach (Entity child in ent.Children)
            {
                CachedObjects.CachedCompiledInstances[child.StereotypeName + child.Id.ToString()] = null;
            }

            ICompiled cInst = ent.GetCompiledInstance();
            MethodInfo mi = cInst.GetType().GetMethod(MethodName);
            if (mi == null)
            {
                string excMessage = "Method " + MethodName + " not found ";
                Logger.LogStatic(excMessage);
                throw new Exception(excMessage);
            }
            object[] externalParams = new object[0];
            if (paramString != null)
                externalParams = JsonConvert.DeserializeObject<object[]>(paramString);
            object[] callParams = new object[externalParams.Length + 1];
            callParams[0] = userName;
            for (int i = 0; i < externalParams.Length; i++)
            {
                callParams[i + 1] = externalParams[i];
            }
            //Logger.LogStatic(string.Format("method {0}, params count {1}", MethodName, callParams.Length));
            object result = mi.Invoke(cInst, new object[] { callParams });
            if (result == null)
                return null;
            if(result is string)
                return result.ToString();
            return JsonConvert.SerializeObject(result);
        }
        public static string GetQueryId(string query)
        {
            string UserName = HttpContext.Current.User.Identity.Name;
            if (string.IsNullOrEmpty(UserName))
                return null;
            string token = QueryExecuteService.GetToken(UserName);
            string encodedQuery = Cryptor.Encrypt(query, token);
            //_query = encodedQuery;
            //return Guid.NewGuid().ToString();
            return PutString(encodedQuery);
        }

        public string ExecSql(string json_params)
        {
            IParameterCollection Params = Util.DeserializeParams(json_params);
            if (!Params.Contains("SQL"))
                return json_params;
            else
            {
                DersaSqlManager M = new DersaSqlManager();
                string sql = Params["SQL"].Value.ToString().Replace("$gt$", ">").Replace("$lt$", "<");
                IParameterCollection UserParams = new ParameterCollection();
                string userName = HttpContext.Current.User.Identity.Name;
                UserParams.Add("@login", userName);
                UserParams.Add("@password", Util.GetPassword(userName));
                int userPermissions = M.ExecuteSPWithResult("DERSA_USER$GetPermissions", false, UserParams);
                int canExecSql = userPermissions & 1;
                if (canExecSql == 0)
                    return "You have no permissions to exec SQL in database.";
                UserParams.Add("@user_setting_name", "Выполнять SQL локально");
                int execSqlLocal = M.ExecuteSPWithResult("DERSA_USER$GetBoolUserSetting", false, UserParams);
                int canExecLocalSql = userPermissions & 2;
                if (execSqlLocal > 0)
                {
                    if (canExecLocalSql == 0)
                        return "You have no permissions to exec SQL locally.";
                    else
                    {
                        string queryId = GetQueryId(sql);
                        (UserParams["@user_setting_name"] as IParameter).Value = "Функция вызова локального клиента SQL";
                        System.Data.DataTable VT = M.ExecuteSPWithParams("DERSA_USER$GetTextUserSetting", UserParams);
                        if (VT == null || VT.Rows.Count < 1)
                            throw new Exception("Функция вызова локального клиента SQL не определена");
                        string functionBody = VT.Rows[0][0].ToString();
                        var result = new { action = functionBody, arg_name = "queryId", arg = queryId };
                        return JsonConvert.SerializeObject(result);
                    }
                }

                try
                {
                    string result = "Unknown error";
                    if (Params.Contains("Server") && Params["Server"].Value != null)
                    {
                        string connectionString = string.Format("Server={0};Database={1};user={2};password={3}", Params["Server"].Value, Params["Database"].Value, Params["Login"].Value, Params["Password"].Value);
                        SqlManager ExecM = new SqlManager(connectionString);
                        result = ExecM.ExecMultiPartSql(sql);
                    }
                    else
                    {
                        DersaUserSqlManager UM = new DersaUserSqlManager();
                        result = UM.ExecMultiPartSql(sql);
                    }
                    if (result != "")
                        return result;
                    return "Запрос успешно выполнен:\n\n" + sql;
                }
                catch (Exception exc)
                {
                    return exc.Message;
                }
            }
        }
    }
}