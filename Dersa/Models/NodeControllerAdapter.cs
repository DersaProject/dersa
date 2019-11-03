using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dersa.Interfaces;
using Dersa.Common;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Newtonsoft.Json;
using System.Xml;
using System.Reflection;
using System.Data;
using DersaStereotypes;

namespace Dersa.Models
{
    public class NodeControllerAdapter
    {
        public class SchemaEntity
        {
            public string StereotypeName;
            public string Name;
            public SchemaAttribute[] schemaAttributes;
            public SchemaEntity[] childEntities;
        }
        public class SchemaAttribute
        {
            public string Name;
            public string Value;
        }

        public static SchemaEntity[] GetSchema(string JsonContent)
        {
            SchemaEntity[] schemaContent = JsonConvert.DeserializeObject<SchemaEntity[]>(JsonContent);
            return schemaContent;
        }

        public string GetInsertSubmenu(int id)
        {
            DataTable menuLevels = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(
                new object[] {
                    new
                    {
                        name = "Package",
                        icon = "/icons/Package.gif",
                        level = 1,
                        is_submenu = false
                    },
                    new
                    {
                        name = "Entity",
                        icon = "/icons/Entity.gif",
                        level = 1,
                        is_submenu = false
                    },
                    new
                    {
                        name = "Attribute",
                        icon = "/icons/Attribute.gif",
                        level = 1,
                        is_submenu = false
                    },
                    new
                    {
                        name = "Script",
                        icon = "/icons/Script.gif",
                        level = 1,
                        is_submenu = false
                    },
                    new
                    {
                        name = "Procedure",
                        icon = "/icons/Procedure.gif",
                        level = 1,
                        is_submenu = false
                    },
                    new
                    {
                        name = "Method",
                        icon = "/icons/Method.gif",
                        level = 1,
                        is_submenu = false
                    },
                    new
                    {
                        name = "прочие",
                        icon = "",
                        level = 1,
                        is_submenu = true
                    }
                }));
            DersaSqlManager DM = new DersaSqlManager();
            string userName = HttpContext.Current.User.Identity.Name;
            DataTable childStereotypes = DM.ExecuteSPWithParams("ENTITY$GetChildStereotypes", new object[] { id, userName, DersaUtil.GetPassword(userName) });
            var result = from DataRow RL in menuLevels.Rows
                         where RL["level"].ToString() == "1" && (childStereotypes.Select("name = '" + RL["name"].ToString() + "'").Length > 0 || (bool)RL["is_submenu"])//аналог exists
                              select
                              new
                              {
                                  label = RL["name"],
                                  icon = RL["icon"],
                                  children = from DataRow RS in childStereotypes.Rows
                                             where menuLevels.Select("name = '" + RS["name"].ToString() + "'").Length == 0 && (bool)RL["is_submenu"]
                                             select
                                             new
                                             {
                                                label = RS["name"],
                                                icon = RS["icon_path"]
                                             }
                              };
                
            return JsonConvert.SerializeObject(result);
        }

        public int CanDnD(string src, int dst)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                string userName = HttpContext.Current.User.Identity.Name;
                IParameterCollection Params = new ParameterCollection();
                Params.Add("dnd_source", src);
                Params.Add("dnd_target", dst);
                Params.Add("login", userName);
                Params.Add("password", DersaUtil.GetPassword(userName));
                int result = DM.ExecuteSPWithResult("ENTITY$CanDnD", false, Params);
                return result;
            }
            catch
            {
                return 0;
            }
        }

        public string ChildStereotypes(int id)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                string userName = HttpContext.Current.User.Identity.Name;
                string result = JsonConvert.SerializeObject(DM.ExecuteSPWithParams("ENTITY$GetChildStereotypes", new object[] { id, userName, DersaUtil.GetPassword(userName) }));
                return result;
            }
            catch
            {
                return "";
            }
        }

        public object ExecMethodResult(int id, string method_name)
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
            foreach(DersaEntity child in ent.Children)
            {
                CachedObjects.CachedCompiledInstances[child.StereotypeName + child.Id.ToString()] = null;
            }
            Logger.LogStatic("ExecMethodResult clear cache Entity" + ent.Id.ToString());
            //Type nType = Type.GetType("DersaStereotypes." + ent.Stereotype.Name);

            ICompiled cInst = ent.GetCompiledInstance();
            MethodInfo mi = cInst.GetType().GetMethod(method_name);
            if(mi == null)
            {
                string excMessage = "Method " + method_name + " not found ";
                Logger.LogStatic(excMessage);
                throw new Exception(excMessage);
            }
            //string userName = HttpContext.Current.User.Identity.Name;
            //System.Data.DataTable T = M.ExecuteSPWithParams("dbo.ENTITY$GetMethodParams", new object[] { id, method_name, userName, DersaUtil.GetPassword(userName) });
            //string Params = "";
            //if (T.Rows.Count > 0)
            //    Params = T.Rows[0][0].ToString();
            //object[] ParamValues = Util.GetMethodCallParameterValues(Params);
            //return mi.Invoke(cInst, ParamValues);
            return mi.Invoke(cInst, new object[]{});
            //object res = mi.Invoke(cInst, ParamValues);
            //string resultText = "";
            //if (res != null)
            //    resultText = res.ToString();
            //return resultText;
        }

        public string ExecMethodForm(int id, string method_name)
        {
            //DersaSqlManager M = new DersaSqlManager();
            //System.Data.DataTable t = M.GetEntity(id.ToString());
            //if (t == null)
            //    throw new Exception(string.Format("Table is null for entity {0}", id));
            //if (t.Rows.Count < 1)
            //    throw new Exception(string.Format("Table is empty for entity {0}", id));
            //Entity ent = new Entity(t, M);
            //Type nType = Type.GetType("DersaStereotypes." + ent.Stereotype.Name);

            //ICompiled cInst = ent.GetCompiledInstance();
            //MethodInfo mi = cInst.GetType().GetMethod(method_name);
            //string userName = HttpContext.Current.User.Identity.Name;
            //System.Data.DataTable T = M.ExecuteSPWithParams("dbo.ENTITY$GetMethodParams", new object[] { id, method_name, userName, DersaUtil.GetPassword(userName) });
            //string Params = "";
            //if (T.Rows.Count > 0)
            //    Params = T.Rows[0][0].ToString();
            //object[] ParamValues = Util.GetMethodCallParameterValues(Params);
            //object res = mi.Invoke(cInst, ParamValues);

            object execRes = ExecMethodResult(id, method_name);
            string res = "";
            if (execRes is string)
                res = execRes.ToString();
            string displayText = "the method " + method_name + " returned no result";
            if (!string.IsNullOrEmpty(res))
                displayText = res.ToString();
            var resultArray = new object[]{
                new
                {
                    Name = "SQL",
                    Value = displayText,
                    DisplayValue = displayText,
                    ControlType = "textarea",
                    Height = 200,
                    Width = 300,
                    WriteUnchanged = true
                },
                new
                {
                    Name = "Server",
                    Value = "",
                    DisplayValue = "",
                    ControlType = "text"
                },
                new
                {
                    Name = "Database",
                    Value = "",
                    DisplayValue = "",
                    ControlType = "text"
                },
                new
                {
                    Name = "Login",
                    Value = "",
                    DisplayValue = "",
                    ControlType = "text"
                },
                new
                {
                    Name = "Password",
                    Value = "",
                    DisplayValue = "",
                    ControlType = "password"
                }
            };
            string result = JsonConvert.SerializeObject(resultArray);
            return result;
        }

        private string GetOnClick(int getResultType, string methodName, int id)
        {
            switch(getResultType)
            {
                case 0:
                    return "javascript:window.open(\"Node/DownloadMethodResult?id=" + id.ToString() + "&method_name=" + methodName + "\")";
                case 2:
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append("var xhr = new XMLHttpRequest();");
                        sb.Append("xhr.open('GET','/Query/GetAction?MethodName=" + methodName + "&id=" + id.ToString() + "',false);");
                        sb.Append("xhr.send('');");
                        sb.Append("try{eval(xhr.responseText);}");
                        sb.Append("catch(err){");
                        sb.Append("let errWin = window.open('', 'newwin', 'width=700,height=700,status=1,menubar=1');");
                        sb.Append("errWin.document.open();");
                        sb.Append("errWin.document.write(xhr.responseText);");
                        sb.Append("errWin.document.close();}");

                        return sb.ToString(); ;
                    }
            }
            return "javascript:alert('!!!*!!!')";
        }

        public string MethodsForm(int id)
        {
            try
            {
                string SqlExecAction = "alert";
                DersaSqlManager DM = new DersaSqlManager();
                string userName = HttpContext.Current.User.Identity.Name;
                IParameterCollection UserParams = new ParameterCollection();
                UserParams.Add("@login", userName);
                UserParams.Add("@password", DersaUtil.GetPassword(userName));
                int userPermissions = DM.ExecuteSPWithResult("DERSA_USER$GetPermissions", false, UserParams);
                int canExecSql = userPermissions & 1;
                if (canExecSql != 0)
                {
                    UserParams.Add("@user_setting_name", "Выполнять SQL локально");
                    int execSqlLocal = DM.ExecuteSPWithResult("DERSA_USER$GetBoolUserSetting", false, UserParams);
                    int canExecLocalSql = userPermissions & 2;
                    if (execSqlLocal > 0 && canExecLocalSql != 0)
                    {
                        SqlExecAction = "exec"; 
                    }
                }

                System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$GetMethods", new object[] { id, userName, DersaUtil.GetPassword(userName) });
                int i = 1;
                var query =
                    from System.Data.DataRow R in T.Rows
                    select new
                    {
                        Name = R["name"],
                        Value = "",
                        ReadOnly = false,
                        Type = -1,
                        ControlType = "button",
                        ChildFormAttrs = new
                        {
                            Height = 800,
                            Width = 600,
                            DisplayValue = "...",
                            InfoLink = (int)R["get_result_type"] == 1 ? "Node/ExecMethodForm?id=" + id.ToString() + "&method_name=" + R["name"].ToString() : "",
                            SaveLink = "Query/ExecSql",//GetSaveLink((int)R["get_result_type"], R["name"].ToString(), id),
                            OnClick = GetOnClick((int)R["get_result_type"], R["name"].ToString(), id),
                            ActionAfterExec = SqlExecAction
                        }

                    };
                string result = JsonConvert.SerializeObject(query);
                return result;
            }
            catch
            {
                return "";
            }
        }
        public string PropertyForm(int id, string prop_name, int prop_type)
        {
            //DersaSqlManager DM = new DersaSqlManager();
            string userName = HttpContext.Current.User.Identity.Name;
            //System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$GetAttribute", new object[] { id, prop_name, userName, DersaUtil.GetPassword(userName) });
            //if (T.Rows.Count < 1)
            //    return null;
            string attrValue = DersaUtil.GetAttributeValue(userName, id, prop_name, prop_type);
            //var query =
            //    from System.Data.DataRow R in T.Rows
            //    select new
            //    {
            //        Name = R["Name"],
            //        Value = prop_type == 4 ? "*****" : R["Value"],
            //        DisplayValue = prop_type == 4? "*****" : R["Value"],
            //        ControlType = "textarea",
            //        Height = 300,
            //        Width = 300,
            //        InfoLink = ""
            //    };
            var resObj = new object[] {
                new
                {
                    Name = prop_name,
                    Value = attrValue,
                    DisplayValue = attrValue,
                    ControlType = "textarea",
                    Height = 300,
                    Width = 300,
                    InfoLink = ""
                }
            };
            string result = JsonConvert.SerializeObject(resObj);
            return result;
        }
        public string PropertiesForm(int id)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                string userName = HttpContext.Current.User.Identity.Name;
                System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$GetAttributes", new object[] { id, userName, DersaUtil.GetPassword(userName) });
                var query =
                    from System.Data.DataRow R in T.Rows
                    select new
                    {
                        Name = R["Name"],
                        Value = R["Value"],
                        ReadOnly = R["ReadOnly"],
                        WriteUnchanged = R["WriteUnchanged"],
                        Type = R["Type"],
                        ControlType = (int)R["Type"] == 1 ? "text" : "button",
                        ChildFormAttrs = (int)R["Type"] == 1 ? null : new 
                        {
                            Height = 900,
                            Width = 600,
                            DisplayValue = (int)R["Type"] == 1 ? R["Value"] : "...",
                            InfoLink = (int)R["Type"] == 1 ? "" : "Node/PropertyForm?id=" + id.ToString() + "&prop_name=" + R["Name"].ToString() + "&prop_type=" + R["Type"].ToString()
                        }
                    };
                string result = JsonConvert.SerializeObject(query);
                return result;
            }
            catch
            {
                return "";
            }
        }

        public static string SetTextProperty(int entity, string prop_name, string prop_value, string userName = null)
        {
            DersaSqlManager DM = userName == null? new DersaSqlManager(): new DersaAnonimousSqlManager();
            if(userName == null)
                userName = HttpContext.Current.User.Identity.Name;
            try
            {
                System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$SetAttribute", new object[] { entity, prop_name, prop_value, userName, DersaUtil.GetPassword(userName) });
                if (T != null && T.Rows.Count > 0)
                {
                    return T.Rows[0][0].ToString();
                }
            }
            catch
            {
                throw;
            }
            return "";
        }

        public static void SetAttribute(DersaSqlManager DM, AttributeOwnerType ownerType, string entityId, string paramName, string paramValue, int attrType)
        {
            string userName = HttpContext.Current.User.Identity.Name;
            DersaUtil.SetAttributeValue(DM, userName, ownerType, entityId, paramName, attrType, paramValue);
            //DM.ExecuteSPWithParams(procName, new object[] { entityId, paramName, paramValue, userName, DersaUtil.GetPassword(userName) });
        }

        public string SetProperties(string json_params)
        {
            Dersa.Common.CachedObjects.ClearCache();
            IParameterCollection Params = Util.DeserializeParams(json_params);
            string key = "-1";
            //string procName = "";
            AttributeOwnerType ownerType = AttributeOwnerType.Entity;
            if (Params.Contains("entity"))
            {
                key = Params["entity"].Value.ToString();
                //procName = "ENTITY$SetAttribute";
                ownerType = AttributeOwnerType.Entity;
                Params.Remove("entity");
                if (Params.Count < 1)
                    return "no data";
            }
            if (Params.Contains("relation"))
            {
                key = Params["relation"].Value.ToString();
                //procName = "RELATION$SetAttribute";
                ownerType = AttributeOwnerType.Relation;
                Params.Remove("relation");
                if (Params.Count < 1)
                    return "no data";
            }
            DersaSqlManager DM = new DersaSqlManager();
            foreach (IParameter Param in Params)
            {
                try
                {
                    if(Param.Value != null)
                    {
                        string strVal = Param.Value.ToString().Replace("$lt$", "<").Replace("$gt$", ">");
                        Param.Value = strVal;
                    }
                    SetAttribute(DM, ownerType, key, Param.Name, Param.Value?.ToString(), 0);
                }
                catch
                {
                    throw;
                }
            }
            return "";
        }
        public string Properties(int id)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                string userName = HttpContext.Current.User.Identity.Name;
                string result = JsonConvert.SerializeObject(DM.ExecuteSPWithParams("ENTITY$GetAttributes", new object[] { id, userName, DersaUtil.GetPassword(userName) }));
                return result;
            }
            catch
            {
                return "";
            }

        }
        public string DnD(string src, string dst, int options)
        {
            //try
            //{
            //    DersaSqlManager DM = new DersaSqlManager();
            //    string userName = HttpContext.Current.User.Identity.Name;
            //    DataTable T = DM.ExecuteSPWithParams("ENTITY$OnDnD", new object[] { src, dst, options, userName, DersaUtil.GetPassword(userName) });
            //    string result = JsonConvert.SerializeObject(T);
            //    return result;
            //}
            //catch
            //{
            //    return "";
            //}
            string userName = HttpContext.Current.User.Identity.Name;

            if (src.Contains("D_"))//диагграммы
                return DersaUtil.EntityAddChild(HttpContext.Current.User.Identity.Name, src, dst, options);
            StereotypeBaseE objFrom = null;
            int intSrc = -1;
            try
            {
                intSrc = int.Parse(src);
                objFrom = StereotypeBaseE.GetSimpleInstance(intSrc);
            }
            catch { }
            StereotypeBaseE objTo = null;
            int intDst = -1;
            try
            {
                intDst = int.Parse(dst);
                objTo = StereotypeBaseE.GetSimpleInstance(intDst);
            }
            catch { }
            if (objFrom != null && objTo != null)//copy or move node
            {
                if ((options & 3) == 0)//move
                {
                    if (objFrom.Parent == null || !(objFrom.Parent as StereotypeBaseE).AllowModifyChildren())
                        return "";
                    return objTo.MoveChild(userName, src);
                }
                else
                {
                    return objTo.CopyChild(userName, src, options);
                }
            }
            else if(objTo != null) //child from stereotype
            {
                return objTo.CopyChild(userName, src, options);
            }

            return "";
        }

        public string Rename(string id, string name)
        {
            int intId = -1;
            try
            {
                intId = int.Parse(id);
            }
            catch { }
            if(intId < 0)//диаграммы и проч.
            {
                try
                {
                    DersaSqlManager DM = new DersaSqlManager();
                    string userName = HttpContext.Current.User.Identity.Name;
                    string result = JsonConvert.SerializeObject(DM.ExecuteSPWithParams("ENTITY$Rename", new object[] { id, name, userName, DersaUtil.GetPassword(userName) }));
                    return result;
                }
                catch
                {
                    return "";
                }
            }
            //для нормальных объектов используем объектные методы
            try
            {
                string userName = HttpContext.Current.User.Identity.Name;
                StereotypeBaseE objToRename = StereotypeBaseE.GetSimpleInstance(intId);
                if (objToRename != null)
                {
                    return objToRename.Rename(userName, name);
                }
                return "";
            }
            catch
            {
                return "";
            }
            //try
            //{
            //    DersaSqlManager DM = new DersaSqlManager();
            //    string userName = HttpContext.Current.User.Identity.Name;
            //    string result = JsonConvert.SerializeObject(DM.ExecuteSPWithParams("ENTITY$Rename", new object[] { id, name, userName, DersaUtil.GetPassword(userName) }));
            //    return result;
            //}
            //catch
            //{
            //    return "";
            //}
        }

        public string Restore(int id)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                string userName = HttpContext.Current.User.Identity.Name;
                string result = JsonConvert.SerializeObject(DM.ExecuteSPWithParams("ENTITY$Restore", new object[] { id, userName, DersaUtil.GetPassword(userName) }));
                return result;
            }
            catch
            {
                return "";
            }
        }
        public string Remove(int id, string diagram_id, int options)
        {
            try
            {
                string userName = HttpContext.Current.User.Identity.Name;
                StereotypeBaseE objToRemove = StereotypeBaseE.GetSimpleInstance(id);
                if(objToRemove != null)
                {
                    if (!objToRemove.AllowDrop())
                        return "object can not be dropped";
                    objToRemove.Drop(userName, options);
                    return "object dropped";
                }
                if(id < 0)
                {
                    StereotypeBaseE.DropRelation(id, userName);
                    return "relation dropped";
                }
                if (diagram_id != null)
                {
                    StereotypeBaseE.DropDiagram(diagram_id, userName);
                    return "diagram dropped";
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        public static string ListNodes(string id)
        {

            IObjectCollection col = DersaEntity.List(null);
            var query = from Dersa.Common.Entity ent in col
                        select new
                        {
                            id = ent.entity,
                            text = ent.name,
                            icon = ent.icon
                        };
            string result = JsonConvert.SerializeObject(query);
            return result;
        }
        public string List(string id)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                string userName = HttpContext.Current.User.Identity.Name;
                object parent = null;
                if (!id.Contains("#"))
                    parent = id;
                string result = "[]";
                if(id == "STEREOTYPES")
                {
                    result = DersaUtil.GetUserSetting(userName, "root stereotypes");
                    Regex removeDigitsEx = new Regex("[0-9]");
                    result = removeDigitsEx.Replace(result, "*");
                }
                else
                    result = JsonConvert.SerializeObject(DM.ExecuteSPWithParams("ENTITY$JTreeList", new object[] { parent, userName, DersaUtil.GetPassword(userName) }));
                return result;
            }
            catch(Exception exc)
            {
                return "";
            }
        }
        public string Description(string id, string attr_name)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                string userName = HttpContext.Current.User.Identity.Name;
                System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$GetDescription", new object[] { id, attr_name, userName, DersaUtil.GetPassword(userName) });
                string result = "";
                if (T.Rows.Count > 0)
                    result = T.Rows[0][0].ToString();
                if (attr_name == "DiagramXmlAdditional")
                {
                    XmlDocument addXml = new XmlDocument();
                    addXml.LoadXml(result);
                }
                return result;
            }
            catch(Exception exc)
            {
                return "";
            }
        }

    }
}