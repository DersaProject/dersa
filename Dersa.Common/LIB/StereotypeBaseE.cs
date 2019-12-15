using System;
using Dersa.Interfaces;
using Dersa.Common;
using System.Data;
using System.Reflection;
using Newtonsoft.Json;
using DIOS.Common;
using DIOS.Common.Interfaces;

namespace DersaStereotypes
{
    public class StereotypeBaseE : ICompiledEntity
    {
        public StereotypeBaseE() { }

        public static StereotypeBaseE GetSimpleInstance(int id)
        {
            DersaSqlManager M = new DersaSqlManager();
            DataTable ET = M.GetEntity(id.ToString());
            if (ET == null || ET.Rows.Count < 1)
                return null;
            string typeName = "DersaStereotypes." + ET.Rows[0]["stereotype_name"].ToString();
            Type dType = DersaUtil.GetDynamicType(typeName);
            if (dType == null)
                return null;
            object inst = Activator.CreateInstance(dType, new object[] { });
            StereotypeBaseE res = inst as StereotypeBaseE;
            res._id = id;
            if (ET.Rows[0]["parent"] != DBNull.Value)
            {
                dynamic parentId = ET.Rows[0]["parent"];
                res._parent = GetSimpleInstance(parentId);
            }
            return res;
        }

        protected IDersaEntity _object;
        public IDersaEntity Object
        {
            get { return _object; }
        }
        #region Наследуемые свойства
        #region Name
        protected System.String _name;
        public virtual System.String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion
        #region Id
        protected System.Int32 _id;
        public virtual System.Int32 Id
        {
            get { return _id; }
        }
        #endregion
        #region Parent
        protected Dersa.Interfaces.ICompiledEntity _parent;
        public virtual Dersa.Interfaces.ICompiledEntity Parent
        {
            get
            {
                if (_parent == null)
                {
                    Dersa.Interfaces.IDersaEntity parent = null;
                    if (_object != null)
                        parent = _object.Parent;
                    if (parent != null) _parent = parent.GetInstance();
                    return _parent;
                }
                else
                {
                    return _parent;
                }
            }
        }
        #endregion
        #region Children
        protected System.Collections.IList _children;
        public virtual System.Collections.IList Children
        {
            get
            {
                if (_children == null)
                {
                    _children = _object.ChildrenInstance();
                    return _children;
                }
                else
                {
                    return _children;
                }
            }
        }
        #endregion
        #region ARelations
        protected System.Collections.IList _aRelations;
        public virtual System.Collections.IList ARelations
        {
            get
            {
                if (_aRelations == null)
                {
                    _aRelations = _object.ARelationsInstance();
                    return _aRelations;
                }
                else
                {
                    return _aRelations;
                }
            }
        }
        #endregion
        #region BRelations
        protected System.Collections.IList _bRelations;
        public virtual System.Collections.IList BRelations
        {
            get
            {
                if (_bRelations == null)
                {
                    _bRelations = _object.BRelationsInstance();
                    return _bRelations;
                }
                else
                {
                    return _bRelations;
                }
            }
        }
        #endregion
        #endregion
        #region Наследуемые методы
        public void SetParent(Dersa.Interfaces.ICompiledEntity parent)
        {
            this._parent = parent;
        }
        public void SetARelations(System.Collections.IList aRelations)
        {
            this._aRelations = aRelations;
        }
        public void SetBRelations(System.Collections.IList bRelations)
        {
            this._bRelations = bRelations;
        }
        public void SetChildren(System.Collections.IList children)
        {
            this._children = children;
        }

        public void Reinitialize()
        {
            Dersa.Common.DersaSqlManager M = new Dersa.Common.DersaSqlManager();
            System.Data.DataTable T = M.GetEntity(this.Id.ToString());
            this._object = new Dersa.Common.DersaEntity(T, M);
            this._parent = null;
            this._aRelations = null;
            this._bRelations = null;
            this._children = null;
            ClearCache();
        }

        public void ClearCache()
        {
            CachedObjects.CachedCompiledInstances[this.Object.StereotypeName + this.Id.ToString()] = null;
            CachedObjects.CachedCompiledInstances[this.Id] = null;
        }

        public static void DropDiagram(string diagram_id, string userName)
        {
            DersaSqlManager DM = new DersaSqlManager();
            DM.ExecuteMethod("ENTITY", "Remove", new object[] { diagram_id, userName, DersaUtil.GetPassword(userName), 0 });
        }

        public static void DropRelation(int id, string userName)
        {
            DersaSqlManager DM = new DersaSqlManager();
            DM.ExecuteMethod("ENTITY", "Remove", new object[] { id, userName, DersaUtil.GetPassword(userName), 0 });
        }

        public virtual string AddChild(string userName, string stereotypeName)
        {
            return "";
        }

        public virtual string MoveChild(string userName, string src)
        {
            try
            {
                if (!this.AllowModifyChildren())
                    return "";
                return DersaUtil.EntityAddChild(userName, src, this.Id.ToString(), 0);
            }
            catch
            {
                return "";
            }
        }

        public virtual string CopyChild(string userName, string src, int options)
        {
            try
            {
                if (!this.AllowModifyChildren())
                    return "";
                return DersaUtil.EntityAddChild(userName, src, this.Id.ToString(), options);
            }
            catch
            {
                return "";
            }
        }

        public virtual bool AllowExecuteMethod(string userName, string methodName)
        {
            return true;
        }

        public virtual bool AllowModifyChildren()
        {
            return true;
        }

        public virtual bool AllowDrop()
        {
            return true;
        }

        public virtual string Rename(string userName, string objectName)
        {
            try
            {
                if (this.Parent != null && !(this.Parent as StereotypeBaseE).AllowModifyChildren())
                    return "";
                DersaSqlManager DM = new DersaSqlManager();
                IParameterCollection Params = new ParameterCollection();
                Params.Add("entity", this.Id);
                Params.Add("name", objectName);
                Params.Add("login", userName);
                Params.Add("password", DersaUtil.GetPassword(userName));
                string result = JsonConvert.SerializeObject(DM.ExecuteMethod("ENTITY", "Rename", Params));
//                string result = JsonConvert.SerializeObject(DM.ExecuteMethod("ENTITY$Rename", new object[] { this.Id, objectName, userName, DersaUtil.GetPassword(userName) }));
                return result;
            }
            catch
            {
                return "";
            }
        }

        public virtual void Drop(string userName, int options)
        {
            try
            {
                if (!this.AllowDrop())
                    return;
                if (this.Parent == null)
                    return;
                if (!(this.Parent as StereotypeBaseE).AllowModifyChildren())
                    return;
                DersaSqlManager DM = new DersaSqlManager();
                DM.ExecuteMethod("ENTITY", "Remove", new object[] { this.Id, userName, DersaUtil.GetPassword(userName), options });
            }
            catch
            {
            }
        }
        #endregion

    }
}

