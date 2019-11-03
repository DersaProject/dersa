using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Dersa.Common;
using Dersa.Interfaces;
using System.Reflection;

namespace Dersa.Models
{
    public class EntityControllerAdapter
    {
        public string Find(string srchval, int root_entity, int entity)
        {
            DersaSqlManager DM = new DersaSqlManager();
            string userName = HttpContext.Current.User.Identity.Name;
            object rootArg = null;
            if (root_entity > 0)
                rootArg = root_entity;
            System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$FindNext", new object[] { entity, srchval, rootArg, 8, userName, DersaUtil.GetPassword(userName) });
            System.Data.DataRow lastRow = null;
            if (T.Rows.Count > 0)
                lastRow = T.Rows[T.Rows.Count - 1];
            var query =
                (from System.Data.DataRow R in T.Rows
                select new
                {
                    Name = R[0].ToString() + " " + R[1].ToString(),
                    Value = "",
                    ReadOnly = true,
                    Type = -1,
                    ControlType = "button",
                    WriteUnchanged = true,
                    ChildFormAttrs = new
                    {
                        Height = 400,
                        Width = 800,
                        DisplayValue = "Go",
                        InfoLink = "",
                        OnClick = "javascript:GoToNode(" + R[0].ToString() + ")",
                        SaveLink = "Node/Close", 
                        ActionAfterExec = "close"
                    }

                }).Union(
                    from object x in new object[] { 1 }
                    where lastRow != null
                    select new
                    {
                        Name = "find",
                        Value = "",
                        ReadOnly = true,
                        Type = -1,
                        ControlType = "button",
                        WriteUnchanged = true,
                        ChildFormAttrs = new
                        {
                            Height = 400,
                            Width = 800,
                            DisplayValue = "Next",
                            InfoLink = "Entity/Find?srchval=" + srchval + "&root_entity=" + root_entity.ToString() + "&entity=" + lastRow[0].ToString(),
                            OnClick = "",
                            SaveLink = "Node/Close",
                            ActionAfterExec = "close"
                        }

                    });
            string result = entity < 0? T.Rows[0][0].ToString() : JsonConvert.SerializeObject(query);
            return result;
        }

        public string GetPath(int id, int for_system)
        {
            DersaSqlManager DM = new DersaSqlManager();
            System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$GetPath", new object[] { id, 1/*for_system*/ });
            string result = "";
            if (T.Rows.Count > 0)
                result = T.Rows[0][0].ToString();
            return result;
        }

        public string Range(string ids)
        {
            DersaSqlManager M = new DersaSqlManager();
            //M.SetDatabaseName("1gb_d-ersa", true);
            string[] ids_str = ids.Split(',');
            List<ICompiledEntity> entities = DersaEntity.Range(ids_str, M);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (ICompiled e in entities)
            {
                sb.Append(e.Name);
                sb.Append(", ");
            }
            return sb.ToString();
        }
        public string Test(int id)
        {
            DersaSqlManager M = new DersaSqlManager();
            //M.SetDatabaseName("1gb_d-ersa", true);
            System.Data.DataTable t = M.GetEntity(id.ToString());
            if (t == null)
                throw new Exception(string.Format("Table is null for entity {0}", id));
            if (t.Rows.Count < 1)
                throw new Exception(string.Format("Table is empty for entity {0}", id));
            DersaEntity ent = new DersaEntity(t, M);
            ICompiled cInst = ent.GetCompiledInstance();
            string MethodName = "sql_Generate";
            MethodInfo mi = /*typeof(DersaStereotypes.Entity)*/cInst.GetType().GetMethod(MethodName);
            object[] ParamValues = new object[] {false, true, true, true, true};
//            object[] ParamValues = new object[] { true, true };
            object res = mi.Invoke(cInst, ParamValues);
            return JsonConvert.SerializeObject(res);
        }
    }
}