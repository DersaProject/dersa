using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dersa.Interfaces;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Newtonsoft.Json;
using System.Reflection;

namespace Dersa.Common
{
    public enum AttributeOwnerType : int { Entity = 0, Relation = 1 }
    public class Util
    {
        public static string SetGuid(string userName, string entityId, string guid)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                DM.ExecuteSPWithParams("ENTITY$SetGuid", new object[] { entityId, guid, Util.GetPassword(userName) });
                return "";
            }
            catch(Exception exc)
            {
                return exc.Message;
            }
        }

        public static string SetAttributeValue(DersaSqlManager DM, string userName, AttributeOwnerType ownerType, string entityId, string attrName, int attrType, string attrValue)
        {
            string procName = "";
            switch(ownerType)
            {
                case AttributeOwnerType.Entity:
                    procName = "ENTITY$SetAttribute";
                    break;
                case AttributeOwnerType.Relation:
                    procName = "RELATION$SetAttribute";
                    break;
            }
            IParameterCollection Params = new ParameterCollection();
            Params.Add("@entity", entityId);
            Params.Add("@attr_name", attrName);
            Params.Add("@attr_value", attrValue);
            Params.Add("@login", userName);
            Params.Add("@password", Util.GetPassword(userName));
            Params.Add("@attr_type", attrType);
            int res = DM.ExecuteSPWithResult(procName, false, Params);
            if(res == 5)
            {
                Params["@attr_type"].Value = 5;
                Params["@attr_value"].Value = Cryptor.Encrypt(attrValue, userName);
                res = DM.ExecuteSPWithResult(procName, false, Params);
            }
            return "";
        }
        public static string GetAttributeValue(string userName, int entityId, string attrName, int attrType)
        {
            DersaSqlManager DM = new DersaSqlManager();
            System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$GetAttribute", new object[] { entityId, attrName, userName, Util.GetPassword(userName) });
            if (T == null || T.Rows.Count < 1)
                return null;
            if (attrType <= 0)
                attrType = (int)T.Rows[0]["Type"];
            string result = T.Rows[0]["Value"].ToString();
            if (attrType == 5)
            {
                try
                {
                    result = Cryptor.Decrypt(result, userName);
                }
                catch { }
            }
            return result;
        }

        public static int GetUserPermissions(string userName)
        {
            DersaSqlManager M = new DersaSqlManager();
            IParameterCollection UserParams = new ParameterCollection();
            UserParams.Add("@login", userName);
            UserParams.Add("@password", Util.GetPassword(userName));
            return M.ExecuteSPWithResult("DERSA_USER$GetPermissions", false, UserParams);
        }

        public static string GetUserSetting(string userName, string settingName)
        {
            DersaSqlManager DM = new DersaSqlManager();
            IParameterCollection UserParams = new ParameterCollection();
            UserParams.Add("@login", userName);
            UserParams.Add("@password", Util.GetPassword(userName));
            UserParams.Add("@user_setting_name", settingName);
            System.Data.DataTable VT = DM.ExecuteSPWithParams("DERSA_USER$GetUserSetting", UserParams);
            if (VT == null || VT.Rows.Count < 1)
                throw new Exception(string.Format("Не задано значение для параметра {0}", settingName));
            return VT.Rows[0][0].ToString();
        }

        public static string EntitySetAttribute(string userName, int entity, string prop_name, string prop_value)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$SetAttribute", new object[] { entity, prop_name, prop_value, userName, Util.GetPassword(userName) });
                if (T.Rows.Count > 0)
                {
                    return T.Rows[0][0].ToString();
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public static void EntitySetStereotype(string userName, int entity, string stereotype_name)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                DM.ExecuteSPWithParams("ENTITY$SetStereotype", new object[] { entity, stereotype_name, userName, Util.GetPassword(userName) });
            }
            catch
            {
            }
        }

        public static string EntityAddRelation(string userName, int entity, int entity_b, string stereotype_name, string relation_name, string source_class, int source_ref)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$AddRelation", new object[] { entity, entity_b, stereotype_name, relation_name, source_class, source_ref, userName, Util.GetPassword(userName) });
                if (T.Rows.Count > 0)
                {
                    return T.Rows[0][0].ToString();
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public static string EntityAddChild(string userName, string src, string dst, int options)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                DataTable T = DM.ExecuteSPWithParams("ENTITY$OnDnD", new object[] { src, dst, options, userName, GetPassword(userName) });
                string result = JsonConvert.SerializeObject(T);
                return result;
            }
            catch
            {
                return "";
            }
        }

        public static string GetDefaultPassword()
        {
            return "#$&65498fgxghAS";
        }

        public static string GetPassword(string login)
        {
            byte[] data = System.Text.Encoding.Default.GetBytes(login + login.ToLower() + login.ToUpper() + "654^%$");
            byte[] h = Cryptor.ComputeHash(data);
            return System.Text.Encoding.Default.GetString(h);
        }

        public static IParameterCollection DeserializeParams(string json_params)
        {
            IParameterCollection Params = new ParameterCollection();
            if (json_params != null && json_params != "")
            {
                DIOS.Common.Parameter[] res = JsonConvert.DeserializeObject(json_params, typeof(DIOS.Common.Parameter[])) as DIOS.Common.Parameter[];
                for (int p = 0; p < res.Length; p++)
                {
                    if (res[p].Predicate != null && res[p].Predicate.ToLower() == "in")
                    {
                        Newtonsoft.Json.Linq.JArray jarrayValue = (Newtonsoft.Json.Linq.JArray)res[p].Value;
                        object[] paramValue = new object[jarrayValue.Count];
                        int i = 0;
                        foreach (object val in jarrayValue)
                        {
                            paramValue[i++] = val;
                        }
                        res[p].Value = paramValue;
                        res[p].Predicate = "in";
                    }
                    if (res[p].Name != "id")
                        Params.Add(res[p]);
                }
            }
            return Params;
        }

        public static object[] GetMethodCallParameterValues(string Parameters)
        {
            if (Parameters == null || Parameters == "")
                return new object[] { };
            string[] ParamsWithValues = Parameters.Split(',');
            object[] res = new object[ParamsWithValues.Length];
            for (int p = 0; p < ParamsWithValues.Length; p++)
            {
                string[] ParamWithValue = ParamsWithValues[p].Split('=');
                if (ParamWithValue.Length == 2)
                {
                    string ParamWithType = ParamWithValue[0].Trim();
                    string ParamTypeString = ParamWithType.Split(' ')[0].Trim();
                    Type ParamType = GetDynamicType(ParamTypeString); //Type.GetType(ParamTypeString);
                    res[p] = Convert(ParamWithValue[1].Trim(), ParamType);
                }
                else
                    res[p] = null;
            }
            return res;
        }
        public static object GetParamValue(string description)
        {
            string descrWithType = description.Split('=')[0];
            string descrVal = description.Split('=')[1].Replace("\"", "");
            string typeName = descrWithType.Split(' ')[0];
            System.Type T = Type.GetType(typeName);
            if (T == null)
                throw new System.Exception("No type for type name " + typeName);

            return Dersa.Common.Util.Convert(descrVal, T);
        }

        public static object Convert(object value, Type conversionType)
        {
            if ((value == null) || (conversionType.IsAssignableFrom(value.GetType()))) return value;
            if (value is string)
            {
                string str = (string)value;
                if (str.Trim() == "") return null;
                if (str.Trim() == "null") return null;
                if (conversionType == typeof(bool))
                {
                    if ((str.ToLower() != "true") && (str.ToLower() != "false"))
                    {
                        throw new Exception(str + " не конвертится в Boolean");
                    }
                }
            }
            if (conversionType.IsEnum)
            {
                System.ComponentModel.EnumConverter converter = new System.ComponentModel.EnumConverter(conversionType);
                return converter.ConvertFrom(value);
            }
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
                throw new Exception("Тип " + value.GetType().ToString() + " не является конвертируемым.");
            object res = null;
            res = convertible.ToType(conversionType, null);
            return res;
        }

        public static Type GetDynamicType(string typeName)
        {
            Type dType = Type.GetType(typeName);
            if (dType != null)
                return dType;
            Assembly stAssembly = Assembly.Load("Dersa.Stereotypes");
            if (stAssembly != null)
            {
                dType = stAssembly.GetType(typeName);
                if (dType != null)
                    return dType;
            }
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (!assemblies[i].GetName().Name.Contains("App_Code") && !assemblies[i].GetName().Name.Contains("Dersa.Stereotypes"))
                    continue;
                dType = assemblies[i].GetType(typeName);
                if (dType != null)
                    return dType;
            }
            return null;
        }
        public static ICompiled CreateInstance(IStereotypedObject obj, DersaSqlManager M)
        {
            if (obj == null)
                throw new Exception("obj == null");
            if (CachedObjects.CachedCompiledInstances[obj.StereotypeName + obj.Id.ToString()] != null)
                return (ICompiled)CachedObjects.CachedCompiledInstances[obj.StereotypeName + obj.Id.ToString()];
            //if (obj.Stereotype == null)
            //    throw new Exception("obj.Stereotype == null, obj.name = " + obj.Name);
            Type nType = GetDynamicType("DersaStereotypes." + obj.StereotypeName);
            ICompiled res = (ICompiled)Activator.CreateInstance(nType, new object[] { obj });
            System.Data.DataTable t = null;
            if (res is ICompiledRelation)
                t = M.GetAttributesForRelation(obj.Id.ToString());
            else if (res is ICompiledEntity)
                t = M.GetAttributesForEntity(obj.Id.ToString());
            if (t == null)
            {
                throw new Exception("Не смогли определиться между E и R");
            }
            foreach (System.Data.DataRow r in t.Rows)
            {
                //res[r["name"].ToString()] = r["value"].ToString();
                System.Reflection.FieldInfo[] fis = nType.GetFields();
                foreach (System.Reflection.FieldInfo fi in fis)
                {
                    if (fi.Name == r["name"].ToString())
                    {
                        object value_type = r["value_type"];
                        object value = null;
                        if (value_type == null || value_type.ToString() == "1")
                            value = r["value"];
                        else
                            value = r["text_value"];
                        if (value == null || value is System.DBNull)
                            value = "";
                        if (value != null)
                        {
                            object ConvertedValue = DIOS.Common.TypeUtil.Convert(value, fi.FieldType);
                            fi.SetValue(res, ConvertedValue);
                        }
                    }
                }
            }
            CachedObjects.CachedCompiledInstances[obj.StereotypeName + obj.Id.ToString()] = res;
            return res;

            /*
            if (obj.Stereotype.Name == "Attribute")
            {
                Type nType = Type.GetType("DersaStereotypes." + obj.Stereotype.Name);
                //Type type = typeof(DersaStereotypes.Attribute);
                SqlManager M = new SqlManager();
                DersaStereotypes.Attribute res = new DersaStereotypes.Attribute((IEntity)obj);
                System.Data.DataTable t = M.GetAttributesForEntity(obj.Id.ToString());
                foreach (System.Data.DataRow r in t.Rows)
                {
                    //res[r["name"].ToString()] = r["value"].ToString();
                    System.Reflection.FieldInfo[] fis = type.GetFields();
                    foreach (System.Reflection.FieldInfo fi in fis)
                    {
                        if (fi.Name == r["name"].ToString())
                        {
                            object value = r["value"];
                            if (value != null)
                            {
                                object ConvertedValue = Dersa.Common.Util.Convert(value, fi.FieldType);
                                fi.SetValue(res, ConvertedValue);
                            }
                        }
                    }
                }
                return res;
            }
            if (obj.Stereotype.Name == "Entity")
            {
                Type type = typeof(DersaStereotypes.Entity);
                SqlManager M = new SqlManager();
                DersaStereotypes.Entity res = new DersaStereotypes.Entity((IEntity)obj);
                System.Data.DataTable t = M.GetAttributesForEntity(obj.Id.ToString());
                foreach (System.Data.DataRow r in t.Rows)
                {
                    //res[r["name"].ToString()] = r["value"].ToString();
                    System.Reflection.FieldInfo[] fis = type.GetFields();
                    foreach (System.Reflection.FieldInfo fi in fis)
                    {
                        if (fi.Name == r["name"].ToString())
                        {
                            object value = r["value"];
                            if (value != null)
                            {
                                object ConvertedValue = Dersa.Common.Util.Convert(value, fi.FieldType);
                                fi.SetValue(res, ConvertedValue);
                            }
                        }
                    }
                }
                return res;
            }*/
            return null;

            //////////////////////////////////////////////////////////////////////
            //if (_stereotypesAssembly == null)
            //    throw new NullReferenceException("Библиотека стереотипов не скомпилирована");
            //Type type = _stereotypesAssembly.GetType("DersaStereotypes." + obj.Stereotype.Name);
            //ICompiled cs = (ICompiled)Activator.CreateInstance(type, new object[] { obj });
            //System.Reflection.FieldInfo[] fis = type.GetFields();
            //try
            //{
            //    foreach (System.Reflection.FieldInfo fi in fis)
            //    {
            //        object value = obj.GetAttribute(fi.Name).GetValue();
            //        if (value != null)
            //        {
            //            fi.SetValue(cs, value);
            //        }
            //    }
            //    return cs;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Ошибка создания экземпляра объекта " + obj.Stereotype.Name + " : " + obj.Name + " : " + obj.Id + "\n" + ex.Message, ex);
            //}
        }
    }
}
