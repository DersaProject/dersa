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

        internal static bool GetLocalSqlExecution()
        {
            string userName = HttpContext.Current.User.Identity.Name;
            DersaSqlManager M = new DersaSqlManager();
            IParameterCollection UserParams = new ParameterCollection();
            UserParams.Add("@login", userName);
            UserParams.Add("@password", DersaUtil.GetPassword(userName));
            int userPermissions = M.ExecuteIntMethod("DERSA_USER", "GetPermissions", UserParams);
            int canExecSql = userPermissions & 1;
            if (canExecSql == 0)
                throw new Exception("You have no permissions to exec SQL in database.");
            UserParams.Add("@user_setting_name", "Выполнять SQL локально");
            int execSqlLocal = M.ExecuteIntMethod("DERSA_USER", "GetBoolUserSetting", UserParams);
            int canExecLocalSql = userPermissions & 2;
            if (execSqlLocal > 0)
            {
                if (canExecLocalSql == 0)
                    throw new Exception("You have no permissions to exec SQL locally.");
                return true;
            }
            return false;
        }

        public IParameterCollection GetViewParams(string cshtmlId)
        {
            string json_params = DersaUtil.GetString(cshtmlId, false);
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
        public string GetText(string json_params)
        {
            IParameterCollection Params = Util.DeserializeParams(json_params);
            if (Params.Contains("entity") && Params.Contains("attr_name") && Params.Contains("method_name"))
            {
                try
                {
                    string entityId = Params["entity"].Value.ToString();
                    string methodName = Params["method_name"].Value.ToString();
                    string res = DersaUtil.ExecMethodResult(int.Parse(entityId), methodName).ToString();
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
                string attrValue = DersaUtil.GetAttributeValue(userName, int.Parse(entityId), attrName, -1);
                string fileExtension = DersaUtil.GetFileExtension(userName, int.Parse(entityId), attrName, -1);
                //(new QueryExecuteService()).GetAttrValue(attrName, entityId, userName);
                var result = new
                {
                    entityId = entityId,
                    attrName = attrName,
                    attrValue = attrValue,
                    fileExtension = fileExtension
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
                string Id = DersaUtil.PutString(Params["html"].Value.ToString());
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

        public static string GetActionForParams(string json_params)
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

        public static Tuple<ICompiled, MethodInfo> GetMethodInfo(string MethodName, int id, string paramString = null)
        {
            CachedObjects.CachedEntities[id] = null;
            DersaSqlManager M = new DersaSqlManager();
            System.Data.DataTable t = M.GetEntity(id.ToString());
            if (t == null)
                throw new Exception(string.Format("Table is null for entity {0}", id));
            if (t.Rows.Count < 1)
                throw new Exception(string.Format("Table is empty for entity {0}", id));
            DersaEntity ent = new DersaEntity(t, M);
            CachedObjects.CachedCompiledInstances[ent.StereotypeName + id.ToString()] = null;
            foreach (DersaEntity child in ent.Children)
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
            return new Tuple<ICompiled, MethodInfo>(cInst, mi);
        }

        public static string GetAction(string MethodName, int id, string paramString = null)
        {//AllowExecuteJSMethod
            StereotypeBaseE target = StereotypeBaseE.GetSimpleInstance(id);
            string userName = HttpContext.Current.User.Identity.Name;
            if (!target.AllowExecuteMethod(userName, MethodName))
                return string.Format("You are not allowed to execute method {0}", MethodName);
            Tuple<ICompiled, MethodInfo> MI = GetMethodInfo(MethodName, id, paramString);
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
            object result = MI.Item2.Invoke(MI.Item1, new object[] { callParams });

            if (result == null)
                return null;
            if(result is string)
                return result.ToString();
            return JsonConvert.SerializeObject(result);
        }
        public static string GetQueryId(string query, object dersaEntity, object objectName, object objectType, object changerComment)
        {
            var queryStruct = new
            {
                dersa_entity = dersaEntity,
                object_name = objectName,
                object_type = objectType,
                query_text = query,
                comment = changerComment == null? "executed via local DERSA service" : changerComment
            };
            string UserName = HttpContext.Current.User.Identity.Name;
            if (string.IsNullOrEmpty(UserName))
                return null;
            string token = QueryExecuteService.GetToken(UserName);
            string encodedQueryStruct = Cryptor.Encrypt(JsonConvert.SerializeObject(queryStruct), token);
            //_query = encodedQuery;
            //return Guid.NewGuid().ToString();
            return DersaUtil.PutString(encodedQueryStruct);
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
                string userName = HttpContext.Current.User.Identity.Name;
                bool execSqlLocal = false;
                try
                {
                    execSqlLocal = GetLocalSqlExecution();
                }
                catch(Exception exc)
                {
                    return exc.Message;
                }
                object dersaEntity = Params["entity_id"]?.Value;
                object objectName = Params["object_name"]?.Value;
                object objectType = Params["object_type"]?.Value;
                object changerComment = Params["comment"]?.Value;
                if (execSqlLocal)
                {
                    string queryId = GetQueryId(sql, dersaEntity, objectName, objectType, changerComment);
                    IParameterCollection UserParams = new ParameterCollection();
                    UserParams.Add("@login", userName);
                    UserParams.Add("@password", DersaUtil.GetPassword(userName));
                    UserParams.Add("@user_setting_name", "Функция вызова локального клиента SQL");
                    //                        (UserParams["@user_setting_name"] as IParameter).Value = "Функция вызова локального клиента SQL";
                    try
                    {
                        System.Data.DataTable VT = M.ExecuteMethod("DERSA_USER", "GetTextUserSetting", UserParams);
                        if (VT == null || VT.Rows.Count < 1)
                            throw new Exception("Функция вызова локального клиента SQL не определена");
                        string functionBody = VT.Rows[0][0].ToString();
                        var result = new { action = functionBody, arg_name = "queryId", arg = queryId };
                        return JsonConvert.SerializeObject(result);
                    }
                    catch (Exception exc)
                    {
                        throw;
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
                    else if (Params.Contains("conn_string") && Params["conn_string"].Value != null)
                    {
                        string connectionStringAlias = Params["conn_string"].Value.ToString();
                        SqlManager ExecM = new SqlManager(connectionStringAlias);
                        if (dersaEntity != null)
                        {
                            try
                            {
                                IParameterCollection preExecParams = new DIOS.Common.ParameterCollection();
                                preExecParams.Add("dersa_entity", dersaEntity);
                                preExecParams.Add("changer", userName);
                                preExecParams.Add("object_name", objectName);
                                preExecParams.Add("object_type", objectType);
                                preExecParams.Add("new_ddl", sql);
                                preExecParams.Add("changer_comment", changerComment);
                                ExecM.ExecuteIntMethod("QUERY_AUDIT", "OnQuery", preExecParams);
                            }
                            catch(Exception exc)
                            {
                                Logger.LogStatic(exc.Message);
                            }
                        }
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