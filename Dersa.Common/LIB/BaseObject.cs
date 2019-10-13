using System;
using System.Reflection;
using System.Threading;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Security.Principal;
using Dersa.Interfaces;

/// <summary>
/// Summary description for BaseObject.
namespace Dersa.Common
{
    /// </summary>
    public abstract class BaseObject// : Object//, IBaseObject
    {
        public BaseObject()
        {
        }
        //public BaseObject(ObjectClass objectClass) : base(objectClass)
        //{
        //    if (objectClass == null)
        //        throw new Exception("objectClass is null");
        //    _name = objectClass.ObjectName;
        //}
        protected string _name;

        //public static new SqlProperty[] InitializeProperties(string keyName)
        //{
        //    SqlProperty[] tempPropery = Object.InitializeProperties(keyName);
        //    SqlProperty[] sqlProperty = new SqlProperty[tempPropery.Length + 1];
        //    tempPropery.CopyTo(sqlProperty, 0);
        //    sqlProperty[sqlProperty.Length - 1] = new SqlProperty("Name", "name", System.Data.SqlDbType.VarChar, 128);
        //    return sqlProperty;
        //}
        //public static new PropertyInfo[] InitializePropertyInfos()
        //{
        //    Type thisType = typeof(BaseObject);
        //    PropertyInfo[] tempProperty = Object.InitializePropertyInfos();
        //    PropertyInfo[] newProperty = new PropertyInfo[tempProperty.Length + 1];
        //    tempProperty.CopyTo(newProperty, 0);
        //    newProperty[newProperty.Length - 1] = thisType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //    return newProperty;
        //}
        public virtual string Name
        {
            get
            {
                //if (ReturnBackupProperty())
                //{
                //    return (string)GetBackUpProperty("Name");
                //}
                if (_name == null) return "";
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    //locateAfterPost = true;
                }
            }
        }
        //public override int CompareTo(Object y)
        //{
        //    if (!(y is BaseObject)) return 0;
        //    int result = base.CompareTo(y);
        //    if (result != 0) return result;
        //    return this.Name.CompareTo(((BaseObject)y).Name);
        //}
        public override String ToString()
        {
            return Name;
        }
    }
}