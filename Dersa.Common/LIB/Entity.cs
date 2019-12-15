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
        #region parent
        protected SqlInt32 _parent;
        [DataMember]
        [ObjectPropertyAttribute("parent", false, false, 0, false, false)]
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
        #region stereotype
        protected SqlInt32 _stereotype;
        [DataMember]
        [ObjectPropertyAttribute("stereotype", false, false, 0, false, false)]
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
        #region icon
        protected SqlString _icon;
        [DataMember]
        [ObjectPropertyAttribute("icon", false, false, 255, false, false)]
        public SqlString icon
        {
            get
            {
                return _icon;
            }
            set
            {
                if (!this.changedFields.Contains("_icon") && this._icon != value)
                    this.changedFields.Add("_icon", this._icon);
                _icon = value;
            }
        }
        #endregion
        #region Константы
        #endregion
        #region RefObjects
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
            dataStore[i++] = active;
            dataStore[i++] = name;
            dataStore[i++] = parent;
            dataStore[i++] = stereotype;
            dataStore[i++] = icon;
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