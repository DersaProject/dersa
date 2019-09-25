using System;
using System.Reflection;
using Dersa.Interfaces;

namespace Dersa.Common
{
    public class Account : BaseObject, IAccount
    {
        public Account()
        {
        }
        public Account(ObjectClass objectClass) : base(objectClass)
        {
        }
        private AccountType _type;

        public AccountType Type
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (AccountType)GetBackUpProperty("Type");
                }
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        public int AccountType
        {
            get
            {
                return (int)Type;
            }
            set
            {
                Type = (AccountType)value;
            }
        }
        protected override void CheckForPermissions(bool onNew)
        {
            //Account currentAccount = Manager.CurrentAccount;
            //if (!currentAccount.Is(Dersa.Interfaces.AccountType.atManage)) throw new Exception("У вас нет прав на модификацию учетной записи.\n Ваши права: " + currentAccount.Type.ToString());
        }
        public bool Is(AccountType accountType)
        {
            return (this.Type & accountType) != 0;
        }
        public static new SqlProperty[] InitializeProperties(string keyName)
        {
            SqlProperty[] tempProperty = BaseObject.InitializeProperties(keyName);
            SqlProperty[] sqlProperty = new SqlProperty[tempProperty.Length + 1];
            tempProperty.CopyTo(sqlProperty, 0);
            SqlProperty property = new SqlProperty("AccountType", "type", System.Data.SqlDbType.TinyInt, 0);
            property.FieldName = "_type";
            sqlProperty[sqlProperty.Length - 1] = property;
            return sqlProperty;
        }
        public static new PropertyInfo[] InitializePropertyInfos()
        {
            Type thisType = typeof(Account);
            PropertyInfo[] tempProperty = BaseObject.InitializePropertyInfos();
            PropertyInfo[] newProperty = new PropertyInfo[tempProperty.Length + 1];
            tempProperty.CopyTo(newProperty, 0);
            newProperty[newProperty.Length - 1] = thisType.GetProperty("Type", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return newProperty;
        }
    }
}