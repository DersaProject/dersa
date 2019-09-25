using System;
using System.Reflection;
using System.Collections;
using System.Data.SqlClient;
using Dersa.Interfaces;
using Dersa.Common;
using System.Runtime.Serialization;
using DIOS.Common;

namespace Dersa.Common
{
    public class Relation : StereotypedObject, IRelation
    {
        public Relation()
        {
        }
        public Relation(ObjectClass objectClass, DersaSqlManager sm) : base(objectClass, sm)
        {
        }
        public Relation(System.Data.DataRow r, Entity _AEntity, Entity _BEntity, DersaSqlManager M)
            : this(ObjectClass.GetObjectClass(r), M)
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
                        this.A = new Entity(M.GetEntity(a_id), null, M, AddChildrenMode.Never, false);
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
                            this.B = (Entity)CachedObjects.CachedEntities[r["b"]];
                        else
                            this.B = new Entity(M.GetEntity(b_id), null, M, AddChildrenMode.Always, a_id != b_id);
                    }
                }
                string StereotypeID = r["stereotype"].ToString();
                if (StereotypeID != "")
                {
                    this.Stereotype = M.GetStereotype(StereotypeID);
                }
                this.Id = (int)r["relation"];
            }
        }


        protected int _type;
        protected Entity _a;
        protected Entity _b;
        protected ChildrenCollection _diagramRelations = new ChildrenCollection();

        public int Type
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (int)GetBackUpProperty("Type");
                }
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public Entity A
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (Entity)GetBackUpProperty("A");
                }
                return _a;
            }
            set
            {
                if ((_a != null) && (_a.ARelations.Contains(this))) _a.ARelations.Remove(this);
                _a = value;
                if (_a != null) _a.ARelations.Add(this);
            }
        }
        //public int AId
        //{
        //    get
        //    {
        //        if (ReturnBackupProperty())
        //        {
        //            return (int)GetBackUpProperty("AId");
        //        }
        //        if (_a != null) return A.Id;
        //        return 0;
        //    }
        //    set
        //    {
        //        A = Manager.GetObject("ENTITY", value) as Entity;
        //    }
        //}
        public Entity B
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (Entity)GetBackUpProperty("B");
                }
                return _b;
            }
            set
            {
                if ((_b != null) && (_b.BRelations.Contains(this))) _b.BRelations.Remove(this);
                _b = value;
                if (_b != null) _b.BRelations.Add(this);
            }
        }
        //public int BId
        //{
        //    get
        //    {
        //        if (ReturnBackupProperty())
        //        {
        //            return (int)GetBackUpProperty("BId");
        //        }
        //        if (_b != null) return B.Id;
        //        return 0;
        //    }
        //    set
        //    {
        //        B = Manager.GetObject("ENTITY", value) as Entity;
        //    }
        //}
        public override string Name
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("Name");
                }
                if (_stereotype == null) return base._name;
                return _stereotype.Name;
            }
        }
        public ChildrenCollection DiagramRelations
        {
            get
            {
                if (!_diagramRelations.AutoSorting)
                {
                    _diagramRelations.Sort();
                    _diagramRelations.AutoSorting = true;
                }
                return _diagramRelations;
            }
        }
        public string FullPath
        {
            get
            {
                return A.FullPath + " / " + this.Name;
            }
        }
        //public void FinishCreation(int bId)
        //{
        //    ThrowIfNotModified();
        //    this.BId = bId;
        //    this.Post();
        //}
        /*
		public IDiagramRelation CreateDiagramRelation(int diagramId, int diagramEntityID)
		{
			DiagramRelation dr = (DiagramRelation)Manager.NewObject("DIAGRAM_RELATION");
			dr.Relation = this;
			dr.DiagramId = diagramId;
			dr.DiagramEntityAId = diagramEntityID;
			return dr;
		}*/
        public static new SqlProperty[] InitializeProperties(string keyName)
        {
            SqlProperty[] tempProperty = StereotypedObject.InitializeProperties(keyName);
            SqlProperty[] sqlProperty = new SqlProperty[tempProperty.Length + 2];
            tempProperty.CopyTo(sqlProperty, 0);
            sqlProperty[sqlProperty.Length - 2] = new SqlProperty("AId", "a", System.Data.SqlDbType.Int, 0);
            sqlProperty[sqlProperty.Length - 2].FieldName = "_a";
            sqlProperty[sqlProperty.Length - 1] = new SqlProperty("BId", "b", System.Data.SqlDbType.Int, 0);
            sqlProperty[sqlProperty.Length - 1].FieldName = "_b";
            return sqlProperty;
        }
        public static new PropertyInfo[] InitializePropertyInfos()
        {
            Type thisType = typeof(Relation);
            PropertyInfo[] tempProperty = StereotypedObject.InitializePropertyInfos();
            PropertyInfo[] newProperty = new PropertyInfo[tempProperty.Length + 5];
            tempProperty.CopyTo(newProperty, 0);
            newProperty[newProperty.Length - 5] = thisType.GetProperty("Type", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 4] = thisType.GetProperty("A", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 3] = thisType.GetProperty("AId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 2] = thisType.GetProperty("B", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 1] = thisType.GetProperty("BId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return newProperty;
        }
        public override String ToString()
        {
            return "[" + _a.Name + "] - [" + _b.Name + "]";
        }
        protected override void BeforeDrop()
        {
            //while (_diagramRelations.Count > 0)
            //{
            //    _diagramRelations[0].Drop();
            //}
            //base.BeforeDrop();
        }
        public void Dispose()
        {
            if ((_a != null) && (_a.ARelations.Contains(this))) _a.ARelations.Remove(this);
            if ((_b != null) && (_b.BRelations.Contains(this))) _b.BRelations.Remove(this);
            /*if (_diagramRelations != null) 
			{
				while(_diagramRelations.Count > 0) 
				{
					((DiagramRelation)_diagramRelations[0]).Dispose();
				}
				//_diagramRelations = null;
			}*/
            _a = null;
            _b = null;
            base.Dispose();
        }
        IEntity IRelation.A
        {
            get
            {
                return (IEntity)A;
            }
        }
        IEntity IRelation.B
        {
            get
            {
                return (IEntity)B;
            }
        }
        public ICompiledRelation GetInstance()
        {
            return (ICompiledRelation)base.GetCompiledInstance();
        }
    }
}