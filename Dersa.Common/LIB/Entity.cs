using System;
using DIOS.Common;
using DIOS.Common.Interfaces;
using DIOS.ObjectLib;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Dersa.Common
{
    [LocalizedName("ENTITY")]
    [DataContract]
    public class Entity : DIOS.ObjectLib.Object
    {

        public Entity() : base() { }

        public Entity(UniStructView v, ObjectFactory f) : base(v, f) { }

        public const string EntityClassName = "ENTITY";
        #region entity
        protected SqlInt32 _entity;
        [DataMember]
        [ObjectPropertyAttribute("#", true, false, 0, false, true)]
        public SqlInt32 entity
        {
            get
            {
                return _entity;
            }
            set
            {
                if (!this.changedFields.Contains("_entity") && this._entity != value)
                    this.changedFields.Add("_entity", this._entity);
                _entity = value;
            }
        }
        #endregion
        #region parent
        protected SqlInt32 _parent;
        [DataMember]
        [ObjectPropertyAttribute("entity", false, false, 0, false, false)]
        public SqlInt32 parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (!this.changedFields.Contains("_parent") && this._parent != value)
                    this.changedFields.Add("_parent", this._parent);
                _parent = value;
            }
        }
        #endregion
        #region occur
        protected SqlDateTime _occur;
        [DataMember]
        [ObjectPropertyAttribute("occur", false, false, 0, false, false)]
        public SqlDateTime occur
        {
            get
            {
                return _occur;
            }
            set
            {
                if (!this.changedFields.Contains("_occur") && this._occur != value)
                    this.changedFields.Add("_occur", this._occur);
                _occur = value;
            }
        }
        #endregion
        #region changer
        protected SqlString _changer;
        [DataMember]
        [ObjectPropertyAttribute("changer", false, false, 128, false, false)]
        public SqlString changer
        {
            get
            {
                return _changer;
            }
            set
            {
                if (!this.changedFields.Contains("_changer") && this._changer != value)
                    this.changedFields.Add("_changer", this._changer);
                _changer = value;
            }
        }
        #endregion
        #region owner_account
        protected SqlInt32 _owner_account;
        [DataMember]
        [ObjectPropertyAttribute("account", true, false, 0, false, false)]
        public SqlInt32 owner_account
        {
            get
            {
                return _owner_account;
            }
            set
            {
                if (!this.changedFields.Contains("_owner_account") && this._owner_account != value)
                    this.changedFields.Add("_owner_account", this._owner_account);
                _owner_account = value;
            }
        }
        #endregion
        #region stereotype
        protected SqlInt32 _stereotype;
        [DataMember]
        [ObjectPropertyAttribute("stereotype", true, false, 0, false, false)]
        public SqlInt32 stereotype
        {
            get
            {
                return _stereotype;
            }
            set
            {
                if (!this.changedFields.Contains("_stereotype") && this._stereotype != value)
                    this.changedFields.Add("_stereotype", this._stereotype);
                _stereotype = value;
            }
        }
        #endregion
        #region permissions
        protected SqlInt32 _permissions;
        [DataMember]
        [ObjectPropertyAttribute("permissions", false, false, 0, false, false)]
        public SqlInt32 permissions
        {
            get
            {
                return _permissions;
            }
            set
            {
                if (!this.changedFields.Contains("_permissions") && this._permissions != value)
                    this.changedFields.Add("_permissions", this._permissions);
                _permissions = value;
            }
        }
        #endregion
        #region rank
        protected SqlInt32 _rank;
        [DataMember]
        [ObjectPropertyAttribute("rank", false, false, 0, false, false)]
        public SqlInt32 rank
        {
            get
            {
                return _rank;
            }
            set
            {
                if (!this.changedFields.Contains("_rank") && this._rank != value)
                    this.changedFields.Add("_rank", this._rank);
                _rank = value;
            }
        }
        #endregion
        #region name
        protected SqlString _name;
        [DataMember]
        [ObjectPropertyAttribute("name", false, false, 128, false, false)]
        public SqlString name
        {
            get
            {
                return _name;
            }
            set
            {
                if (!this.changedFields.Contains("_name") && this._name != value)
                    this.changedFields.Add("_name", this._name);
                _name = value;
            }
        }
        #endregion
        #region active
        protected SqlBoolean _active;
        [DataMember]
        [ObjectPropertyAttribute("active", false, false, 0, false, false)]
        public SqlBoolean active
        {
            get
            {
                return _active;
            }
            set
            {
                if (!this.changedFields.Contains("_active") && this._active != value)
                    this.changedFields.Add("_active", this._active);
                _active = value;
            }
        }
        #endregion
        #region guid
        protected SqlGuid _guid;
        [DataMember]
        [ObjectPropertyAttribute("guid", false, false, 0, false, false)]
        public SqlGuid guid
        {
            get
            {
                return _guid;
            }
            set
            {
                if (!this.changedFields.Contains("_guid") && this._guid != value)
                    this.changedFields.Add("_guid", this._guid);
                _guid = value;
            }
        }
        #endregion
        #region author
        protected SqlString _author;
        [DataMember]
        [ObjectPropertyAttribute("author", false, false, 255, false, false)]
        public SqlString author
        {
            get
            {
                return _author;
            }
            set
            {
                if (!this.changedFields.Contains("_author") && this._author != value)
                    this.changedFields.Add("_author", this._author);
                _author = value;
            }
        }
        #endregion
        #region Константы
        #endregion
        #region RefObjects
        #region parentObject
        [ObjectPropertyAttribute("ENTITY", false, false)]
        [ClassKeyName("ENTITY", "entity")]
        public Dersa.Common.Entity parentObject
        {
            get
            {
                if (this.parent.IsNull) return null;
                return (Dersa.Common.Entity)GetObject("ENTITY", this.parent);
            }
            set
            {
                if (value != null)
                    parent = value.entity;
                else
                    this.parent = System.DBNull.Value;
            }
        }
        #endregion
        #endregion
        #region Методы

        #region GetFactory
        public static ObjectFactory GetFactory()
        {
            DiosSqlManager M = new DiosSqlManager();
            ObjectFactory F = M.GetFactory(EntityClassName);
            M.IsOccupied = false;
            return F;
        }
        #endregion
        #endregion
        #region GetUniView()
        protected override UniStructView GetUniView()
        {
            IndexerPropertyDescriptorCollection props = this.GetObjectProperties();
            object[] dataStore = new object[props.Count];
            int i = 0;
            dataStore[i++] = entity;
            dataStore[i++] = parent;
            dataStore[i++] = occur;
            dataStore[i++] = changer;
            dataStore[i++] = owner_account;
            dataStore[i++] = stereotype;
            dataStore[i++] = permissions;
            dataStore[i++] = rank;
            dataStore[i++] = name;
            dataStore[i++] = active;
            dataStore[i++] = guid;
            dataStore[i++] = author;
            for (int k = i; k < props.Count; k++)
            {
                dataStore[k] = this[props[k].Name];
            }
            UniStructView result = new UniStructView(dataStore, props);
            return result;
        }
        #endregion
        #region Refs
        #endregion
        #region Properties
        #endregion
    }
}