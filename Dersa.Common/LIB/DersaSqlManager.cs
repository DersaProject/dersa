using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dersa.Common;
using System.Xml;
using Microsoft.Extensions.Configuration;

namespace Dersa.Common
{
    public class DersaAnonimousSqlManager : DersaSqlManager
    {
        protected override bool Anonimous
        {
            get
            {
                return true;
            }
        }
    }
    public class DersaUserSqlManager : DIOS.Common.SqlManager
    {
        public DersaUserSqlManager(): this(DIOS.Common.SqlManager.SqlBrand) { }
        public DersaUserSqlManager(DIOS.Common.SqlBrand sqlBrand): base(sqlBrand) { }
        protected override string GetConnectionString()
        {
            return this.InitConnectionString;
        }
        protected override string GetConnectionNodeName()
        {
            return "DersaUserConnection";
        }
    }
    public class DersaSqlManager: DIOS.Common.SqlManager
    {
        protected override void LogSqlActivity(string query, string methodName)
        {
            DIOS.Common.Logger.LogStatic(query);
            //dbManager.ExecuteMethod("ACTIVITY_LOG$Log", new object[] { query.Replace("'", "''"), methodName});
        }

        public DersaSqlManager(DIOS.Common.SqlBrand sqlBrand):base(sqlBrand)
        {
        }
        public DersaSqlManager()
        {
            Stereotypes = new Hashtable();
        }
        protected override string GetConnectionNodeName()
        {
            return "DersaConnection"; 
        }
        

        private System.Collections.Hashtable Stereotypes;
        public System.Data.DataTable GetEntity(string ID)
        {
            return GetSqlObject("ENTITY_VIEW", "entity", ID);
        }
        public System.Data.DataTable GetEntityChildren(string ID)
        {
            return GetSqlObject("ENTITY_VIEW", "parent", ID);
        }

        public System.Data.DataTable GetEntityARelations(string ID)
        {
            return GetSqlObject("RELATION_VIEW", "a", ID);
        }

        public System.Data.DataTable GetEntityBRelations(string ID)
        {
            return GetSqlObject("RELATION_VIEW", "b", ID);
        }

        //public Stereotype GetStereotype(string ID)
        //{
        //    if (Stereotypes.ContainsKey(ID))
        //        return Stereotypes[ID] as Stereotype;
        //    else
        //    {
        //        System.Data.DataTable t = GetSqlObject("STEREOTYPE", "stereotype", ID);
        //        Stereotype S = new Stereotype(t);// (ObjectClass.GetObjectClass(t));
        //        Stereotypes.Add(ID, S);
        //        return S;
        //    }
        //}
        public System.Data.DataTable GetRelation(string ID)
        {
            return GetSqlObject("RELATION_VIEW", "relation", ID);
        }

        public System.Data.DataTable GetAttribute(string ID)
        {
            return GetSqlObject("ATTRIBUTE_VIEW", "attribute", ID);
        }

        public System.Data.DataTable GetAttributesForEntity(string ID)
        {
            return GetSqlObject("ATTRIBUTE_VIEW", string.Format("where owner_class = 'ENTITY' and owner_ref = {0}", ID));
        }

        public System.Data.DataTable GetOperationsForStereotype(string ID)
        {
            return GetSqlObject("OPERATION", string.Format("where stereotype = {0}", ID));
        }

        public System.Data.DataTable GetAttributesForRelation(string ID)
        {
            return GetSqlObject("ATTRIBUTE_VIEW", string.Format("where owner_class = 'RELATION' and owner_ref = {0}", ID));
        }

    }
}