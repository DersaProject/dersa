using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dersa.Common;
using System.Xml;

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
        static DersaUserSqlManager()
        {
            SqlBrand = DIOS.Common.SqlBrand.MSSqlServer;
        }
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
        static DersaSqlManager()
        {
            SqlBrand = DIOS.Common.SqlBrand.MSSqlServer;
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
            return GetSqlObject("ENTITY", "entity", ID);
        }
        public System.Data.DataTable GetEntityChildren(string ID)
        {
            return GetSqlObject("ENTITY", "parent", ID);
        }

        public System.Data.DataTable GetEntityARelations(string ID)
        {
            return GetSqlObject("RELATION", "a", ID);
        }

        public System.Data.DataTable GetEntityBRelations(string ID)
        {
            return GetSqlObject("RELATION", "b", ID);
        }

        public Stereotype GetStereotype(string ID)
        {
            if (Stereotypes.ContainsKey(ID))
                return Stereotypes[ID] as Stereotype;
            else
            {
                System.Data.DataTable t = GetSqlObject("STEREOTYPE", "stereotype", ID);
                Stereotype S = new Stereotype(ObjectClass.GetObjectClass(t));
                Stereotypes.Add(ID, S);
                return S;
            }
        }
        public System.Data.DataTable GetRelation(string ID)
        {
            return GetSqlObject("RELATION", "relation", ID);
        }

        public System.Data.DataTable GetAttribute(string ID)
        {
            return GetSqlObject("ATTRIBUTE", "attribute", ID);
        }

        public System.Data.DataTable GetAttributesForEntity(string ID)
        {
            return GetSqlObject("ATTRIBUTE", string.Format("where owner_class = 'ENTITY' and owner_ref = {0}", ID));
        }

        public System.Data.DataTable GetOperationsForStereotype(string ID)
        {
            return GetSqlObject("OPERATION", string.Format("where stereotype = {0}", ID));
        }

        public System.Data.DataTable GetAttributesForRelation(string ID)
        {
            return GetSqlObject("ATTRIBUTE", string.Format("where owner_class = 'RELATION' and owner_ref = {0}", ID));
        }

    }
}