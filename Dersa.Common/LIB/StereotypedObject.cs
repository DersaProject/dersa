using System;
using System.Reflection;
using Dersa.Interfaces;
using Dersa.Common;
using System.Runtime.Serialization;
using DIOS.Common;
/// <summary>
/// Summary description for StereotypedObject.
/// </summary>
namespace Dersa.Common
{
    public abstract class StereotypedObject : BaseClass, IStereotypedObject
    {
        public StereotypedObject()
        {
        }
        public StereotypedObject(ObjectClass objectClass, DersaSqlManager sm) : base(objectClass)
        {
            _SM = sm;
        }

        private DersaSqlManager _SM;
        protected DersaSqlManager SM
        {
            get
            {
                return _SM;
            }
        }
        protected Stereotype _stereotype;
        public Stereotype Stereotype
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (Stereotype)GetBackUpProperty("Stereotype");
                }
                return _stereotype;
            }
            set
            {
                _stereotype = value;
            }
        }
        //protected int StereotypeId
        //{
        //    get
        //    {
        //        if (ReturnBackupProperty())
        //        {
        //            return (int)GetBackUpProperty("StereotypeId");
        //        }
        //        if (Stereotype == null) return 0;
        //        return Stereotype.Id;
        //    }
        //    set
        //    {
        //        Stereotype = Manager.GetObject("STEREOTYPE", value) as Stereotype;
        //    }
        //}
        public string StereotypeName
        {
            get
            {
                if (_stereotype == null) return "<нет стереотипа>";
                return _stereotype.Name;
            }
        }
        public static new SqlProperty[] InitializeProperties(string keyName)
        {
            SqlProperty[] tempProperty = BaseClass.InitializeProperties(keyName);
            SqlProperty[] sqlProperty = new SqlProperty[tempProperty.Length + 1];
            tempProperty.CopyTo(sqlProperty, 0);
            sqlProperty[sqlProperty.Length - 1] = new SqlProperty("StereotypeId", "stereotype", System.Data.SqlDbType.Int, 0);
            sqlProperty[sqlProperty.Length - 1].FieldName = "_stereotype";
            return sqlProperty;
        }
        public static new PropertyInfo[] InitializePropertyInfos()
        {
            Type thisType = typeof(StereotypedObject);
            PropertyInfo[] tempProperty = BaseClass.InitializePropertyInfos();
            PropertyInfo[] newProperty = new PropertyInfo[tempProperty.Length + 2];
            tempProperty.CopyTo(newProperty, 0);
            newProperty[newProperty.Length - 2] = thisType.GetProperty("Stereotype", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 1] = thisType.GetProperty("StereotypeId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return newProperty;
        }
        protected override void BeforeDrop()
        {
            while (attributes.Count > 0)
            {
                ((Attribute)attributes[0]).Drop();
            }
            base.BeforeDrop();
        }
        public override ChildrenCollection Attributes
        {
            get
            {
                ChildrenCollection stereotypeAttrs = new ChildrenCollection();
                stereotypeAttrs.AddRange(_stereotype.PublicAttributes);
                for (int i = 0; i < stereotypeAttrs.Count; i++)
                {
                    for (int j = 0; j < this.attributes.Count; j++)
                    {
                        Attribute a = (Attribute)this.attributes[j];
                        if (stereotypeAttrs[i].Id == a.ParentId)
                        {
                            stereotypeAttrs[i] = a;
                            break;
                        }
                    }
                }
                return stereotypeAttrs;
            }
        }
        //public Attribute GetAttributeForModify(string attributeName)
        //{
        //    ChildrenCollection attrs = this.Attributes;
        //    for (int i = 0; i < attrs.Count; i++)
        //    {
        //        Attribute a = (Attribute)attrs[i];
        //        if (a.Name == attributeName)
        //        {
        //            if (a.Owner == this)
        //            {
        //                a.Load();
        //                return a;
        //            }
        //            else
        //            {
        //                Attribute new_a = (Attribute)Manager.NewObject("ATTRIBUTE");
        //                new_a.Owner = this;
        //                new_a.Parent = a;
        //                return new_a;
        //            }
        //        }
        //    }
        //    return null;
        //}
        public Attribute GetAttributeForView(string attributeName)
        {
            ChildrenCollection attrs = this.Attributes;
            for (int i = 0; i < attrs.Count; i++)
            {
                Attribute a = (Attribute)attrs[i];
                if (a.Name == attributeName)
                {
                    if (a.Owner == this)
                    {
                        return a;
                    }
                    else
                    {
                        throw new Exception("Аттрибут " + attributeName + " не существует");
                    }
                }
            }
            return null;
        }
        //IAttribute IStereotypedObject.GetAttributeForModify(string attributeName)
        //{
        //    return (IAttribute)GetAttributeForModify(attributeName);
        //}
        //IAttribute IStereotypedObject.GetAttributeForView(string attributeName)
        //{
        //    return (IAttribute)GetAttributeForView(attributeName);
        //}
        //public ChildrenCollection Operations
        //{
        //    get
        //    {
        //        return _stereotype.ClientOperations;
        //    }
        //}
        //IChildrenCollection IStereotypedObject.Operations
        //{
        //    get
        //    {
        //        return this.Operations;
        //    }
        //}
        public ICompiled GetCompiledInstance()
        {
            return Dersa.Common.Util.CreateInstance(this, this.SM);
            //return null;
        }
        IStereotype IStereotypedObject.Stereotype
        {
            get
            {
                return (IStereotype)Stereotype;
            }
        }
        protected override void OnClone(Object o)
        {
            base.OnClone(o);
            StereotypedObject so = (StereotypedObject)o;
            so.Stereotype = this.Stereotype;
        }
        public void Dispose()
        {
            _stereotype = null;
            base.Dispose();
        }
        public override string ToDisplayString()
        {
            return ToDisplay(_stereotype.ParsedFormat);
        }
        public string ToDisplayDiagramString()
        {
            return ToDisplay(_stereotype.DiagramParsedFormat);
        }
        private string ToDisplay(Parser[] pf)
        {
            if (pf != null)
            {
                string list_string = "";
                for (int i = 0; i < pf.Length; i++)
                {
                    if (pf[i].Type == ParseType.Delimeter)
                        list_string += pf[i].Value;
                    else
                    {
                        if (pf[i].Value == "Name")
                            list_string += this.Name;
                        else if (pf[i].Value == "Id")
                            list_string += this.Id.ToString();
                        else
                        {
                            ChildrenCollection attrs = this.Attributes;
                            bool finded = false;
                            for (int j = 0; j < attrs.Count; j++)
                            {
                                Attribute a = (Attribute)attrs[j];
                                if (a.Name.Equals(pf[i].Value))
                                {
                                    list_string += a.Value;
                                    finded = true;
                                    break;
                                }
                            }
                            if (!finded)
                            {
                                list_string += pf[i].Value;
                            }
                        }
                    }
                }
                return list_string;
            }
            else
            {
                return this.ToString();
            }
        }
        public override int CompareTo(Object y)
        {
            if (!(y is StereotypedObject)) return 0;
            int result = this.Stereotype.Rank.CompareTo(((StereotypedObject)y).Stereotype.Rank);
            if (result != 0) return result;
            return base.CompareTo(y);
        }
    }
}