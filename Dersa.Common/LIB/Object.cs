using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Reflection;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Security.Principal;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using Dersa.Interfaces;
//using Dersa.Common;

/// <summary>
/// Summary description for Object.
/// </summary>
namespace Dersa.Common
{
    public abstract class Object : IObject, IComparable, ICloneable//, IEditableObject
    {
        public Object()
        {
        }
        public Object(ObjectClass objectClass) : this()
        {
            _objectClass = objectClass;
        }
        private Guid _identity = Guid.NewGuid();
        private ObjectState _objectState = ObjectState.osPool;
        //private Backup[] _backupData; 
        //private Backup[] _backupProperties; 
        private ObjectClass _objectClass;

        protected Dersa.Interfaces.IUserContext _currentChanger = null;
        protected int _id = -1;
        protected string _changer;
        protected Account _owner_account;
        protected DateTime _occur;
        protected bool locateAfterPost = false;

        //protected ObjectManager Manager
        //{
        //    get { return _objectClass.ObjectManager; }
        //}
        protected internal ObjectClass ObjectClass
        {
            get { return _objectClass; }
        }
        [Browsable(false)]
        public ObjectState ObjectState
        {
            get
            {
                return _objectState;
            }
        }

        internal Dersa.Interfaces.IUserContext CurrentChanger
        {
            get
            {
                return _currentChanger;
            }
            set
            {
                //if (_currentChanger != value)
                //{
                //    if (_currentChanger != null)
                //    {
                //        Hashtable modified = (Hashtable)_currentChanger[ObjectManager.MOK];
                //        lock (modified.SyncRoot)
                //        {
                //            if (modified.ContainsKey(_identity))
                //            {
                //                modified.Remove(_identity);
                //            }
                //        }
                //    }
                //    _currentChanger = value;
                //    if (_currentChanger != null)
                //    {
                //        Hashtable modified = (Hashtable)_currentChanger[ObjectManager.MOK];
                //        lock (modified.SyncRoot)
                //        {
                //            if (!modified.ContainsKey(_identity))
                //            {
                //                modified.Add(_identity, this);
                //            }
                //        }
                //    }
                //}
            }
        }
        protected void Insert()
        {
            SqlConnection connection = null;// Manager.Connection;
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using (SqlCommand command = Command(SelectState.Insert))
                {
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();
                }
                if (_objectClass.WriteLog)
                {
                    using (SqlCommand logCommand = LogCommand())
                    {
                        logCommand.Connection = connection;
                        logCommand.Transaction = transaction;
                        SetLogParameters(logCommand, ObjectClass.SqlProperty, "A");
                        logCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
        protected void Update()
        {
            SqlConnection connection = null;// Manager.Connection;
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using (SqlCommand command = Command(SelectState.Update))
                {
                    if (command.CommandText != "")
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                    }
                }
                if (_objectClass.WriteLog)
                {
                    using (SqlCommand logCommand = LogCommand())
                    {
                        logCommand.Connection = connection;
                        logCommand.Transaction = transaction;
                        SetLogParameters(logCommand, ObjectClass.SqlProperty, "P");
                        logCommand.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
        protected virtual void Delete()
        {
            SqlConnection connection = null;//Manager.Connection;
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                Delete(connection, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
        protected void Delete(SqlConnection connection, SqlTransaction transaction)
        {
            using (SqlCommand command = Command(SelectState.Delete))
            {
                command.Connection = connection;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
            if (_objectClass.WriteLog)
            {
                using (SqlCommand logCommand = LogCommand())
                {
                    logCommand.Connection = connection;
                    logCommand.Transaction = transaction;
                    SetLogParameters(logCommand, ObjectClass.SqlProperty, "D");
                    logCommand.ExecuteNonQuery();
                }
            }
        }
        public static SqlProperty[] InitializeProperties(string keyName)
        {
            SqlProperty[] sqlProperty = new SqlProperty[5];
            sqlProperty[0] = new SqlProperty("Id", keyName, System.Data.SqlDbType.Int, 0);
            sqlProperty[1] = new SqlProperty("Occur", "occur", System.Data.SqlDbType.DateTime, 0);
            sqlProperty[2] = new SqlProperty("Changer", "changer", System.Data.SqlDbType.VarChar, 128);
            sqlProperty[3] = new SqlProperty("OwnerAccountId", "owner_account", System.Data.SqlDbType.Int, 0);
            sqlProperty[3].FieldName = "_owner_account";
            sqlProperty[4] = new SqlProperty("Identity", "guid", System.Data.SqlDbType.UniqueIdentifier, 0);
            return sqlProperty;
        }
        public static PropertyInfo[] InitializePropertyInfos()
        {
            Type thisType = typeof(Object);
            PropertyInfo[] newProperty = new PropertyInfo[4];
            newProperty[newProperty.Length - 4] = thisType.GetProperty("OwnerAccount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 3] = thisType.GetProperty("OwnerAccountId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 2] = thisType.GetProperty("Changer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 1] = thisType.GetProperty("Occur", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return newProperty;
        }

        #region LOG
        private SqlCommand LogCommand()
        {
            SqlCommand command = new SqlCommand();
            string fields = "state";
            string values = "@state";
            string coma = ",";
            SqlProperty[] sqlProperty = ObjectClass.SqlProperty;
            for (int i = 0; i < sqlProperty.Length; i++)
            {
                fields = fields + coma + sqlProperty[i].GetSqlName(this);
                values = values + coma + "@" + sqlProperty[i].GetSqlName(this);
            }
            command.CommandText = "INSERT INTO " + _objectClass.ObjectName + "_LOG(" + fields + ") Values (" + values + ")";
            InitializeLogParameters(command, sqlProperty);
            return command;
        }
        private void InitializeLogParameters(SqlCommand command, SqlProperty[] sqlProperty)
        {
            if (command == null) return;
            InitializeParameters(command, sqlProperty);
            if (command.CommandText.IndexOf("@state") > 0)
            {
                command.Parameters.Add("@state", System.Data.SqlDbType.Char, 1);
            }
        }
        private void SetLogParameters(SqlCommand command, SqlProperty[] sqlProperty, string state)
        {
            SetParameters(command, sqlProperty);
            if (command.Parameters.Contains("@state"))
            {
                command.Parameters["@state"].Value = state;
            }
        }
        #endregion

        protected virtual SqlCommand Command(SelectState type)
        {
            SqlCommand command = null;
            switch (type)
            {
                case SelectState.Select:
                    {
                        command = SelectCommand();
                        break;
                    }
                case SelectState.Insert:
                    {
                        command = InsertCommand();
                        break;
                    }
                case SelectState.Update:
                    {
                        command = UpdateCommand();
                        break;
                    }
                case SelectState.Delete:
                    {
                        command = DeleteCommand();
                        break;
                    }
            }
            SetParameters(command, ObjectClass.SqlProperty);
            return command;
        }
        protected virtual SqlCommand SelectCommand()
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * from " + _objectClass.ObjectName + " where " + Condition();
            InitializeParameters(command, ObjectClass.SqlProperty);
            return command;
        }
        protected virtual SqlCommand InsertCommand()
        {
            SqlCommand command = new SqlCommand();
            string fields = "";
            string values = "";
            string coma = "";
            SqlProperty[] sqlProperty = ObjectClass.SqlProperty;
            for (int i = 0; i < sqlProperty.Length; i++)
            {
                fields = fields + coma + sqlProperty[i].GetSqlName(this);
                values = values + coma + "@" + sqlProperty[i].GetSqlName(this);
                if (coma.Equals("")) coma = ",";
            }
            command.CommandText = "INSERT INTO " + _objectClass.ObjectName + "(" + fields + ") Values (" + values + ")";
            InitializeParameters(command, sqlProperty);
            return command;
        }

        protected virtual SqlCommand UpdateCommand()
        {
            SqlCommand command = new SqlCommand();
            string updateStr = GenerateUpdateString();
            if (updateStr != "")
            {
                command.CommandText = "UPDATE " + _objectClass.ObjectName + " set " + updateStr + " where " + Condition();
            }
            InitializeParameters(command, ObjectClass.SqlProperty);
            return command;
        }
        protected virtual string GenerateUpdateString()
        {
            string updateStr = "";
            /*
			string coma = "";
			if (_backupData == null) throw new Exception("backupData ПУСТАЯ");
			foreach (SqlProperty sp in ObjectClass.SqlProperty)
			{
				PropertyInfo pInfo = this.GetType().GetProperty(sp.PropertyName, BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance);
				if (pInfo != null)
				{
					foreach (Backup bp in _backupData)
					{
						if (sp.PropertyName == bp.Name)
						{
							object value = pInfo.GetValue(this, null);
							if (((value != null)&&(!value.Equals(bp.Value)))||((value == null)&&(bp.Value != null)))
							{
								string sqlName = sp.GetSqlName(this);
								updateStr = updateStr + coma + sqlName + " = @" + sqlName;
								if (coma.Equals("")) coma = ",";	
								break;
							}
						}
					}
				}*/
            /*
             * FieldInfo fInfo = this.GetType().GetField(sp.FieldName, BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance);
            if (fInfo != null)
            {
                foreach (Backup bp in backupData)
                {
                    if (sp.FieldName == bp.Name)
                    {
                        object value = fInfo.GetValue(this);
                        if (((value != null)&&(!value.Equals(bp.Value)))||((value == null)&&(bp.Value != null)))
                        {
                            string sqlName = sp.GetSqlName(this);
                            updateStr = updateStr + coma + sqlName + " = @" + sqlName;
                            if (coma.Equals("")) coma = ",";	
                            break;
                        }
                    }
                }
            }*/
            //}
            return updateStr;
        }
        protected virtual SqlCommand DeleteCommand()
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "DELETE from " + _objectClass.ObjectName + " where " + Condition();
            InitializeParameters(command, ObjectClass.SqlProperty);
            return command;
        }
        protected virtual string Condition()
        {
            return _objectClass.KeyName + " = @" + _objectClass.KeyName;
        }
        protected virtual void InitializeParameters(SqlCommand command, SqlProperty[] sqlProperty)
        {
            if (command == null) return;
            command.Parameters.Clear();
            foreach (SqlProperty sp in sqlProperty)
            {
                string sqlName = sp.GetSqlName(this);
                if (command.CommandText.IndexOf("@" + sqlName) > 0)
                {
                    command.Parameters.Add("@" + sqlName, sp.GetSqlType(this), sp.GetSqlSize(this));
                }
            }
        }
        protected virtual void SetParameters(SqlCommand command, SqlProperty[] sqlProperty)
        {
            foreach (SqlProperty sp in sqlProperty)
            {
                string sqlName = sp.GetSqlName(this);
                if (command.Parameters.Contains("@" + sqlName))
                {
                    PropertyInfo pInfo = this.GetType().GetProperty(sp.PropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if ((pInfo != null) && (pInfo.CanRead))
                    {
                        if (pInfo.GetValue(this, null) == null)
                            command.Parameters["@" + sqlName].Value = System.DBNull.Value;
                        else if ((pInfo.PropertyType == typeof(string)) && (pInfo.GetValue(this, null).Equals("")))
                            command.Parameters["@" + sqlName].Value = System.DBNull.Value;
                        else if ((pInfo.PropertyType == typeof(int)) && (pInfo.GetValue(this, null).Equals(0)))
                            command.Parameters["@" + sqlName].Value = System.DBNull.Value;
                        else
                            command.Parameters["@" + sqlName].Value = pInfo.GetValue(this, null);
                    }
                    else
                        command.Parameters["@" + sqlName].Value = System.DBNull.Value;
                }
            }
        }
        public virtual void Restore()
        {
            //SqlConnection connection = Manager.Connection;
            //try
            //{
            //    using (SqlCommand command = Command(SelectState.Select))
            //    {
            //        command.Connection = connection;
            //        SqlDataReader dr = command.ExecuteReader();
            //        if (dr.Read())
            //        {
            //            Restore(dr);
            //        }
            //        dr.Close();
            //    }
            //}
            //finally
            //{
            //    connection.Close();
            //}
        }
        public object Clone()
        {
            //ThrowIfDisposed();
            //Object o = Manager.CreateObject(_objectClass.ObjectName);
            //o.RestoreFields(this);
            //o.New();
            //this.OnClone(o);
            //return o;
            return null;
        }
        protected virtual void OnClone(Object o)
        {
        }
        [Browsable(false)]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value > 0) _id = value;
            }
        }
        [Browsable(false)]
        protected Guid Identity
        {
            get
            {
                return _identity;
            }
            set
            {
                if (value != Guid.Empty) _identity = value;
            }
        }
        [Browsable(false)]
        public string NativeName
        {
            get
            {
                return _objectClass.NativeName;
            }
        }
        [Browsable(false)]
        public Account OwnerAccount
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return GetBackUpProperty("OwnerAccount") as Account;
                }
                return _owner_account;
            }
            set
            {
                _owner_account = value;
            }
        }
        [Browsable(false)]
        protected int OwnerAccountId
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (int)GetBackUpProperty("OwnerAccountId");
                }
                Account ownerAccount = OwnerAccount;
                if (ownerAccount != null) return ownerAccount.Id;
                return 0;
            }
            set
            {
                //OwnerAccount = Manager.GetObject("ACCOUNT", value) as Account;
            }
        }
        protected string Changer
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("Changer");
                }
                return _changer;
            }
            set
            {
                _changer = value;
            }
        }
        string IObject.Changer
        {
            get
            {
                return this.Changer;
            }
        }
        protected DateTime Occur
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (DateTime)GetBackUpProperty("Occur");
                }
                return _occur;
            }
            set
            {
                _occur = value;
            }
        }
        DateTime IObject.Occur
        {
            get
            {
                return this.Occur;
            }
        }
        protected bool ReturnBackupProperty()
        {
            return false;
            //return ((IsModified) && (CurrentChanger != Manager.CurrentUser));
        }
        [Browsable(false)]
        public object this[string propertyName]
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return GetBackUpProperty(propertyName);
                }
                else
                {
                    PropertyInfo pInfo = this.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if ((pInfo != null) && (pInfo.CanRead))
                    {
                        return pInfo.GetValue(this, null);
                    }
                }
                return null;
            }
            set
            {
                if (!IsModified) throw new Exception("Нельзя проcтавлять свойство у объекта в состоянии Pooled");
                PropertyInfo pInfo = this.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if ((pInfo != null) && (pInfo.CanWrite))
                {
                    object correctValue = DIOS.Common.TypeUtil.Convert(value, pInfo.PropertyType);
                    pInfo.SetValue(this, correctValue, null);
                }
            }
        }

        internal object GetBackUpProperty(string propertyName)
        {/*
			if (_backupProperties == null) return null;
			foreach (Backup b in _backupProperties)
			{
				if (b.Name == propertyName) return b.Value;
			}*/
            return null;
        }
        public void SetProperty(string propertyName, object propertyValue)
        {
            lock (this)
            {
                this.Load();
                try
                {
                    this[propertyName] = propertyValue;
                    this.Post();
                }
                catch (Exception ex)
                {
                    Cancel();
                    throw ex;
                }
            }
        }
        [Browsable(false)]
        public bool IsModified
        {
            get
            {
                return CurrentChanger != null;
            }
        }
        protected virtual void CheckForPermissions(bool onNew)
        {
            //Account currentAccount = Manager.CurrentAccount;
            //if (!currentAccount.Is(AccountType.atWrite | AccountType.atWriteOwned)) throw new Exception("У вас нет прав на модификацию объектов.\n Ваши права: " + currentAccount.Type.ToString());
            //if ((!onNew) && (currentAccount.Is(AccountType.atWriteOwned)) && (!currentAccount.Is(AccountType.atWrite)))
            //{
            //    Account ownerAccount = OwnerAccount;
            //    if (currentAccount != ownerAccount)
            //    {
            //        throw new Exception("У вас нет прав на модификацию чужих объектов.\n Ваши права: " + currentAccount.Type.ToString());
            //    }
            //}
        }
        //public void New()
        //{
        //    lock (this)
        //    {
        //        ThrowIfDisposed();
        //        CheckForPermissions(true);
        //        BeginEdit();
        //        _objectState = ObjectState.osNew;
        //        _id = ObjectClass.GetId();
        //        _owner_account = Manager.CurrentAccount;
        //        OnNew();
        //    }
        //}
        //protected virtual void OnNew()
        //{
        //}
        public void Load()
        {
            lock (this)
            {
                ThrowIfDisposed();
                CheckForPermissions(false);
                BeginEdit();
                _objectState = ObjectState.osLoad;
                locateAfterPost = false;
                OnLoad();
            }
        }
        protected virtual void OnLoad()
        {
        }
        public void Post()
        {
            lock (this)
            {
                ThrowIfDisposed();
                BeforePost();
                _changer = CurrentChanger.UserName;
                _occur = DateTime.Now;
                if (_objectState == ObjectState.osNew)
                {
                    this.Insert();
                }
                else if (_objectState == ObjectState.osLoad)
                {
                    this.Update();
                }
                AfterPost();
                EndEdit();
                if (locateAfterPost)
                {
                    LoacateAfterPost();
                    locateAfterPost = false;
                }
                //Manager.NotifyUpdate(this);
            }
        }
        protected virtual void LoacateAfterPost()
        {
        }
        protected virtual void BeforePost()
        {
            ThrowIfNotModified();
            ObjectClass.CheckObject(this);
            //if (_currentChanger == null) throw new NullReferenceException("У объекта " + this.GetType().FullName + " id = " + _id + " СurrentChanger не определен");
        }
        protected void ThrowIfNotModified()
        {
            if (!IsModified) throw new Exception("Объект " + this.GetType().FullName + " id = " + _id + " не находится в режиме редактирования");
        }
        protected void ThrowIfDisposed()
        {
            if (_objectState == ObjectState.osDisposed) throw new ObjectDisposedException(this.GetType().FullName + " id = " + _id);
        }
        protected virtual void AfterPost()
        {
            if (ObjectState == ObjectState.osNew)
            {
                ObjectClass.AddObject(this);
            }
            else if (ObjectState == ObjectState.osLoad)
            {
                ObjectClass[this.Id] = this;
            }
        }
        public void Cancel()
        {
            lock (this)
            {
                ThrowIfDisposed();
                bool newObject = ObjectState == ObjectState.osNew;
                BeforeCancel();
                AfterCancel();
                CancelEdit();
                if (newObject) this.Dispose();
            }
        }
        protected virtual void BeforeCancel()
        {
        }
        protected virtual void AfterCancel()
        {
        }
        public void Drop()
        {
            lock (this)
            {
                ThrowIfDisposed();
                CheckForPermissions(false);
                BeginEdit();
                _objectState = ObjectState.osDrop;
                _changer = CurrentChanger.UserName;
                _occur = DateTime.Now;
                BeforeDrop();
                try
                {
                    this.Delete();
                }
                catch (Exception ex)
                {
                    CancelEdit();
                    throw ex;
                }
                EndEdit();
                //Manager.NotifyRemove(this);
                this.Dispose();
            }
        }
        protected virtual void BeforeDrop()
        {
        }
        protected void RestoreFields(Object original)
        {
            if (original != null)
            {
                FieldInfo[] fInfos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo fi in fInfos)
                {
                    if (fi.GetValue(original) == null)
                    {
                        fi.SetValue(this, null);
                    }
                    else if (fi.FieldType.IsArray)
                    {
                        Array a = Array.CreateInstance(fi.FieldType.GetElementType(), ((Array)fi.GetValue(original)).Length);
                        ((Array)fi.GetValue(original)).CopyTo(a, 0);
                        fi.SetValue(this, a);
                    }
                    else if ((fi.FieldType.IsValueType) || (fi.FieldType == typeof(string)))
                    {
                        fi.SetValue(this, fi.GetValue(original));
                    }
                }
            }
        }
        public virtual void Restore(SqlDataReader dr)
        {
            foreach (SqlProperty sp in ObjectClass.SqlProperty)
            {
                PropertyInfo pInfo = this.GetType().GetProperty(sp.PropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if ((pInfo != null) && (pInfo.CanWrite))
                {
                    string sqlName = sp.GetSqlName(this);
                    if (dr[sqlName].GetType() == typeof(System.DBNull))
                        pInfo.SetValue(this, null, null);
                    else
                        pInfo.SetValue(this, dr[sqlName], null);
                }
            }
            ObjectClass.AddObject(this);
        }
        public virtual int CompareTo(Object y)
        {
            return 0;
        }
        public override String ToString()
        {
            return "";
        }
        public virtual string ToDisplayString()
        {
            return ToString();
        }
        public void Dispose()
        {
            if (this._objectState == ObjectState.osDisposed) return;
            if (_objectClass != null)
            {
                //Manager.GCCollect();
                if (((IDictionary)_objectClass).Contains(this.Id))
                {
                    _objectClass.RemoveObject(this);
                }
                _objectClass = null;
            }
            this._objectState = ObjectState.osDisposed;
        }
        #region Implements IEditableObject
        protected void BeginEdit()
        {/*
			if ((IsModified)&&(CurrentChanger != Manager.CurrentUser))
			{
				throw new Exception("Нельзя изменять оъект " + this.GetType().FullName + " id = " + this.Id + "\n\n" +
					"Объект изменяется пользователем " + CurrentChanger.UserName);
			}
			CurrentChanger = Manager.CurrentUser;
			if (this._backupData == null) 
			{
				PropertyInfo[] bpInfos = this.GetType().GetProperties(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance);
				System.Collections.ArrayList list = new System.Collections.ArrayList();
				for (int i = 0; i < bpInfos.Length; i++)
				{
					PropertyInfo pi = bpInfos[i];
					if ((pi.CanWrite)&&(pi.GetIndexParameters().Length == 0))
					{
						Backup b = new Backup(pi.Name, pi.GetValue(this, null));
						list.Add(b);
					}
				}
				this._backupData = new Backup[list.Count];
				list.CopyTo(this._backupData, 0);

				PropertyInfo[] pInfos = _objectClass.BackupProperties;
				list = new System.Collections.ArrayList();
				for (int i = 0; i < pInfos.Length; i++)
				{
					PropertyInfo pi = pInfos[i];
					if ((pi.CanRead)&&(pi.GetIndexParameters().Length == 0))
					{
						if ((pi.PropertyType == typeof(ChildrenCollection))||(pi.PropertyType == typeof(EntityChildrenCollection))) continue;
						Backup b = new Backup(pi.Name, pi.GetValue(this, null));
						list.Add(b);
					}
				}
				this._backupProperties = new Backup[list.Count];
				list.CopyTo(this._backupProperties, 0);
			}*/
        }
        protected void CancelEdit()
        {/*
			if (!IsModified) 
			{
				throw new Exception("Объект ни кем не изменяется");
			}
			if (this._backupData != null) 
			{
				foreach (Backup bp in _backupData) 
				{
					PropertyInfo pInfo = this.GetType().GetProperty(bp.Name, BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance);
					object value = pInfo.GetValue(this, null);
					if (value == null)
					{
						if (bp.Value == null) continue;
						pInfo.SetValue(this, bp.Value, null);
					}
					else
					{
						if (pInfo.GetValue(this, null).Equals(bp.Value)) continue;
						pInfo.SetValue(this, bp.Value, null);
					}
				}
			}
			this._backupData = null;
			this._backupProperties = null;
			this._objectState = ObjectState.osPool;
			CurrentChanger = null;*/
        }
        protected void EndEdit()
        {/*
			if (!IsModified) 
			{
				throw new Exception("Объект ни кем не изменяется");
			}
			this._backupData = null;
			this._backupProperties = null;
			this._objectState = ObjectState.osPool;
			CurrentChanger = null;*/
        }
        #endregion

        #region IComparable Members

        int System.IComparable.CompareTo(object obj)
        {
            return CompareTo((Object)obj);
        }

        #endregion
    }
}