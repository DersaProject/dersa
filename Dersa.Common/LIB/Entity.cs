using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Data.SqlClient;
using Dersa.Interfaces;
using Dersa.Common;
using System.Runtime.Serialization;
using DIOS.Common;

namespace Dersa.Common
{
    public enum AddChildrenMode : int { Always = 0, NotPackage = 1, Never = 2 }
    public class Entity : StereotypedObject, IEntity
    {

        public Entity()
        {
        }
        public Entity(ObjectClass objectClass, DersaSqlManager sm) : base(objectClass, sm)
        {
        }

        public Entity(System.Data.DataTable t, DersaSqlManager sm)
            : this(t, null, sm, AddChildrenMode.Always, true)
        {
        }


        public Entity(System.Data.DataTable t, Entity _Parent, DersaSqlManager M, AddChildrenMode AddChildren, bool AddRelations)
            : this(ObjectClass.GetObjectClass(t), M)
        {
            if (t != null && t.Rows.Count > 0)
            {
                // = new DersaSqlManager();
                System.Data.DataRow r = t.Rows[0];
                if (_Parent != null)
                {
                    this.Parent = _Parent;
                }
                else
                {
                    string ParentID = r["parent"].ToString();
                    if (ParentID != "")
                    {
                        if (CachedObjects.CachedEntities[r["parent"]] != null)
                            this.Parent = (Entity)CachedObjects.CachedEntities[r["parent"]];
                        else
                            this.Parent = new Entity(M.GetEntity(ParentID), null, M, AddChildrenMode.NotPackage, false);
                    }
                }
                string StereotypeID = r["stereotype"].ToString();
                if (StereotypeID != "")
                {
                    this.Stereotype = M.GetStereotype(StereotypeID);
                }
                this.Id = (int)r["entity"];

                if (AddChildren == AddChildrenMode.Always || (AddChildren == AddChildrenMode.NotPackage && this.Stereotype.Name != "Package"))
                {
                    if (this._children == null)
                        this._children = new EntityChildrenCollection();
                    if (this._children.Count < 1)
                    {
                        System.Data.DataTable ChildrenTable = M.GetEntityChildren(this.Id.ToString());
                        if (ChildrenTable.Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow cr in ChildrenTable.Rows)
                            {
                                this._children.Add(new Entity(M.GetEntity(cr["entity"].ToString()), this, M, AddChildrenMode.NotPackage, false));
                            }
                        }
                    }
                }
                if (AddRelations)
                {
                    if (this._aRelations == null)
                        this._aRelations = new ChildrenCollection();
                    if (this._aRelations.Count < 1)
                    {
                        System.Data.DataTable ARelationsTable = M.GetEntityARelations(this.Id.ToString());
                        if (ARelationsTable.Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow cr in ARelationsTable.Rows)
                            {
                                this._aRelations.Add(new Relation(cr, this, null, M));
                            }
                        }
                    }
                    if (this._bRelations == null)
                        this._bRelations = new ChildrenCollection();
                    if (this._bRelations.Count < 1)
                    {
                        System.Data.DataTable BRelationsTable = M.GetEntityBRelations(this.Id.ToString());
                        if (BRelationsTable.Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow cr in BRelationsTable.Rows)
                            {
                                this._bRelations.Add(new Relation(cr, null, this, M));
                            }
                        }
                    }

                }
            }
            CachedObjects.CachedEntities[this.Id] = this;
        }

        public static List<ICompiledEntity> Range(string[] ids_str, DersaSqlManager M)
        {
            List<ICompiledEntity> entities = new List<ICompiledEntity>();
            ICompiledEntity[] eArr = new ICompiledEntity[ids_str.Length];
            for (int i = 0; i < ids_str.Length; i++)
            {
                int entId = int.Parse(ids_str[i]);
                CachedObjects.CachedEntities[entId] = null;
                System.Data.DataTable t = M.GetEntity(ids_str[i]);
                if (t == null)
                    throw new Exception(string.Format("Table is null for entity {0}", ids_str[i]));
                if (t.Rows.Count < 1)
                    throw new Exception(string.Format("Table is empty for entity {0}", ids_str[i]));
                Entity ent = new Entity(t, M);
                CachedObjects.CachedCompiledInstances[ent.Stereotype.Name + ids_str[i]] = null;
                ICompiledEntity cInst = (ICompiledEntity)ent.GetCompiledInstance();
                eArr[i] = cInst;
                entities.Add(cInst);
            }
            Type sUtilType =  Util.GetDynamicType("DersaStereotypes.StereotypeUtil");//typeof(DersaStereotypes.StereotypeUtil);
            if (sUtilType == null)
                throw new Exception("Class StereotypeUtil not found");
            IEntityComparerProvider CP = System.Activator.CreateInstance(sUtilType) as IEntityComparerProvider;
            if(CP == null)
                throw new Exception("Class StereotypeUtil is not IEntityComparerProvider");
            IComparer<ICompiledEntity> eCmpr = CP.GetEntityComparer();
            //entities.Sort(eCmpr);
            //entities.Sort(new EntityComparer());  //быстрая сортировка иногда дает сбой, т.к. не проверяет все сочетания, заменяем ее на "пузырек"
            IComparer<ICompiledEntity> cmp = CP.GetEntityComparer();
            for (int i = 0; i < eArr.Length; i++)
            {
                for (int j = i; j < eArr.Length; j++)
                {
                    if (cmp.Compare(eArr[i], eArr[j]) > 0)
                    {
                        ICompiledEntity tmp = eArr[i];
                        eArr[i] = eArr[j];
                        eArr[j] = tmp;
                    }
                }
            }

            entities.Clear();
            for (int i = 0; i < ids_str.Length; i++)
            {
                entities.Add(eArr[i]);
            }
            return entities;
        }

        //public static Hashtable cachedEntities = new Hashtable();
        //public static Hashtable cachedCompiledInstances = new Hashtable();

        //private bool insertByStereotype = false;
        protected Entity _parent = null;

        private EntityChildrenCollection _children = new EntityChildrenCollection();
        private ChildrenCollection _diagrams = new ChildrenCollection();
        private ChildrenCollection _aRelations = new ChildrenCollection();
        private ChildrenCollection _bRelations = new ChildrenCollection();
        protected ChildrenCollection _diagramEntities = new ChildrenCollection();

        protected int _rank;

        /*public bool InsertByStereotype
		{
			set
			{
				insertByStereotype = value;
			}
		}*/
        public Entity Parent
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (Entity)GetBackUpProperty("Parent");
                }
                //if ((_parent == null)||(_parent == Manager.RootEntity)) return null;
                return _parent;
            }
            set
            {
                if ((_parent != null) && (_parent._children.Contains(this)))
                {
                    _parent._children.Remove(this);
                    //Manager.NotifyRemove(this);
                }
                //if (value == null)
                //    _parent = Manager.RootEntity;
                //else
                    _parent = value;
                if (_parent != null)
                {
                    _parent._children.Add(this);
                    /*if (!insertByStereotype)
					{
						_parent._children.Add(this);
					}
					else
					{
						AutoArrange();
					}*/
                }
            }
        }
        public bool HasSubItems
        {
            get
            {
                if (_children.Count > 0) return true;
                if (_diagrams.Count > 0) return true;
                if (ARelations.Count > 0) return true;
                return false;
            }
        }
        /*private void AutoArrange()
		{
			lock (this)
			{
				insertByStereotype = false;
				int index = 0;
				bool insert = false;
				if (_parent._children.Contains(this))
				{
					_parent._children.Remove(this);
				}
				for (int i = 0; i < _parent._children.Count; i++)
				{
					index = i;
					Entity ent = _parent._children[i];
					if (ent.Stereotype.Id == this.Stereotype.Id)
					{
						if (ent.Name.CompareTo(this.Name) > 0)
						{
							insert = true;
							break;
						}
					}
					if (ent.Stereotype.Id > this.Stereotype.Id)
					{
						insert = true;
						break;
					}
				}
				if (insert)
				{
					_parent._children.Insert(index, this);
				}
				else
				{
					_parent._children.Add(this);
				}
			}
		}*/
        public int ParentId
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (int)GetBackUpProperty("ParentId");
                }
                if (Parent != null) return Parent.Id;
                return 0;
            }
            set
            {
            //    Parent = Manager.GetObject("ENTITY", value) as Entity;
            }
        }

        public EntityChildrenCollection Children
        {
            get
            {
                if (!_children.AutoSorting)
                {
                    _children.Sort();
                    _children.AutoSorting = true;
                }
                return _children;
            }
        }

        public ChildrenCollection Diagrams
        {
            get
            {
                if (!_diagrams.AutoSorting)
                {
                    _diagrams.Sort();
                    _diagrams.AutoSorting = true;
                }
                return _diagrams;
            }
        }
        public ChildrenCollection ARelations
        {
            get
            {
                if (!_aRelations.AutoSorting)
                {
                    _aRelations.Sort();
                    _aRelations.AutoSorting = true;
                }
                return _aRelations;
            }
        }
        public ChildrenCollection BRelations
        {
            get
            {
                if (!_bRelations.AutoSorting)
                {
                    _bRelations.Sort();
                    _bRelations.AutoSorting = true;
                }
                return _bRelations;
            }
        }
        public ChildrenCollection DiagramEntities
        {
            get
            {
                return _diagramEntities;
            }
        }
        public int Index
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (int)GetBackUpProperty("Index");
                }
                if (_parent != null) return _parent._children.IndexOf(this);
                return 0;
            }
        }
        int IEntity.Index
        {
            get
            {
                return this.Index;
            }
        }
        public int Rank
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (int)GetBackUpProperty("Rank");
                }
                return _rank;
            }
            set
            {
                if (_rank != value)
                {
                    _rank = value;
                    locateAfterPost = true;
                }
            }
        }

        public ChildrenCollection ARoles
        {
            get
            {
                ChildrenCollection bRelations = this.BRelations;
                ChildrenCollection aRoles = new ChildrenCollection();
                for (int i = 0; i < bRelations.Count; i++)
                {
                    aRoles.Add(((Relation)bRelations[i]).A);
                }
                return aRoles;
            }
        }
        IChildrenCollection IEntity.ARoles
        {
            get
            {
                return this.ARoles;
            }
        }
        public ChildrenCollection BRoles
        {
            get
            {
                ChildrenCollection aRelations = this.ARelations;
                ChildrenCollection bRoles = new ChildrenCollection();
                for (int i = 0; i < aRelations.Count; i++)
                {
                    bRoles.Add(((Relation)aRelations[i]).B);
                }
                return bRoles;
            }
        }
        IChildrenCollection IEntity.BRoles
        {
            get
            {
                return this.BRoles;
            }
        }
        public string FullPath
        {
            get
            {
                string fullPath = "";
                string coma = "";
                ChildrenCollection ents = GetPath();
                for (int i = 0; i < ents.Count; i++)
                {
                    fullPath += coma + ((Entity)ents[i]).Name;
                    coma = " / ";
                }
                return fullPath;
            }
        }
        public override int CompareTo(Object y)
        {
            if (!(y is Entity)) return 0;
            int result = this.Stereotype.Rank.CompareTo(((Entity)y).Stereotype.Rank);
            if (result != 0) return result;
            result = this.Rank.CompareTo(((Entity)y).Rank);
            if (result != 0) return result;
            return this.Name.CompareTo(((Entity)y).Name);
        }
        public static new SqlProperty[] InitializeProperties(string keyName)
        {
            SqlProperty[] tempProperty = StereotypedObject.InitializeProperties(keyName);
            SqlProperty[] sqlProperty = new SqlProperty[tempProperty.Length + 2];
            tempProperty.CopyTo(sqlProperty, 0);
            sqlProperty[sqlProperty.Length - 2] = new SqlProperty("ParentId", "parent", System.Data.SqlDbType.Int, 0);
            sqlProperty[sqlProperty.Length - 2].FieldName = "_parent";
            sqlProperty[sqlProperty.Length - 1] = new SqlProperty("Rank", "rank", System.Data.SqlDbType.Int, 0);
            return sqlProperty;
        }
        public static new PropertyInfo[] InitializePropertyInfos()
        {
            Type thisType = typeof(Entity);
            PropertyInfo[] tempProperty = StereotypedObject.InitializePropertyInfos();
            PropertyInfo[] newProperty = new PropertyInfo[tempProperty.Length + 4];
            tempProperty.CopyTo(newProperty, 0);
            newProperty[newProperty.Length - 4] = thisType.GetProperty("Parent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 3] = thisType.GetProperty("ParentId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 2] = thisType.GetProperty("Index", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 1] = thisType.GetProperty("Rank", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return newProperty;
        }
        public void DropWithChildren()
        {
            EntityChildrenCollection children = this.Children;
            while (this.Children.Count > 0)
            {
                Entity childEntity = (Entity)this.Children[0];
                childEntity.DropWithChildren();
            }
            this.Drop();
        }
        private void CheckCloneWithChildren(int parentId)
        {
            if (this.Id == parentId) throw new Exception("Нельзя копировать объект в самого себя или в объекты нижнего уровня");
            EntityChildrenCollection children = this.Children;
            for (int i = 0; i < children.Count; i++)
            {
                Entity childEntity = (Entity)children[i];
                childEntity.CheckCloneWithChildren(parentId);
            }
        }
        public IEntity CloneWithChildren(int parentId)
        {
            CheckCloneWithChildren(parentId);
            Entity entity = (Entity)this.Clone();
            entity.ParentId = parentId;
            entity.Post();
            EntityChildrenCollection children = this.Children;
            for (int i = 0; i < children.Count; i++)
            {
                IEntity childEntity = children[i];
                childEntity.CloneWithChildren(entity.Id);
            }
            return entity;
        }
        protected override void BeforeDrop()
        {
            //if (_children.Count > 0) throw new Exception("Сначала удалите объекты нижнего уровня");
            //if (_aRelations.Count > 0) throw new Exception("Сначала удалите объекты связи по A");
            //if (_bRelations.Count > 0) throw new Exception("Сначала удалите объекты связи по B");
            //if (_diagrams.Count > 0) throw new Exception("Сначала удалите диаграммы");
            //while (_diagramEntities.Count > 0)
            //{
            //    _diagramEntities[0].Drop();
            //}
            //base.BeforeDrop();
        }
        /*public void Move(int offset)
		{
			if ((Rank <= 0)&&(offset == -1)) return;
			if ((Rank >= _parent.Children.Count - 1)&&(offset == 1)) return;
			int index = 0;
			for (int i = 0; i < _parent.Children.Count; i++)
			{
				if (_parent.Children[i].Equals(this)) 
				{
					this.Manager.NotifyRemove(this);
					this._parent.Children.Insert(i + offset, this);
					this.Manager.NotifyUpdate(this);
					if (offset == -1) index = i + offset; else index = i;
					break;
				}
			}
			for (int i = index; i < _parent.Children.Count; i++)
			{
				((Entity)_parent.Children[i]).Post();
			}
		}*/
        public void Move(int parentID, bool toParent)
        {
            //lock (this)
            //{
            //    Load();
            //    Entity parent = Manager.GetObject("ENTITY", parentID) as Entity;
            //    Move(parent);
            //    Post();
            //}
        }
        public IDiagram CreateDiagram()
        {/*
			Diagram diagram = (Diagram)Manager.NewObject("DIAGRAM");
			diagram.Name = "New Diagram";
			diagram.Entity = this;
			diagram.Post();
			return diagram;*/
            return null;
        }
        private void Move(Entity parent)
        {
            if ((parent == this) || (parent == Parent)) return;
            //insertByStereotype = true;
            Parent = parent;
        }
        public ChildrenCollection GetPath()
        {
            ChildrenCollection entities = new ChildrenCollection();
            return this.GetPath(entities);
        }
        IChildrenCollection IEntity.GetPath()
        {
            return this.GetPath();
        }
        private ChildrenCollection GetPath(ChildrenCollection entities)
        {
            entities.Insert(0, this);
            if (this.Parent != null)
            {
                entities = Parent.GetPath(entities);
            }
            return entities;
        }
        protected override void LoacateAfterPost()
        {
            base.LoacateAfterPost();
            _parent._children.Sort();
        }
        public void Dispose()
        {
            if ((_parent != null) && (_parent._children.Contains(this)))
            {
                _parent.Children.Remove(this);
            }
            _parent = null;
            if (_diagramEntities != null)
            {
                while (_diagramEntities.Count > 0)
                {
                    _diagramEntities[0].Dispose();
                }
                //_diagramEntities = null;
            }
            if (_diagrams != null)
            {
                while (_diagrams.Count > 0)
                {
                    _diagrams[0].Dispose();
                }
                //_diagrams = null;
            }
            if (_aRelations != null)
            {
                while (_aRelations.Count > 0)
                {
                    _aRelations[0].Dispose();
                }
                //_aRelations = null;
            }
            if (_bRelations != null)
            {
                while (_bRelations.Count > 0)
                {
                    _bRelations[0].Dispose();
                }
                //_bRelations = null;
            }
            base.Dispose();
        }
        string IEntity.GetProperty(string propName)
        {
            IAttribute a = this.GetAttribute("Properties");
            if (a != null)
            {
                string[] props = ((string)a.Value).Split(',');
                foreach (string str in props)
                {
                    string[] strArr = str.Split('=');
                    if ((strArr != null) && (strArr.Length == 2) && (strArr[0].Trim() == propName))
                        return str.Split('=')[1];
                }
            }
            if (Parent != null)
                return ((IEntity)this).Parent.GetProperty(propName);
            return null;
        }
        IEntity IEntity.Parent
        {
            get
            {
                return (IEntity)Parent;
            }
        }
        IChildrenCollection IEntity.ARelations
        {
            get
            {
                return ARelations;
            }
        }
        IChildrenCollection IEntity.BRelations
        {
            get
            {
                return BRelations;
            }
        }
        IChildrenCollection IEntity.Children
        {
            get
            {
                return Children;
            }
        }
        IChildrenCollection IEntity.Diagrams
        {
            get
            {
                return Diagrams;
            }
        }
        ICompiledEntity IEntity.GetInstance()
        {
            return (ICompiledEntity)base.GetCompiledInstance();
        }
        IList IEntity.ARelationsInstance()
        {
            return ARelations.GetInstance();
        }
        IList IEntity.BRelationsInstance()
        {
            return BRelations.GetInstance();
        }
        IList IEntity.ChildrenInstance()
        {
            return Children.GetInstance();
        }
    }

}