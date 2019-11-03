using System;
using System.Collections.Generic;
using System.Collections;
using Dersa.Interfaces;

namespace Dersa.Common
{
    public enum AddChildrenMode : int { Always = 0, NotPackage = 1, Never = 2 } 
    public class TreeNode: StereotypedObject, ITreeNode
    {
        public static TreeNode Copy(TreeNode src)
        {
            TreeNode N = new TreeNode();// (src.ObjectClass, null);
            N._id = src.Id;
            N._name = src.Name;
            N._stereotype_name = src.StereotypeName;
            N._children = new EntityChildrenCollection();
            foreach (TreeNode ch in src.Children)
                N._children.Add(Copy(ch));
            return N;

        }
        public TreeNode()
        {
        }
        public TreeNode(DersaSqlManager sm) : base(sm)
        {
        }
        protected EntityChildrenCollection _children = new EntityChildrenCollection();

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

        IChildrenCollection ITreeNode.Children
        {
            get
            {
                return Children;
            }
        }

    }
    public class DersaEntity : TreeNode, IDersaEntity, ITreeNode
    {

        public DersaEntity()
        {
        }
        public DersaEntity(DersaSqlManager sm):base(sm)
        {
        }

        public DersaEntity(System.Data.DataTable t, DersaSqlManager sm)
            : this(t, null, sm, AddChildrenMode.Always, true)
        {
        }


        public DersaEntity(System.Data.DataTable t, DersaEntity _Parent, DersaSqlManager M, AddChildrenMode AddChildren, bool AddRelations)
            : this(M)
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
                            this.Parent = (DersaEntity)CachedObjects.CachedEntities[r["parent"]];
                        else
                            this.Parent = new DersaEntity(M.GetEntity(ParentID), null, M, AddChildrenMode.NotPackage, false);
                    }
                }
                string StereotypeID = r["stereotype"].ToString();
                this._id = (int)r["entity"];
                this._name = (string)r["name"];
                this._stereotype_name = (string)r["stereotype_name"];


                if (AddChildren == AddChildrenMode.Always || (AddChildren == AddChildrenMode.NotPackage && this.StereotypeName != "Package"))
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
                                this._children.Add(new DersaEntity(M.GetEntity(cr["entity"].ToString()), this, M, AddChildrenMode.NotPackage, false));
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
                DersaEntity ent = new DersaEntity(t, M);
                CachedObjects.CachedCompiledInstances[ent.StereotypeName + ids_str[i]] = null;
                ICompiledEntity cInst = (ICompiledEntity)ent.GetCompiledInstance();
                eArr[i] = cInst;
                entities.Add(cInst);
            }
            Type sUtilType = DersaUtil.GetDynamicType("DersaStereotypes.StereotypeUtil");//typeof(DersaStereotypes.StereotypeUtil);
            if (sUtilType == null)
                throw new Exception("Class StereotypeUtil not found");
            IEntityComparerProvider CP = System.Activator.CreateInstance(sUtilType) as IEntityComparerProvider;
            if(CP == null)
                throw new Exception("Class StereotypeUtil is not IDersaEntityComparerProvider");
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
        protected DersaEntity _parent = null;

        private ChildrenCollection _diagrams = new ChildrenCollection();
        private ChildrenCollection _aRelations = new ChildrenCollection();
        private ChildrenCollection _bRelations = new ChildrenCollection();
        protected ChildrenCollection _diagramEntities = new ChildrenCollection();

        protected int _rank;

        public DersaEntity Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if ((_parent != null) && (_parent._children.Contains(this)))
                {
                    _parent._children.Remove(this);
                }
                    _parent = value;
                if (_parent != null)
                {
                    _parent._children.Add(this);
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
        public int ParentId
        {
            get
            {
                if (Parent != null) return Parent.Id;
                return 0;
            }
            set
            {
            //    Parent = Manager.GetObject("ENTITY", value) as Entity;
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
                if (_parent != null) return _parent._children.IndexOf(this);
                return 0;
            }
        }
        int IDersaEntity.Index
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
                return _rank;
            }
            set
            {
                if (_rank != value)
                {
                    _rank = value;
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
        IChildrenCollection IDersaEntity.ARoles
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
        IChildrenCollection IDersaEntity.BRoles
        {
            get
            {
                return this.BRoles;
            }
        }
        public/* override*/ int CompareTo(Object y)
        {
            if (!(y is DersaEntity)) return 0;
            return this.Name.CompareTo(((DersaEntity)y).Name);
        }
        public void Dispose()
        {
            if ((_parent != null) && (_parent._children.Contains(this)))
            {
                _parent.Children.Remove(this);
            }
            _parent = null;
        }
        IDersaEntity IDersaEntity.Parent
        {
            get
            {
                return (IDersaEntity)Parent;
            }
        }
        IChildrenCollection IDersaEntity.ARelations
        {
            get
            {
                return ARelations;
            }
        }
        IChildrenCollection IDersaEntity.BRelations
        {
            get
            {
                return BRelations;
            }
        }
        IChildrenCollection IDersaEntity.Children
        {
            get
            {
                return Children;
            }
        }
        IChildrenCollection IDersaEntity.Diagrams
        {
            get
            {
                return Diagrams;
            }
        }
        ICompiledEntity IDersaEntity.GetInstance()
        {
            return (ICompiledEntity)base.GetCompiledInstance();
        }
        IList IDersaEntity.ARelationsInstance()
        {
            return ARelations.GetInstance();
        }
        IList IDersaEntity.BRelationsInstance()
        {
            return BRelations.GetInstance();
        }
        IList IDersaEntity.ChildrenInstance()
        {
            return Children.GetInstance();
        }
        IChildrenCollection ITreeNode.Children
        {
            get
            {
                return Children;
            }
        }
    }

}