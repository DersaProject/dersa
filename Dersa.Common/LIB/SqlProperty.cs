using System;
using System.Reflection;

namespace Dersa.Common
{
    public class SqlProperty
    {
        public SqlProperty()
        {
        }
        public SqlProperty(string propertyName, string sqlName, System.Data.SqlDbType sqlType, int size)
        {
            _propertyName = propertyName;
            _fieldName = "_" + Char.ToLower(propertyName[0]) + propertyName.Substring(1);
            _sqlName = sqlName;
            _sqlDataType = sqlType;
            _size = size;
        }
        private string _propertyName;
        private string _fieldName;
        private string _sqlName;
        private int _size;
        private System.Data.SqlDbType _sqlDataType;
        private string _alternativeSqlName;
        private int _alternativeSize;
        private System.Data.SqlDbType _alternativeSqlDataType;
        private string _conditionPropertyName;
        private object _ñonditionNotValue;

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }
        public string FieldName
        {
            get
            {
                return _fieldName;
            }
            set
            {
                _fieldName = value;
            }
        }
        public string SqlName
        {
            get
            {
                return _sqlName;
            }
            set
            {
                _sqlName = value;
            }
        }
        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }
        public System.Data.SqlDbType SqlDataType
        {
            get
            {
                return _sqlDataType;
            }
            set
            {
                _sqlDataType = value;
            }
        }
        public string AlternativeSqlName
        {
            get
            {
                return _alternativeSqlName;
            }
            set
            {
                _alternativeSqlName = value;
            }
        }
        public int AlternativeSize
        {
            get
            {
                return _alternativeSize;
            }
            set
            {
                _alternativeSize = value;
            }
        }
        public System.Data.SqlDbType AlternativeSqlDataType
        {
            get
            {
                return _alternativeSqlDataType;
            }
            set
            {
                _alternativeSqlDataType = value;
            }
        }
        public string ÑonditionPropertyName
        {
            get
            {
                return _conditionPropertyName;
            }
            set
            {
                _conditionPropertyName = value;
            }
        }
        public object ÑonditionNotValue
        {
            get
            {
                return _ñonditionNotValue;
            }
            set
            {
                _ñonditionNotValue = value;
            }
        }
        public string GetSqlName(Object o)
        {
            if (_conditionPropertyName == null)
                return _sqlName;
            PropertyInfo pInfo = o.GetType().GetProperty(_conditionPropertyName);
            object value = pInfo.GetValue(o, null);
            if (!_ñonditionNotValue.Equals(value))
                return _alternativeSqlName;
            return _sqlName;
        }
        public System.Data.SqlDbType GetSqlType(Object o)
        {
            if (_conditionPropertyName == null)
                return _sqlDataType;
            PropertyInfo pInfo = o.GetType().GetProperty(_conditionPropertyName);
            object value = pInfo.GetValue(o, null);
            if (!_ñonditionNotValue.Equals(value))
                return _alternativeSqlDataType;
            return _sqlDataType;
        }
        public int GetSqlSize(Object o)
        {
            if (_conditionPropertyName == null)
                return _size;
            PropertyInfo pInfo = o.GetType().GetProperty(_conditionPropertyName);
            object value = pInfo.GetValue(o, null);
            if (!_ñonditionNotValue.Equals(value))
                return _alternativeSize;
            return _size;
        }
    }
}