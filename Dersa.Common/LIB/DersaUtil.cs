using System;
using System.Linq;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dersa.Interfaces;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Newtonsoft.Json;
using System.Reflection;
using DersaStereotypes;
using System.Xml;
using System.Web;
using System.Diagnostics;

namespace Dersa.Common
{
    public class RouteRedirectException: Exception
    {
        public string configFolder;
        public RouteRedirectException(string _configFolder, string message) : base(message)
        {
            configFolder = _configFolder;
        }
    }
    public class DersaIO
    {
        public Func<string> ReadLine;
        public Func<string> ReadLineVirtually;
        public Action<string> WriteLine;
    }

    public enum AttributeOwnerType : int { Entity = 0, Relation = 1 }

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
    public class NodeData
    {
        public string id;
        public string text;
        public string icon;
        public string data;
        public int rank;
        public int erank;
        public bool children;

    }

    public class DersaKey
    {
        public string cacheId;
        public string cacheParent;
        public int entity;
    }
    public class DersaCache
    {
        private static int key = 0;
        private static Hashtable nodesTable = new Hashtable();
        private static Hashtable keysTable = new Hashtable();

        public static string GetAttributeValue(string id, string attrName)
        {
            object key = keysTable[id];
            if (key != null)
            {
                var obj = nodesTable[key] as SchemaEntity;
                var query = from attr in obj.schemaAttributes
                            where attr.Name == attrName
                            select attr.Value;
                return query.ToArray<string>()[0];
            }
            return "";
        }
        public static IList GetChildren(string id)
        {
            ArrayList list = new ArrayList();
            var query = from DersaKey k in nodesTable.Keys
                        where k.cacheParent == id
                        select k.cacheId;
            foreach (string childId in query)
            {
                list.Add(GetInstance(childId));
            }
            return list;
        }

        public static StereotypeBaseE GetParent(string id)
        {
            DersaKey key = keysTable[id] as DersaKey;
            if (key != null)
            {
                return GetInstance(key.cacheParent);
            }
            return null;
        }

        public static StereotypeBaseE GetInstance(int entityId)
        {
            var query = from DersaKey k in nodesTable.Keys
                        where k.entity == entityId
                        select k.cacheId;
            if (query.Count() > 0)
                return GetInstance(query.ToArray()[0]);
            return null;
        }

        public static StereotypeBaseE GetInstance(string id)
        {
            DersaKey key = keysTable[id] as DersaKey;
            if (key != null)
            {
                return GetInstance(key);
            }
            return null;
        }

        public static StereotypeBaseE GetInstance(DersaKey key)
        {
            var obj = nodesTable[key] as SchemaEntity;
            Type nodeType = DersaUtil.GetDynamicType("DersaStereotypes." + obj.StereotypeName);
            if (nodeType != null)
            {
                StereotypeBaseE inst = Activator.CreateInstance(nodeType, new object[] { }) as StereotypeBaseE;
                inst.SetCachedId(key.cacheId);
                inst.SetId(key.entity);
                inst.Name = obj.Name;
                for (int i = 0; i < obj.schemaAttributes.Length; i++)
                {
                    var fi = nodeType.GetField(obj.schemaAttributes[i].Name);
                    object value = obj.schemaAttributes[i].Value;
                    if (value != null)
                    {
                        if (value.ToString() == "" && fi.FieldType != typeof(string))
                            continue;
                        object ConvertedValue = DIOS.Common.TypeUtil.Convert(value, fi.FieldType);
                        fi.SetValue(inst, ConvertedValue);
                    }
                }
                return inst;
            }
            return null;
        }
            public static dynamic ExecMethodResult(string id, string method_name)
        {
            StereotypeBaseE inst = GetInstance(id);
            if (inst != null)
            {
                Type nodeType = inst.GetType();
                MethodInfo mi = nodeType.GetMethod(method_name);
                if (mi == null)
                {
                    string excMessage = "Method " + method_name + " not found ";
                    Logger.LogStatic(excMessage);
                    throw new Exception(excMessage);
                }
                return mi.Invoke(inst, new object[] { });
            }
            return null;
        }

        public static void SetAttributes(string id, IParameterCollection Params)
        {
            object key = keysTable[id];
            if (key != null)
            {
                var obj = nodesTable[key] as SchemaEntity;
                var query = (from SchemaAttribute attr in obj.schemaAttributes
                             select new SchemaAttribute
                             {
                                 Name = attr.Name,
                                 Value = Params[attr.Name] == null ? attr.Value : Params[attr.Name].Value.ToString()
                             });
                obj.schemaAttributes = query.ToArray();
            }

        }
        public static DataTable GetAttributes(string id)
        {
            var T = new DataTable();
            T.Columns.Add("Name", typeof(string));
            T.Columns.Add("Value", typeof(string));
            T.Columns.Add("Type", typeof(int));
            T.Columns.Add("ReadOnly", typeof(bool));
            T.Columns.Add("WriteUnchanged", typeof(bool));

            object key = keysTable[id];
            if (key != null)
            {
                var obj = nodesTable[key] as SchemaEntity;
                Type nodeType = DersaUtil.GetDynamicType("DersaStereotypes." + obj.StereotypeName);
                if (nodeType != null)
                {
                    DataRow R = T.NewRow();
                    R["Name"] = "cached_id";
                    R["Value"] = id;
                    R["Type"] = 1;
                    R["ReadOnly"] = true;
                    R["WriteUnchanged"] = true;
                    T.Rows.Add(R);
                    R = T.NewRow();
                    R["Name"] = "stereotype";
                    R["Value"] = obj.StereotypeName;
                    R["Type"] = 1;
                    R["ReadOnly"] = true;
                    R["WriteUnchanged"] = false;
                    T.Rows.Add(R);
                    R = T.NewRow();
                    R["Name"] = "name";
                    R["Value"] = obj.Name;
                    R["Type"] = 1;
                    R["ReadOnly"] = true;
                    R["WriteUnchanged"] = false;
                    T.Rows.Add(R);

                    var fis = nodeType.GetFields();
                    var attrs = from fi in fis
                                from objAttr in obj.schemaAttributes
                                where fi.Name == objAttr.Name
                                select new Tuple<string, string, bool>(fi.Name, objAttr.Value, fi.GetCustomAttribute(typeof(LongStringAttribute), false) != null);
                    foreach (Tuple<string, string, bool> attr in attrs)
                    {
                        R = T.NewRow();
                        R["Name"] = attr.Item1;
                        R["Value"] = attr.Item2;
                        R["Type"] = attr.Item3 ? 2 : 1;
                        R["ReadOnly"] = false;
                        R["WriteUnchanged"] = false;
                        T.Rows.Add(R);
                    }
                }
            }
            return T;
        }
        public static DataTable GetMethods(string id)
        {
            var T = new DataTable();
            T.Columns.Add("name", typeof(string));
            T.Columns.Add("get_result_type", typeof(int));
            
            object key = keysTable[id];
            if (key != null)
            {
                var obj = nodesTable[key] as SchemaEntity;
                Type nodeType = DersaUtil.GetDynamicType("DersaStereotypes." + obj.StereotypeName);
                if (nodeType != null)
                {
                    object inst = Activator.CreateInstance(nodeType, new object[] { });
                    System.Reflection.MethodInfo[] mis = nodeType.GetMethods();

                    var methodNames = from mi in mis
                                      //where mi.GetParameters().Length == 0
                                      where mi.GetCustomAttribute(typeof(CallFromUIAttribute), false) != null 
                                      select mi.Name;
                    foreach(string mname in methodNames)
                    {
                        DataRow R = T.NewRow();
                        R["name"] = mname;
                        R["get_result_type"] = 1;
                        T.Rows.Add(R);
                    }
                }
            }
            return T;
        }

        public static int Remove(string id)
        {
            DersaKey key = keysTable[id] as DersaKey;
            if (key != null)
            {
                nodesTable.Remove(key);
                keysTable.Remove(id);
                return key.entity;
            }
            return 0;
        }
        public static string Rename(string id, string newName)
        {
            object key = keysTable[id];
            if (key != null)
            {
                var obj = nodesTable[key] as SchemaEntity;
                obj.Name = newName;
                return JsonConvert.SerializeObject(
                    new
                    {
                        id = id,
                        text = obj.Name,
                        data = obj.StereotypeName,
                        icon = obj.StereotypeName,
                        rank = -1,
                        erank = -1,
                        children = true
                    });
            }
            return "";
        }
        public static IEnumerable List(string userName, string parentId)
        {
            var result = from DersaKey key in nodesTable.Keys
                         where key.cacheParent == parentId
                         select new NodeData
                         {
                             id = key.cacheId,
                             text = ((SchemaEntity)nodesTable[key]).Name,
                             icon = ((SchemaEntity)nodesTable[key]).StereotypeName,
                             data = ((SchemaEntity)nodesTable[key]).StereotypeName,
                             rank = -1,
                             erank = -1,
                             children = true
                         };
            return result;
        }

        public static bool NodesAreChained(string majorId, string minorId)
        {
            DersaKey key = keysTable[minorId] as DersaKey;
            if(key != null)
            {
                if (keysTable[key.cacheParent] as DersaKey == null)
                    return false;
                if (key.cacheParent == majorId)
                    return true;
                return NodesAreChained(majorId, key.cacheParent);
            }
            return false;
        }

        public static string DnD(string userName, string src, string dst, int entityId = 0)
        {
            DersaKey key = keysTable[src] as DersaKey;
            if (key != null)
                return SetParent(userName, key, dst, entityId);
            else
                return AddNode(userName, src, dst, entityId);
        }

        public static string SetParent(string userName, DersaKey objectKey, string parentId, int entityId = 0)
        {
            var obj = nodesTable[objectKey] as SchemaEntity;
            objectKey.cacheParent = parentId;
            var query = from N in new int[1]
                        select new
                        {
                            id = objectKey.cacheId,
                            name = obj.Name,
                            text = obj.Name,
                            icon = obj.StereotypeName,
                        };
            string result = JsonConvert.SerializeObject(query);
            return result;
        }
        public static string AddNode(string userName, string src, string dst, int entityId = 0, string nodeName = null)
        {
            string StereotypeName = src.Replace("CACHE_", "");
            string NodeName = nodeName == null ? StereotypeName : nodeName;
            var attrs = new SchemaAttribute[0];
            Type nodeType = DersaUtil.GetDynamicType("DersaStereotypes." + StereotypeName);
            if (nodeType != null)
            {
                object inst = Activator.CreateInstance(nodeType, new object[] { });
                System.Reflection.FieldInfo[] fis = nodeType.GetFields();
                attrs = (from fi in fis
                         select new SchemaAttribute
                         {
                             Name = fi.Name,
                             Value = entityId == 0? fi.GetValue(inst)?.ToString() : DersaUtil.GetAttributeValue(userName, entityId, fi.Name, 2)
                         }).ToArray();


                string id = "CACHE_" + key++.ToString();
                DersaKey nodeKey = new DersaKey { cacheId = id, cacheParent = dst, entity = entityId };
                var theNode = new SchemaEntity
                {
                    StereotypeName = StereotypeName,
                    Name = NodeName,
                    schemaAttributes = attrs,
                    childEntities = new SchemaEntity[0]
                };
                nodesTable.Add(nodeKey, theNode);
                keysTable.Add(id, nodeKey);
                var query = from N in new int[1]
                            select new NodeData
                            {
                                id = id,
                                //name = StereotypeName,
                                text = NodeName,
                                icon = StereotypeName,
                            };
                string result = JsonConvert.SerializeObject(query);
                return result;
            }
            return "no type " + StereotypeName;
        }
    }

    public class DersaUtil
    {
        private static Hashtable hashTable = new Hashtable();

        public static string[] GetCacheableClasses()
        {
            Assembly stAssembly = Assembly.Load("Dersa.Stereotypes");
            if (stAssembly != null)
            {
                var stTypes = stAssembly.GetTypes();
                var query = from Type T in stTypes
                            where (System.Attribute.GetCustomAttribute(T, typeof(CacheableAttribute)) as CacheableAttribute) != null
                            select T.Name;
                return query.ToArray();
            }

            return new string[] { "Entity", "Attribute" };
        }
        public static void SaveEntityToFile(int id, string userName, string attrName = "")
        {//сохраняем предыдущие значения, если это отдельный атрибут, то сохраняем и значение атрибута отдельно тоже
            DersaSqlManager DM = new DersaSqlManager();
            string path = HttpContext.Current.Server.MapPath("~/GitDir");
            string fileName = "";
            string fileText = "";

            if (attrName != "")
            {
                string attrValue = GetAttributeValue(userName, id, attrName, -1);
                fileName = string.Format("{0}\\{1}.{2}.txt", path, id, attrName);
                fileText = attrValue;
            }
            else
            {
                fileName = string.Format("{0}\\{1}.json", path, id);
                var entInfo = new Dictionary<string, object>();
                var TEnt = DM.ExecuteMethod("ENTITY", "GetInfo", new object[] { id, userName, DersaUtil.GetPassword(userName) });
                var entRow = TEnt.Rows[0];
                foreach (DataColumn C in TEnt.Columns)
                {
                    entInfo.Add(C.ColumnName, entRow[C]);
                }
                var TAttrs = DM.ExecuteMethod("ENTITY", "GetFullAttributes", new object[] { id, userName, DersaUtil.GetPassword(userName) });
                entInfo.Add("attributes", TAttrs);
                fileText = JsonConvert.SerializeObject(entInfo);
            }
            using (var SW = new System.IO.StreamWriter(fileName))
            {
                SW.Write(fileText);
            }

        }

        public static void CommitToGit(int id, string userName, string attrName = "")
        {
            string path = HttpContext.Current.Server.MapPath("~/GitDir");
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = path + "\\save_to_git.bat";
                startInfo.Arguments = string.Format("{0} {1} {2}", id, userName, attrName);
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))
                using (System.IO.StreamReader reader = process.StandardOutput)
                using (System.IO.StreamReader errReader = process.StandardError)
                {
                    process.WaitForExit();
                    //Logger.LogStatic(reader.ReadToEnd());
                    string execError = errReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(execError))
                        Logger.LogStatic(execError);
                }
            }
            catch (Exception exc)
            {
                Logger.LogStatic(string.Format("Method DersaUtil.SaveEntityToFile, entity {0}, user {1}, error: {2}", id, userName, exc.Message));
            }
        }

        public static string PutString(string src, string userName = null)
        {
            if (userName == null && HttpContext.Current != null)
                userName = HttpContext.Current.User.Identity.Name;
            string Id = Guid.NewGuid().ToString();
            hashTable[userName + Id] = src;
            return Id;
        }
        public static string GetString(string Id, bool viewSource, string userName = null)
        {
            string result = "";
            if (Id.Contains("HIST_"))
            {
                var DM = new DersaLogSqlManager();
                System.Data.DataTable T = DM.ExecuteMethod("ATTRIBUTE_LOG", "GetDescription", new object[] { Id, "Value", userName, DersaUtil.GetPassword(userName) });
                if (T.Rows.Count > 0)
                    result = T.Rows[0][0].ToString();
                return result;
            }
            if (userName == null && HttpContext.Current != null)
                userName = HttpContext.Current.User.Identity.Name;
            if (hashTable[userName + Id] == null)
                return "no value for " + Id;
            result = hashTable[userName + Id].ToString();
            if (!viewSource)
                result = result.Replace("$lt$", "<").Replace("$gt$", ">");
            hashTable.Remove(userName + Id);
            return result;
        }
        public static int GetUserStatus(string user_name, string password)
        {
            if (string.IsNullOrEmpty(user_name))
                return (int)DersaUserStatus.anonimous;

            IParameterCollection Params = new ParameterCollection();
            Params.Add("@login", user_name);
            Params.Add("@password", password);
            SqlManager M = new DersaAnonimousSqlManager();
            int checkresult = M.ExecuteIntMethod("DERSA_USER", "CanAuthorize", Params);
            return checkresult;
        }
        public static string GetActionForParams(string json_params)
        {
            dynamic testObject = JsonConvert.DeserializeObject(json_params);
            if (testObject.json_params != null)
                json_params = testObject.json_params;   //если вызов в старом стиле  (body.json_params = x) попадает в новую оболочку (body и есть json_params)

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
                catch (Exception exc)
                {
                    Logger.LogStatic(exc.Message);
                }
            }
            return null;
        }

        public static string GetAction(string MethodName, int id, string paramString = null)
        {//AllowExecuteJSMethod
            string userName = "localuser";
            StereotypeBaseE target = StereotypeBaseE.GetSimpleInstance(id);
            if (!target.AllowExecuteMethod(userName, MethodName))
                return string.Format("You are not allowed to execute method {0}", MethodName);
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
            if (result is string)
                return result.ToString();
            return JsonConvert.SerializeObject(result);
        }

        public static string SaveDiagramFromJson(string id, string jsonObject)
        {
            try
            {
                object[] diagArray = JsonConvert.DeserializeObject<object[]>(jsonObject);
                var query = from dynamic N in diagArray
                            select new { N.is_visible, N.id, N.left, N.top, N.width, N.height };

                DersaSqlManager DM = new DersaSqlManager();
                IParameterCollection Params = new ParameterCollection();
                foreach (dynamic X in query)
                {
                    int diagram_entity = -1;
                    int entity = -1;
                    bool saveCoords = true;
                    Params.Clear();
                    string S = "";
                    if (!(bool)X.is_visible)
                    {
                        saveCoords = false;
                        if (((string)X.id).Substring(0, 2) == "DE")
                        {
                            diagram_entity = int.Parse(((string)X.id).Split('_')[1]);
                            Params.Add("diagram_entity", diagram_entity);
                            DM.ExecuteIntMethod("DIAGRAM", "DropEntity", Params);
                        }
                    }
                    else if (((string)X.id).Substring(0, 1) == "n")
                    {
                        entity = int.Parse(((string)X.id).Split('_')[1]);
                        Params.Add("diagram", id);
                        Params.Add("entity", entity);
                        Params.Add("left", X.left);
                        Params.Add("top", X.top);
                        Params.Add("w", X.width);
                        Params.Add("h", X.height);
                        DM.ExecuteIntMethod("DIAGRAM", "CreateEntity", Params);
                    }
                    else
                    {
                        diagram_entity = int.Parse(((string)X.id).Split('_')[1]);
                        Params.Add("diagram_entity", diagram_entity);
                        Params.Add("left", X.left);
                        Params.Add("top", X.top);
                        Params.Add("w", X.width);
                        Params.Add("h", X.height);
                        entity = DM.ExecuteIntMethod("DIAGRAM", "UpdateEntity", Params);
                    }
                    if (saveCoords)
                    {
                        StereotypeBaseE objToSaveCoords = StereotypeBaseE.GetSimpleInstance(entity);
                        objToSaveCoords.SaveCoords("", (int)X.left, (int)X.top, (int)X.width, (int)X.height);
                    }
                }
                string result = JsonConvert.SerializeObject(query);
                return result;
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
        public static string CreateDiagram(int parent, string userName)
        {
            IParameterCollection Params = new ParameterCollection();
            Params.Add("@parent", parent);
            Params.Add("@login", userName);
            Params.Add("@password", DersaUtil.GetPassword(userName));
            DersaSqlManager M = new DersaSqlManager();
            int res = M.ExecuteIntMethod("DIAGRAM", "CreateDiagram", Params);
            return res.ToString();
        }
        public static string GetDiagramJson(string id, string userName)
        {
            DersaSqlManager DM = new DersaSqlManager();
            DataTable T = DM.ExecuteMethod("DIAGRAM", "GetEntities", new object[] { id, userName, DersaUtil.GetPassword(userName) });
            int appIndex = 0;
            var query = from DataRow R in T.Rows
                        select new
                        {
                            displayed_name = R["name"],
                            id = R["id"],
                            app_index = appIndex++,
                            left = R["left"],
                            top = R["top"],
                            width = R["width"],
                            height = R["height"],
                            is_selected = false,
                            is_visible = true
                        };
            return JsonConvert.SerializeObject(query);
        }
        public static string GetDiagramEntities(string id, string userName)
        {
            DersaSqlManager DM = new DersaSqlManager();
            DataTable T = DM.ExecuteMethod("DIAGRAM", "GetEntities", new object[] { id, userName, DersaUtil.GetPassword(userName) });
            int appIndex = 0;
            var query = from DataRow R in T.Rows
                        select new
                        {
                            displayed_name = R["name"],
                            id = R["id"],
                            app_index = appIndex++,
                            left = R["left"],
                            top = R["top"],
                            width = R["width"],
                            height = R["height"],
                            is_selected = false,
                            is_visible = true
                        };
            return JsonConvert.SerializeObject(query);
        }
        public static string GetDiagramRelations(string id, string userName)
        {
            DersaSqlManager DM = new DersaSqlManager();
            DataTable T = DM.ExecuteMethod("DIAGRAM", "GetRelations", new object[] { id, userName, DersaUtil.GetPassword(userName) });
            int appIndex = 0;
            var query = from DataRow R in T.Rows
                        select new
                        {
                            id = R["id"],
                            app_index = appIndex++,
                            diagram_entity_a = R["diagram_entity_a"],
                            diagram_entity_b = R["diagram_entity_b"],
                            is_selected = false,
                            is_visible = true
                        };
            return JsonConvert.SerializeObject(query);
        }
        public static string SaveDiagramXml(string id, string xml)
        {
            try
            {
                string decodedXml = xml.Replace("{lt;", "<").Replace("{gt;", ">");
                IParameterCollection Params = new ParameterCollection();
                Params.Add("@diagram", id.Replace("D_", ""));
                Params.Add("@xml", decodedXml);
                string currentUser = System.Web.HttpContext.Current.User.Identity.Name;
                Params.Add("@login", currentUser);
                Params.Add("@password", DersaUtil.GetPassword(currentUser));
                DersaSqlManager M = new DersaSqlManager();
                int res = M.ExecuteIntMethod("DIAGRAM", "SaveFromXml", Params);
                if (res != 0)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(decodedXml);
                    XmlNodeList xNodes = doc.SelectNodes(".//mxCell");
                    //дополнительные действия с объектами внутри диаграммы
                }
                return res.ToString();
            }
            catch(Exception exc)
            {
                Logger.LogStatic(exc.Message);
                return exc.Message;
            }
        }
        public static SchemaEntity[] GetSchema(string JsonContent)
        {
            SchemaEntity[] schemaContent = JsonConvert.DeserializeObject<SchemaEntity[]>(JsonContent);
            return schemaContent;
        }

        public static string CreateEntity(SchemaEntity schemaEntity, string parentId, string userName, string guid = null)
        {
            string stereotypeName = schemaEntity.StereotypeName;
            string entityName = schemaEntity.Name;
            SchemaAttribute[] attrs = schemaEntity.schemaAttributes;
            string entityCreateResult = EntityAddChild(userName, stereotypeName, parentId, 0);//DnD(stereotypeName, parentId, 0);  //"[{\"id\":10000361,\"text\":\"Entity\",\"icon\":\"Entity\",\"name\":\"Entity\"}]"; 
            string resultId = "";
            if (string.IsNullOrEmpty(entityCreateResult))
                return resultId;
            dynamic ES = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(entityCreateResult);
            if (ES == null)
                return resultId;
            if (ES.Count > 0)
            {
                int entId = ES[0].id;
                StereotypeBaseE objToRename = StereotypeBaseE.GetSimpleInstance(entId);
                if (objToRename != null)
                {
                    objToRename.Rename(userName, entityName);
                }
                resultId = entId.ToString();
                //Rename(resultId, entityName);
                //if (guid != null)
                //    DersaUtil.SetGuid(HttpContext.User.Identity.Name, resultId, guid);
            }
            Common.DersaSqlManager DM = new Common.DersaSqlManager();
            if (resultId != "" && attrs != null && attrs.Length > 0)
            {
                for (int a = 0; a < attrs.Length; a++)
                {
                    //NodeControllerAdapter.SetAttribute(DM, "ENTITY$SetAttribute", resultId, attrs[a].Name, attrs[a].Value);
                    SetAttributeValue(DM, userName, AttributeOwnerType.Entity, resultId, attrs[a].Name, 0, attrs[a].Value);
                }
            }
            SchemaEntity[] childEntities = schemaEntity.childEntities;
            for (int c = 0; c < childEntities.Length; c++)
            {
                SchemaEntity childEntity = childEntities[c];
                CreateEntity(childEntity, resultId, userName);
            }

            return resultId;

        }

        public static string AddRelationSimple(string userName, int stereotype, int entity_a, int entity_b)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                IParameterCollection Params = new ParameterCollection(); 
                Params.Add("stereotype", stereotype);
                Params.Add("entity_a", entity_a);
                Params.Add("entity_b", entity_b);
                Params.Add("login", userName);
                Params.Add("password", DersaUtil.GetPassword(userName));
                int result = DM.ExecuteIntMethod("ENTITY","AddRelationSimple", Params);
                //return result.ToString();
                return "";
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }

        public static dynamic ExecMethodResult(int id, string method_name)
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
            Logger.LogStatic("ExecMethodResult clear cache Entity" + ent.Id.ToString());
            //Type nType = Type.GetType("DersaStereotypes." + ent.Stereotype.Name);

            ICompiled cInst = ent.GetCompiledInstance();
            MethodInfo mi = cInst.GetType().GetMethod(method_name);
            if (mi == null)
            {
                string excMessage = "Method " + method_name + " not found ";
                Logger.LogStatic(excMessage);
                throw new Exception(excMessage);
            }
            //string userName = HttpContext.Current.User.Identity.Name;
            //System.Data.DataTable T = M.ExecuteMethod("dbo.ENTITY$GetMethodParams", new object[] { id, method_name, userName, DersaUtil.GetPassword(userName) });
            //string Params = "";
            //if (T.Rows.Count > 0)
            //    Params = T.Rows[0][0].ToString();
            //object[] ParamValues = Util.GetMethodCallParameterValues(Params);
            //return mi.Invoke(cInst, ParamValues);
            return mi.Invoke(cInst, new object[] { });
            //object res = mi.Invoke(cInst, ParamValues);
            //string resultText = "";
            //if (res != null)
            //    resultText = res.ToString();
            //return resultText;
        }

        public static byte[] GenerateWordFile(string json_table, string file_name, string serviceUrl)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
            //Specify the address to be used for the client.
            EndpointAddress address =
               new EndpointAddress(serviceUrl);

            // Create a client that is configured with this address and binding.
            WordGeneratorService.ObjectWcfServiceClient wgsClient = new WordGeneratorService.ObjectWcfServiceClient(binding, address);
            return wgsClient.GenerateWordFile(json_table, file_name);
        }
        public static byte[] GenerateWordFileFromBytes(string json_table, string file_name, byte[] template, string serviceUrl)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
            //Specify the address to be used for the client.
            EndpointAddress address =
               new EndpointAddress(serviceUrl);

            // Create a client that is configured with this address and binding.
            WordGeneratorService.ObjectWcfServiceClient wgsClient = new WordGeneratorService.ObjectWcfServiceClient(binding, address);
            return wgsClient.GenerateWordFileFromBytes(json_table, file_name, template);
        }
        //public static string ObjectList(string class_name, string json_params, string order, int limit, int offset, out int rowcount)
        //{
        //    DiosSqlManager M = new DiosSqlManager();
        //    ObjectFactory F = null;
        //    try
        //    {
        //        F = M.GetFactory(class_name);
        //    }
        //    catch(Exception exc)
        //    {
        //        throw new Exception("Object type " + class_name + " not found");
        //    }
        //    IParameterCollection Params = Util.DeserializeParams(json_params);
        //    string result = F.ListJson(Params, order, limit, offset);
        //    rowcount = F.RowCount;
        //    return result;
        //}

        public static string SetGuid(string userName, string entityId, string guid)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                DM.ExecuteMethod("ENTITY", "SetGuid", new object[] { entityId, guid, DersaUtil.GetPassword(userName) });
                return "";
            }
            catch(Exception exc)
            {
                return exc.Message;
            }
        }

        public static string SetAttributeValue(DersaSqlManager DM, string userName, AttributeOwnerType ownerType, string entityId, string attrName, int attrType, string attrValue)
        {
            SaveEntityToFile(int.Parse(entityId), userName, attrName);
            IParameterCollection Params = new ParameterCollection();
            string className = "";
            switch (ownerType)
            {
                case AttributeOwnerType.Entity:
                    className = "ENTITY";
                    Params.Add("@entity", entityId);
                    break;
                case AttributeOwnerType.Relation:
                    className = "RELATION";
                    Params.Add("@relation", entityId);
                    break;
            }
            Params.Add("@attr_name", attrName);
            Params.Add("@attr_value", attrValue);
            Params.Add("@login", userName);
            Params.Add("@password", DersaUtil.GetPassword(userName));
            int res = 0;
            Params.Add("@attr_type", attrType);
            res = DM.ExecuteIntMethod(className, "SetAttribute", Params);
            if(res == 5)
            {
                Params["@attr_type"].Value = 5;
                Params["@attr_value"].Value = Cryptor.Encrypt(attrValue, userName);
                res = DM.ExecuteIntMethod(className, "SetAttribute", Params);
            }
            return "";
        }
        
        public static string GetFileExtension(string userName, int entityId, string attrName, int attrType)
        {
            DersaSqlManager DM = new DersaSqlManager();
            System.Data.DataTable T = DM.ExecuteMethod("ENTITY", "GetFileExtensionForAttribute", new object[] { entityId, attrName, userName, DersaUtil.GetPassword(userName) });
            if (T == null || T.Rows.Count < 1)
                return ".txt";
            string result = T.Rows[0]["file_extension"].ToString();
            return result;
            //if (attrName.Contains("()"))
            //{
            //    return ".sql";
            //}
            //string fileExtension = ".sql";
            //if (attrName == "Code" || attrName == "Text")
            //    fileExtension = ".cs";
            //else if (attrName == "WordText")
            //    fileExtension = ".html";
            //return fileExtension;
        }

        public static string GetAttributeValue(string userName, int entityId, string attrName, int attrType)
        {
            if (attrName.Contains("()"))
            {
                object res = ExecMethodResult(entityId, attrName.Replace("()", ""));
                if (res == null)
                    return "";
                return res.ToString();
            }
            DersaSqlManager DM = new DersaSqlManager();
            System.Data.DataTable T = DM.ExecuteMethod("ENTITY", "GetAttribute", new object[] { entityId, attrName, userName, DersaUtil.GetPassword(userName) });
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
            UserParams.Add("@password", DersaUtil.GetPassword(userName));
            return M.ExecuteIntMethod("DERSA_USER", "GetPermissions", UserParams);
        }

        public static string GetUserSetting(string userName, string settingName)
        {
            DersaSqlManager DM = new DersaSqlManager();
            IParameterCollection UserParams = new ParameterCollection();
            UserParams.Add("@login", userName);
            UserParams.Add("@password", DersaUtil.GetPassword(userName));
            UserParams.Add("@user_setting_name", settingName);
            System.Data.DataTable VT = DM.ExecuteMethod("DERSA_USER", "GetTextUserSetting", UserParams);
            if (VT == null || VT.Rows.Count < 1)
                throw new Exception(string.Format("Не задано значение для параметра {0}", settingName));
            return VT.Rows[0][0].ToString();
        }

        public static string SetUserSetting(string userName, string settingName, string settingValue)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                System.Data.DataTable T = DM.ExecuteMethod("USER_SETTING", "SetValue", new object[] { settingName, settingValue, userName, DersaUtil.GetPassword(userName) });
                if (T.Rows.Count > 0)
                {
                    return T.Rows[0][0].ToString();
                }
                return "setting was set";
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
        public static string EntitySetAttribute(string userName, int entity, string prop_name, string prop_value)
        {
            DersaSqlManager DM = new DersaSqlManager();
            return SetAttributeValue(DM, userName, AttributeOwnerType.Entity, entity.ToString(), prop_name, 1, prop_value);
            //try
            //{
            //    DersaSqlManager DM = new DersaSqlManager();
            //    System.Data.DataTable T = DM.ExecuteMethod("ENTITY", "SetAttribute", new object[] { entity, prop_name, prop_value, userName, DersaUtil.GetPassword(userName) });
            //    if (T.Rows.Count > 0)
            //    {
            //        return T.Rows[0][0].ToString();
            //    }
            //    return "";
            //}
            //catch(Exception exc)
            //{
            //    return "";
            //}
        }

        public static void EntitySetStereotype(string userName, int entity, string stereotype_name)
        {
            try
            {
                DersaSqlManager DM = new DersaSqlManager();
                DM.ExecuteMethod("ENTITY", "SetStereotype", new object[] { entity, stereotype_name, userName, DersaUtil.GetPassword(userName) });
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
                System.Data.DataTable T = DM.ExecuteMethod("ENTITY", "AddRelation", new object[] { entity, entity_b, stereotype_name, relation_name, source_class, source_ref, userName, DersaUtil.GetPassword(userName) });
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
                IParameterCollection Params = new ParameterCollection();
                Params.Add("dnd_source", src);
                Params.Add("dnd_target", dst);
                Params.Add("options", options);
                Params.Add("login", userName);
                Params.Add("password", GetPassword(userName));
                DataTable T = DM.ExecuteMethod("ENTITY", "OnDnD", Params);

//                DataTable T = DM.ExecuteMethod("ENTITY$OnDnD", new object[] { src, dst, options, userName, GetPassword(userName) });
                string result = JsonConvert.SerializeObject(T);
                return result;
            }
            catch(Exception exc)
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

        //public static IParameterCollection DeserializeParams(string json_params)
        //{
        //    IParameterCollection Params = new ParameterCollection();
        //    if (json_params != null && json_params != "")
        //    {
        //        DIOS.Common.Parameter[] res = JsonConvert.DeserializeObject(json_params, typeof(DIOS.Common.Parameter[])) as DIOS.Common.Parameter[];
        //        for (int p = 0; p < res.Length; p++)
        //        {
        //            if (res[p].Predicate != null && res[p].Predicate.ToLower() == "in")
        //            {
        //                Newtonsoft.Json.Linq.JArray jarrayValue = (Newtonsoft.Json.Linq.JArray)res[p].Value;
        //                object[] paramValue = new object[jarrayValue.Count];
        //                int i = 0;
        //                foreach (object val in jarrayValue)
        //                {
        //                    paramValue[i++] = val;
        //                }
        //                res[p].Value = paramValue;
        //                res[p].Predicate = "in";
        //            }
        //            if (res[p].Name != "id")
        //                Params.Add(res[p]);
        //        }
        //    }
        //    return Params;
        //}

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

            return DersaUtil.Convert(descrVal, T);
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
                            if (value.ToString() == "" && fi.FieldType != typeof(string))
                                continue;
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
                DersaStereotypes.Attribute res = new DersaStereotypes.Attribute((IDersaEntity)obj);
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
                DersaStereotypes.Entity res = new DersaStereotypes.Entity((IDersaEntity)obj);
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
