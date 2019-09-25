using System;
using System.Collections;
using System.Data.SqlClient;
using System.Reflection;

/// <summary>
/// Summary description for ObjectType.
namespace Dersa.Common
{
    /// </summary>
    public class ObjectClass : IDictionary
    {
        public ObjectClass(string nativeName, string objectName, string keyName, Type objectType, bool writeLog)
        {
            this._nativeName = nativeName;
            //this._objectManager = objectManager;
            this._objectName = objectName;
            this._keyName = keyName;
            this._writeLog = writeLog;
            this._objectType = objectType;

            MethodInfo mi = this._objectType.GetMethod("InitializeProperties", BindingFlags.Static | BindingFlags.Public);
            this._sqlProperty = (SqlProperty[])mi.Invoke(null, new object[] { this._keyName });

            mi = this._objectType.GetMethod("InitializePropertyInfos", BindingFlags.Static | BindingFlags.Public);
            this._propertyInfos = (PropertyInfo[])mi.Invoke(null, new object[0]);

            this._hash = new Hashtable();
        }

        public static ObjectClass GetObjectClass(System.Data.DataTable t)
        {
            if (t == null || t.Rows.Count < 1)
                return null;
            return GetObjectClass(t.Rows[0]);
        }

        public static ObjectClass GetObjectClass(System.Data.DataRow r)
        {
            return new ObjectClass(r["name"].ToString(), r["name"].ToString(), "entity", typeof(Entity), false);
        }

        //private ObjectManager _objectManager;
        private bool _writeLog;
        private string _keyName;
        private string _objectName;
        private string _nativeName;
        private Type _objectType;
        private Hashtable _hash;
        private Hashtable _hashIndex;
        private SqlProperty[] _sqlProperty;
        private PropertyInfo[] _propertyInfos;
        private int _id = 0;

        //public ObjectManager ObjectManager
        //{
        //    get { return _objectManager; }
        //}
        public string NativeName
        {
            get { return _nativeName; }
        }
        public string ObjectName
        {
            get { return _objectName; }
        }
        public string KeyName
        {
            get { return _keyName; }
        }
        public Type ObjectType
        {
            get { return _objectType; }
        }
        public bool WriteLog
        {
            get { return _writeLog; }
        }
        public SqlProperty[] SqlProperty
        {
            get { return _sqlProperty; }
        }
        public PropertyInfo[] BackupProperties
        {
            get { return _propertyInfos; }
        }
        public void AddUnicIndex(string fieldName)
        {
            if (_hashIndex == null)
            {
                _hashIndex = new Hashtable();
            }
            if (_hashIndex.ContainsKey(fieldName)) throw new Exception("Уникальный индекс по полю " + fieldName + " уже существует");
            _hashIndex.Add(fieldName, new Hashtable());
        }
        public void AddObject(Object o)
        {
            lock (this.SyncRoot)
            {
                ((IDictionary)this).Add(o.Id, o);
                if (_hashIndex != null)
                {
                    IDictionaryEnumerator enumer = _hashIndex.GetEnumerator();
                    while (enumer.MoveNext()) // в зависимоти от кол-ва индексов
                    {
                        Hashtable hashIndex = (Hashtable)enumer.Value;
                        object indexKey = o[(string)enumer.Key];
                        if (indexKey is String) indexKey = ((String)indexKey).ToLower();
                        hashIndex.Add(indexKey, o);
                    }
                }
            }
        }
        public void CheckObject(Object o)
        {
            if (_hashIndex != null)
            {
                IDictionaryEnumerator enumer = _hashIndex.GetEnumerator();
                while (enumer.MoveNext()) // в зависимоти от кол-ва индексов
                {
                    Hashtable hashIndex = (Hashtable)enumer.Value;
                    object indexKey = o[(string)enumer.Key];
                    if (indexKey is String) indexKey = ((String)indexKey).ToLower();
                    Object duplicate = hashIndex[indexKey] as Object;
                    if ((duplicate != null) && (!object.ReferenceEquals(duplicate, o))) throw new Exception("Объект с уникальным индексом " + indexKey + " уже присутствует в hashtable");
                }
            }
        }
        public void RemoveObject(Object o)
        {
            lock (this.SyncRoot)
            {
                ((IDictionary)this).Remove(o.Id);
                if (_hashIndex != null)
                {
                    IDictionaryEnumerator enumer = _hashIndex.GetEnumerator();
                    while (enumer.MoveNext()) // в зависимоти от кол-ва индексов
                    {
                        Hashtable hashIndex = (Hashtable)enumer.Value;
                        object indexKey = o[(string)enumer.Key];
                        if (indexKey is String) indexKey = ((String)indexKey).ToLower();
                        hashIndex.Remove(indexKey);
                    }
                }
            }
        }
        public Object this[int key]
        {
            get
            {
                return ((IDictionary)this)[key] as Object;
            }
            set
            {
                object o = ((IDictionary)this)[key];
                if (!object.ReferenceEquals(o, value)) throw new Exception("Незя!");
                if (_hashIndex != null)
                {
                    IDictionaryEnumerator enumer = _hashIndex.GetEnumerator();
                    while (enumer.MoveNext()) // в зависимоти от кол-ва индексов
                    {
                        Hashtable hashIndex = (Hashtable)enumer.Value;
                        object oldKey = value.GetBackUpProperty((string)enumer.Key); if (oldKey is String) oldKey = ((String)oldKey).ToLower();
                        object newKey = value[(string)enumer.Key]; if (newKey is String) newKey = ((String)newKey).ToLower();
                        if (oldKey.Equals(newKey)) break;
                        hashIndex.Remove(oldKey);
                        hashIndex.Add(newKey, value);
                    }
                }
            }
        }
        public Object this[string indexField, object indexValue]
        {
            get
            {
                Hashtable hashIndex = _hashIndex[indexField] as Hashtable;
                if (hashIndex == null) return null;
                if (indexValue is String) indexValue = ((String)indexValue).ToLower();
                return hashIndex[indexValue] as Object;
            }
        }
        //public int GetId()
        //{
        //    lock (this)
        //    {
        //        if (_id != 0)
        //        {
        //            return ++_id;
        //        }
        //        else
        //        {
        //            SqlCommand command = new SqlCommand("select max(" + _keyName + ") from " + _objectName + " (nolock)", _objectManager.Connection);
        //            try
        //            {
        //                SqlDataReader dr = command.ExecuteReader();
        //                dr.Read();
        //                if (!dr[0].Equals(System.DBNull.Value))
        //                {
        //                    _id = (int)dr[0];
        //                }
        //                dr.Close();
        //                return ++_id;
        //            }
        //            finally
        //            {
        //                command.Connection.Close();
        //                command.Dispose();
        //            }
        //        }
        //    }
        //}

        #region IDictionary Members

        bool IDictionary.IsReadOnly
        {
            get { return _hash.IsReadOnly; }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _hash.GetEnumerator();
        }

        object IDictionary.this[object key]
        {
            get
            {
                return _hash[key];
            }
            set
            {
                _hash[key] = value;
            }
        }

        void IDictionary.Remove(object key)
        {
            lock (this.SyncRoot)
            {
                if (!_hash.ContainsKey(key)) throw new Exception("Класс " + _objectName + " не содержит объект с id = " + key);
                _hash.Remove(key);
            }
        }

        bool IDictionary.Contains(object key)
        {
            return _hash.Contains(key);
        }

        void IDictionary.Clear()
        {
            _hash.Clear();
        }

        ICollection IDictionary.Values
        {
            get
            {
                return _hash.Values;
            }
        }

        void IDictionary.Add(object key, object value)
        {
            if (_hash.ContainsKey(key)) throw new Exception("Класс " + _objectName + " уже содержит объект с id = " + key);
            _hash.Add(key, value);
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return _hash.Keys;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return _hash.IsFixedSize;
            }
        }

        #endregion

        #region ICollection Members

        bool ICollection.IsSynchronized
        {
            get
            {
                return _hash.IsSynchronized;
            }
        }

        public int Count
        {
            get
            {
                return _hash.Count;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            _hash.CopyTo(array, index);
        }

        public object SyncRoot
        {
            get
            {
                return _hash.SyncRoot;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _hash.GetEnumerator(); ;
        }

        #endregion
    }
}