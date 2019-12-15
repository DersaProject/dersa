using System;
using Dersa.Interfaces;

namespace Dersa.Common
{
    public class Relation : StereotypedObject, IRelation
    {
        public Relation()
        {
        }
        public Relation(DersaSqlManager sm) : base(sm)
        {
        }
        public Relation(System.Data.DataRow r, DersaEntity _AEntity, DersaEntity _BEntity, DersaSqlManager M)
            : this(M)
        {
            if (r != null)
            {
                string a_id = r["a"].ToString();
                string b_id = r["b"].ToString();
                // = new SqlManager();
                if (_AEntity != null)
                {
                    this.A = _AEntity;
                }
                else
                {
                    if (a_id != "")
                    {
                        this.A = new DersaEntity(M.GetEntity(a_id), null, M, AddChildrenMode.Never, false);
                    }
                }
                if (_BEntity != null)
                {
                    this.B = _BEntity;
                }
                else
                {
                    if (b_id != "")
                    {
                        //throw new Exception(b_id);
                        if (CachedObjects.CachedEntities[r["b"]] != null)
                            this.B = (DersaEntity)CachedObjects.CachedEntities[r["b"]];
                        else
                            this.B = new DersaEntity(M.GetEntity(b_id), null, M, AddChildrenMode.Always, a_id != b_id);
                    }
                }
                this._stereotype_name = (string)r["stereotype_name"];
                this._id = (int)r["relation"];
            }
        }


        protected int _type;
        protected DersaEntity _a;
        protected DersaEntity _b;
        protected ChildrenCollection _diagramRelations = new ChildrenCollection();

        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public DersaEntity A
        {
            get
            {
                return _a;
            }
            set
            {
                if ((_a != null) && (_a.ARelations.Contains(this))) _a.ARelations.Remove(this);
                _a = value;
                if (_a != null) _a.ARelations.Add(this);
            }
        }
        public DersaEntity B
        {
            get
            {
                return _b;
            }
            set
            {
                if ((_b != null) && (_b.BRelations.Contains(this))) _b.BRelations.Remove(this);
                _b = value;
                if (_b != null) _b.BRelations.Add(this);
            }
        }
        public/* override */string Name
        {
            get
            {
                return _stereotype_name;
            }
        }
        public override String ToString()
        {
            return "[" + _a.Name + "] - [" + _b.Name + "]";
        }
        public void Dispose()
        {
            if ((_a != null) && (_a.ARelations.Contains(this))) _a.ARelations.Remove(this);
            if ((_b != null) && (_b.BRelations.Contains(this))) _b.BRelations.Remove(this);
            _a = null;
            _b = null;
        }
        IDersaEntity IRelation.A
        {
            get
            {
                return (IDersaEntity)A;
            }
        }
        IDersaEntity IRelation.B
        {
            get
            {
                return (IDersaEntity)B;
            }
        }
        public ICompiledRelation GetInstance()
        {
            return (ICompiledRelation)base.GetCompiledInstance();
        }
    }
}