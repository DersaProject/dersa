using System;
using System.Collections;
using System.Data.SqlClient;
using Dersa.Common;
using Dersa.Interfaces;
using System.Runtime.Serialization;

/// <summary>
/// Summary description for BaseClass.
namespace Dersa.Common
{
    /// </summary>
    public abstract class BaseClass : BaseObject, IBaseClass
    {
        public BaseClass()
        {
        }
        public BaseClass(ObjectClass objectClass) : base(objectClass)
        {
        }
        protected internal ChildrenCollection attributes = new ChildrenCollection();

        //private void ReloadAttributes()
        //{
        //    lock (this)
        //    {
        //        while (attributes.Count > 0)
        //        {
        //            ((Attribute)attributes[0]).Dispose();
        //        }
        //        string sql = "select * from ATTRIBUTE (nolock) where owner_ref = " + Id.ToString() + " and owner_class = '" + ObjectClass.ObjectName + "'";
        //        SqlCommand command = new SqlCommand(sql, Manager.Connection);
        //        try
        //        {
        //            bool isStereotype = this is Stereotype;
        //            SqlDataReader dr = command.ExecuteReader();
        //            while (dr.Read())
        //            {
        //                Attribute a = Manager.CreateObject("ATTRIBUTE") as Attribute;
        //                a.Restore(dr);
        //                //a.Owner = this;
        //            }
        //            dr.Close();
        //        }
        //        finally
        //        {
        //            command.Connection.Close();
        //            command.Dispose();
        //        }
        //    }
        //}

        public virtual ChildrenCollection Attributes
        {
            get
            {
                if (!attributes.AutoSorting)
                {
                    attributes.Sort();
                    attributes.AutoSorting = true;
                }
                return attributes;
            }
        }
        IChildrenCollection IBaseClass.Attributes
        {
            get
            {
                return (IChildrenCollection)Attributes;
            }
        }
        /*public new object this[string attributeName]
		{
			get
			{
				ChildrenCollection attrs = Attributes;
				for (int i = 0; i < attrs.Count; i++) 
				{
					Attribute a = (Attribute)attrs[i];
					if (a.Name.Equals(attributeName)) return a.GetValue();
				}
				throw new Exception("У объекта " + this.Name + " отсутствует свойство " + attributeName);
			}
		}
		object IBaseClass.this[string attributeName]
		{
			get
			{
				return this[attributeName];
			}
		}*/
        public Attribute GetAttribute(int ID)
        {
            ChildrenCollection attrs = Attributes;
            for (int i = 0; i < attrs.Count; i++)
            {
                Attribute a = (Attribute)attrs[i];
                if (a.Id.Equals(ID)) return a;
            }
            return null;
        }
        IAttribute IBaseClass.GetAttribute(int ID)
        {
            return GetAttribute(ID);
        }
        public Attribute GetAttribute(string attributeName)
        {
            ChildrenCollection attrs = Attributes;
            for (int i = 0; i < attrs.Count; i++)
            {
                Attribute a = (Attribute)attrs[i];
                if (a.Name.Equals(attributeName)) return a;
            }
            return null;
        }
        IAttribute IBaseClass.GetAttribute(string attributeName)
        {
            return GetAttribute(attributeName);
        }
        public string GetAttributeText(string attributeName)
        {
            Attribute a = GetAttribute(attributeName);
            if (a != null) return a.Code;
            return null;
        }
        public System.Object GetAttributeValue(string attributeName)
        {
            Attribute a = GetAttribute(attributeName);
            if (a != null) return a.Value;
            return null;
        }
        protected override void OnClone(Object o)
        {
            base.OnClone(o);
            BaseClass bc = (BaseClass)o;
            if (this.attributes.Count > 0)
            {
                for (int i = 0; i < this.attributes.Count; i++)
                {
                    Attribute a = (Attribute)((Attribute)this.attributes[i]).Clone();
                    a.Owner = bc;
                }
            }
        }
        protected override void AfterPost()
        {
            base.AfterPost();
            PostAttributes();
        }
        internal void PostAttributes()
        {
            ChildrenCollection attrs = this.attributes;
            for (int i = 0; i < attrs.Count; i++)
            {
                Attribute a = (Attribute)attrs[i];
                if (a.IsModified)
                {
                    a.Post();
                }
            }
        }
        //protected override void BeforeDrop()
        //{
        //    base.BeforeDrop();
        //    while (this.attributes.Count > 0)
        //    {
        //        this.attributes[0].Drop();
        //    }
        //}

        //protected override void BeforeCancel()
        //{
        //    CancelAttributes();
        //    base.BeforeCancel();
        //}
        //internal void CancelAttributes()
        //{
        //    ChildrenCollection attrs = this.attributes;
        //    for (int i = 0; i < attrs.Count; i++)
        //    {
        //        Attribute a = (Attribute)attrs[i];
        //        if (a.IsModified)
        //        {
        //            a.Cancel();
        //        }
        //    }
        //}
        public void Dispose()
        {
            if (attributes != null)
            {
                while (attributes.Count > 0)
                {
                    ((Attribute)attributes[0]).Dispose();
                }
                //attributes = null;
            }
            base.Dispose();
        }
    }
}