using System;
using System.ComponentModel;
using System.Reflection;
using System.Data.SqlClient;
using Dersa.Interfaces; 
using System.Runtime.Serialization;

/// <summary>
/// Summary description for Attribute.
namespace Dersa.Common
{
    public enum AttributeOwnerType : int { Entity = 0, Relation = 1}
    /// </summary>
    public class Attribute : BaseObject, IAttribute, ICodeObject
    {
        public Attribute()
        {
        }
        public Attribute(ObjectClass objectClass) : base(objectClass)
        {
        }
        private String _temp_value;
        protected String _type;
        protected String _value;
        protected Dersa.Interfaces.ValueType _valueType;
        protected BaseClass _owner;
        protected Attribute _parent;

        public static new SqlProperty[] InitializeProperties(string keyName)
        {
            SqlProperty[] tempProperty = BaseObject.InitializeProperties(keyName);
            SqlProperty[] sqlProperty = new SqlProperty[tempProperty.Length + 7];
            tempProperty.CopyTo(sqlProperty, 0);

            sqlProperty[sqlProperty.Length - 7] = new SqlProperty("AccessModifier", "access_modifier", System.Data.SqlDbType.VarChar, 100);
            SqlProperty property = new SqlProperty("OwnerRef", "owner_ref", System.Data.SqlDbType.Int, 0);
            property.FieldName = "_owner";
            sqlProperty[sqlProperty.Length - 6] = property;
            property = new SqlProperty("OwnerClass", "owner_class", System.Data.SqlDbType.VarChar, 40);
            property.FieldName = "_owner";
            sqlProperty[sqlProperty.Length - 5] = property;
            sqlProperty[sqlProperty.Length - 4] = new SqlProperty("ParentId", "parent", System.Data.SqlDbType.Int, 0);
            sqlProperty[sqlProperty.Length - 3] = new SqlProperty("AttributeValueType", "value_type", System.Data.SqlDbType.Int, 0);
            sqlProperty[sqlProperty.Length - 3].FieldName = "_valueType";

            sqlProperty[sqlProperty.Length - 2] = new SqlProperty("Type", "type", System.Data.SqlDbType.VarChar, 128);
            property = new SqlProperty("Value", "value", System.Data.SqlDbType.VarChar, 255);
            property.AlternativeSqlName = "text_value";
            property.AlternativeSize = 0;
            property.AlternativeSqlDataType = System.Data.SqlDbType.NText;
            property.ÑonditionPropertyName = "ValueType";
            property.ÑonditionNotValue = Dersa.Interfaces.ValueType.Value;
            sqlProperty[sqlProperty.Length - 1] = property;

            return sqlProperty;
        }
        public static new PropertyInfo[] InitializePropertyInfos()
        {
            Type thisType = typeof(Attribute);
            PropertyInfo[] tempProperty = BaseObject.InitializePropertyInfos();
            PropertyInfo[] newProperty = new PropertyInfo[tempProperty.Length + 9];
            tempProperty.CopyTo(newProperty, 0);
            newProperty[newProperty.Length - 9] = thisType.GetProperty("Parent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 8] = thisType.GetProperty("ParentId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 7] = thisType.GetProperty("ValueType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 6] = thisType.GetProperty("Type", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 5] = thisType.GetProperty("Owner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 4] = thisType.GetProperty("OwnerClass", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 3] = thisType.GetProperty("OwnerRef", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 2] = thisType.GetProperty("AccessModifier", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 1] = thisType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return newProperty;
        }
        protected override string Condition()
        {
            return ObjectClass.KeyName + " = @" + ObjectClass.KeyName + " and owner_ref = @owner_ref and owner_class = @owner_class";
        }
        public Attribute Parent
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (Attribute)GetBackUpProperty("Parent");
                }
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
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
                //Parent = Manager.GetObject("ATTRIBUTE", value) as Attribute;
            }
        }
        public Dersa.Interfaces.ValueType ValueType
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (Dersa.Interfaces.ValueType)GetBackUpProperty("ValueType");
                }
                if (_parent == null)
                    return _valueType;
                else
                    return _parent.ValueType;
            }
            set
            {
                if (_parent == null)
                    _valueType = value;
            }
        }
        protected int AttributeValueType
        {
            get
            {
                return (int)ValueType;
            }
            set
            {
                ValueType = (Dersa.Interfaces.ValueType)value;
            }
        }
        public string Type
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("Type");
                }
                if (_parent == null)
                {
                    if (_type == null) return "";
                    return _type;
                }
                else
                    return _parent.Type;
            }
            set
            {
                if (_parent == null)
                    _type = value;
            }
        }
        public BaseClass Owner
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (BaseClass)GetBackUpProperty("Owner");
                }
                return _owner;
            }
            set
            {
                if ((_owner != null) && (_owner.attributes.Contains(this))) _owner.attributes.Remove(this);
                _owner = value;
                if (_owner != null) _owner.attributes.Add(this);
            }
        }

        protected string OwnerClass
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("OwnerClass");
                }
                if (_owner != null) return _owner.ObjectClass.ObjectName;
                return "";
            }
            set
            {
                //if (_temp_value != null)
                //{
                //    int ownerRef = int.Parse(_temp_value);
                //    Owner = ObjectClass.ObjectManager.GetObject(value, ownerRef) as BaseClass;
                //    _temp_value = null;
                //}
                //else
                //{
                //    _temp_value = value;
                //}
            }
        }
        protected int OwnerRef
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (int)GetBackUpProperty("OwnerRef");
                }
                if (_owner != null) return _owner.Id;
                return 0;
            }
            set
            {
                //if (_temp_value != null)
                //{
                //    string ownerClass = _temp_value;
                //    Owner = ObjectClass.ObjectManager.GetObject(ownerClass, value) as BaseClass;
                //    _temp_value = null;
                //}
                //else
                //{
                //    _temp_value = value.ToString();
                //}
            }
        }
        public override string Name
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("Name");
                }
                if (_parent == null)
                {
                    if (_name == null) return "";
                    return _name;
                }
                else
                    return _parent.Name;
            }
            set
            {
                if (_parent == null)
                    _name = value;
            }
        }
        protected string _accessModifier;
        public string AccessModifier
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("AccessModifier");
                }
                if (_parent == null)
                {
                    if (_accessModifier == null) return "";
                    return _accessModifier;
                }
                else
                    return _parent.AccessModifier;
            }
            set
            {
                if (_parent == null)
                    _accessModifier = value;
            }
        }
        public string Value
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("Value");
                }
                if (_value == null) return "";
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public object GetValue()
        {
            return DIOS.Common.TypeUtil.Convert(Value, System.Type.GetType(Type));
        }
        [Browsable(false)]
        public string Code
        {
            get
            {
                if (ValueType != Dersa.Interfaces.ValueType.Value)
                {
                    return Value;
                }
                return null;
            }
            set
            {
                if (ValueType != Dersa.Interfaces.ValueType.Value)
                {
                    _value = value;
                }
            }
        }
        public bool EqualsByValues(object obj)
        {
            if (obj.GetType() == typeof(Attribute))
            {
                Attribute a = (Attribute)obj;
                if ((a._id == this._id) && (a._name == this._name) &&
                    (a.Owner == this.Owner) && (a._type == this._type) &&
                    (a._value == this._value)) return true;
            }
            return false;
        }
        protected override void LoacateAfterPost()
        {
            base.LoacateAfterPost();
            if (this.OwnerClass == "STEREOTYPE")
            {
                this.Owner.attributes.Sort();
            }
        }
        protected override void OnClone(Object o)
        {
            base.OnClone(o);
            Attribute a = (Attribute)o;
            a.Parent = this.Parent;
        }
        //protected override void OnNew()
        //{
        //    base.OnNew();
        //    _accessModifier = "public";
        //}
        protected override void BeforeDrop()
        {
            base.BeforeDrop();
        }

        public void Dispose()
        {
            /*
			if (disposing)
			{
				if ((_owner != null)&&(_owner.attributes.Contains(this)))
				{
					_owner.attributes.Remove(this);
				}
				//if ((_parent != null)&&(_parent.childeren.Contains(this)))
				//{
				//	_parent.childeren.Remove(this);
				//}
				_owner = null;
				_type = null;
				_value = null;
			}*/
            base.Dispose();
        }
        IBaseClass ICodeObject.BaseClass
        {
            get
            {
                return Owner;
            }
        }
        string IAttribute.Type
        {
            get
            {
                return Type;
            }
        }
    }
}