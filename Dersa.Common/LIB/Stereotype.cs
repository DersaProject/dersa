using System;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Data.SqlClient;
using Dersa.Interfaces;
using Dersa.Common;
using System.Runtime.Serialization;

namespace Dersa.Common
{
    public class Stereotype : BaseClass, IStereotype
    {
        public Stereotype()
        {
        }
        public Stereotype(ObjectClass objectClass) : base(objectClass)
        {
        }

        public Stereotype(System.Data.DataTable t)
            : this(ObjectClass.GetObjectClass(t))
        {
        }

        private const string delimiters = " \t\n\r\f\"\'\\+-*/.,;:[]{}?<>()~!%^&$";

        private Parser[] _parsedFormat;
        private Parser[] _diagramParsedFormat;
        private ChildrenCollection _children = new ChildrenCollection();

        protected ChildrenCollection _operations = new ChildrenCollection();
        protected byte[] _imageBytes;
        protected StereotypeType _stereotypeType;
        protected ArrowType _arrowTypeA;
        protected ArrowType _arrowTypeB;
        protected Stereotype _parent = null;
        protected string _viewFormat;
        protected string _diagramViewFormat;
        protected string _extends;
        protected int _rank;

        public ChildrenCollection Operations
        {
            get
            {
                if (!_operations.AutoSorting)
                {
                    _operations.Sort();
                    _operations.AutoSorting = true;
                }
                return _operations;
            }

        }
        public byte[] ImageBytes
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (byte[])GetBackUpProperty("ImageBytes");
                }
                return _imageBytes;
            }
            set
            {
                _imageBytes = value;
            }
        }

        #region StereotypeType
        public StereotypeType StereotypeType
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (StereotypeType)GetBackUpProperty("StereotypeType");
                }
                return _stereotypeType;
            }
            set { _stereotypeType = value; }
        }
        public int Type
        {
            get { return (int)StereotypeType; }
            set { StereotypeType = (StereotypeType)value; }
        }
        #endregion

        #region ArrowTypeA
        public ArrowType ArrowTypeA
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (ArrowType)GetBackUpProperty("ArrowTypeA");
                }
                return _arrowTypeA;
            }
            set { _arrowTypeA = value; }
        }
        public int ArrowA
        {
            get { return (int)ArrowTypeA; }
            set { ArrowTypeA = (ArrowType)value; }
        }
        #endregion

        #region ArrowTypeB
        public ArrowType ArrowTypeB
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (ArrowType)GetBackUpProperty("ArrowTypeB");
                }
                return _arrowTypeB;
            }
            set { _arrowTypeB = value; }
        }
        public int ArrowB
        {
            get { return (int)ArrowTypeB; }
            set { ArrowTypeB = (ArrowType)value; }
        }
        #endregion

        public Stereotype Parent
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (Stereotype)GetBackUpProperty("Parent");
                }
                //if ((_parent == null) || (_parent == Manager.RootStereotype)) return null;
                return _parent;
            }
            set
            {
                if ((_parent != null) && (_parent._children.Contains(this)))
                {
                    _parent._children.Remove(this);
                }
                //if (value == null)
                //    _parent = Manager.RootStereotype;
                //else
                    _parent = value;
                if (_parent != null)
                {
                    _parent._children.Add(this);
                }
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
            //set { Parent = Manager.GetObject("STEREOTYPE", value) as Stereotype; }
        }
        public ChildrenCollection Children
        {
            get
            {
                if (!_children.AutoSorting)
                {
                    _children.Sort();
                    _children.AutoSorting = true;
                }
                return _children;
            }
        }
        IChildrenCollection IStereotype.Children
        {
            get
            {
                return this.Children;
            }
        }
        public virtual ChildrenCollection PublicAttributes
        {
            get
            {
                ChildrenCollection stereotypeAttrs = new ChildrenCollection();
                ChildrenCollection baseAttrs = base.Attributes;
                for (int i = 0; i < baseAttrs.Count; i++)
                {
                    Attribute a = (Attribute)baseAttrs[i];
                    if (a.AccessModifier == "public")
                    {
                        stereotypeAttrs.Add(a);
                    }
                }
                return stereotypeAttrs;
            }
        }
        public string ViewFormat
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("ViewFormat");
                }
                if (_viewFormat == null) return "";
                return _viewFormat;
            }
            set
            {
                _viewFormat = value;
                _parsedFormat = GetParsedFormat(_viewFormat);
            }
        }
        public string DiagramViewFormat
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("DiagramViewFormat");
                }
                if (_diagramViewFormat == null) return "";
                return _diagramViewFormat;
            }
            set
            {
                _diagramViewFormat = value;
                _diagramParsedFormat = GetParsedFormat(_diagramViewFormat);
            }
        }
        public string Extends
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (string)GetBackUpProperty("Extends");
                }
                if (_extends == null) return "";
                return _extends;
            }
            set
            {
                _extends = value;
            }
        }
        public int Rank
        {
            get
            {
                if (ReturnBackupProperty())
                {
                    return (int)GetBackUpProperty("Rank");
                }
                return _rank;
            }
            set
            {
                _rank = value;
            }
        }
        public Parser[] ParsedFormat
        {
            get
            {
                if (_parsedFormat == null)
                {
                    _parsedFormat = GetParsedFormat(ViewFormat);
                }
                return _parsedFormat;
            }
        }
        public Parser[] DiagramParsedFormat
        {
            get
            {
                if (_diagramParsedFormat == null)
                {
                    _diagramParsedFormat = GetParsedFormat(DiagramViewFormat);
                }
                return _diagramParsedFormat;
            }
        }
        //public void Move(int parentID)
        //{
        //    lock (this)
        //    {
        //        Load();
        //        Stereotype parent = Manager.GetObject("STEREOTYPE", parentID) as Stereotype;
        //        Move(parent);
        //        Post();
        //    }
        //}
        //public IEntity CreateEntity(int parentId)
        //{
        //    Entity entity = (Entity)Manager.NewObject("ENTITY");
        //    entity.Name = "<< " + this.Name + " >>";
        //    entity.Stereotype = this;
        //    entity.ParentId = parentId;
        //    entity.Post();
        //    return entity;
        //}
        //public IRelation CreateRelation(int aID)
        //{
        //    Relation relation = (Relation)Manager.NewObject("RELATION");
        //    relation.Stereotype = this;
        //    relation.AId = aID;
        //    return relation;
        //}
        //public IAttribute CreateAttribute()
        //{
        //    Attribute attribute = (Attribute)Manager.NewObject("ATTRIBUTE");
        //    attribute.ValueType = Dersa.Interfaces.ValueType.Value;
        //    attribute.Owner = this;
        //    try
        //    {
        //        attribute.Post();
        //    }
        //    catch (Exception ex)
        //    {
        //        attribute.Owner = null;
        //        attribute.Cancel();
        //        throw ex;
        //    }
        //    return attribute;
        //}
        //public IOperation CreateOperation()
        //{
        //    Operation operation = (Operation)Manager.NewObject("OPERATION");
        //    operation.Stereotype = this;
        //    try
        //    {
        //        operation.Post();
        //    }
        //    catch (Exception ex)
        //    {
        //        operation.Stereotype = null;
        //        operation.Cancel();
        //        throw ex;
        //    }
        //    return operation;
        //}
        private void Move(Stereotype parent)
        {
            if ((parent.Equals(this)) || (parent.Equals(Parent))) return;
            //Manager.NotifyRemove(this);
            Parent = parent;
            /*if (Parent != null) 
			{
				for (int i = 0; i < Parent.Attributes.Count; i++)
				{
					Attribute a = GetAttribute(((Attribute)Parent.Attributes[i]).Name);
					if (a == null) 
					{
						a = (Attribute)((Attribute)Parent.Attributes[i]).Clone();
						a.Owner = this;
						a.Post();
					}
				}
			} */
        }
        public ChildrenCollection GetPath()
        {
            ChildrenCollection entities = new ChildrenCollection();
            return this.GetPath(entities);
        }
        IChildrenCollection IStereotype.GetPath()
        {
            return this.GetPath();
        }
        private ChildrenCollection GetPath(ChildrenCollection stereotypes)
        {
            stereotypes.Insert(0, this);
            if (this.Parent != null)
            {
                stereotypes = Parent.GetPath(stereotypes);
            }
            return stereotypes;
        }
        public static new SqlProperty[] InitializeProperties(string keyName)
        {
            SqlProperty[] tempProperty = BaseClass.InitializeProperties(keyName);
            SqlProperty[] sqlProperty = new SqlProperty[tempProperty.Length + 9];
            tempProperty.CopyTo(sqlProperty, 0);

            sqlProperty[sqlProperty.Length - 9] = new SqlProperty("Rank", "rank", System.Data.SqlDbType.Int, 0);
            sqlProperty[sqlProperty.Length - 8] = new SqlProperty("Extends", "extends", System.Data.SqlDbType.VarChar, 255);

            SqlProperty property = new SqlProperty("ArrowA", "arrow_type_a", System.Data.SqlDbType.TinyInt, 0);
            property.FieldName = "_arrowTypeA";
            sqlProperty[sqlProperty.Length - 7] = property;

            property = new SqlProperty("ArrowB", "arrow_type_b", System.Data.SqlDbType.TinyInt, 0);
            property.FieldName = "_arrowTypeB";
            sqlProperty[sqlProperty.Length - 6] = property;

            property = new SqlProperty("ParentId", "parent", System.Data.SqlDbType.Int, 0);
            property.FieldName = "_parent";
            sqlProperty[sqlProperty.Length - 5] = property;

            sqlProperty[sqlProperty.Length - 4] = new SqlProperty("ImageBytes", "icon", System.Data.SqlDbType.Image, 0);

            property = new SqlProperty("Type", "stereotype_type", System.Data.SqlDbType.TinyInt, 0);
            property.FieldName = "_stereotypeType";
            sqlProperty[sqlProperty.Length - 3] = property;

            sqlProperty[sqlProperty.Length - 2] = new SqlProperty("ViewFormat", "view_format", System.Data.SqlDbType.VarChar, 255);
            sqlProperty[sqlProperty.Length - 1] = new SqlProperty("DiagramViewFormat", "diagram_view_format", System.Data.SqlDbType.VarChar, 255);
            return sqlProperty;
        }
        public static new PropertyInfo[] InitializePropertyInfos()
        {
            Type thisType = typeof(Stereotype);
            PropertyInfo[] tempProperty = BaseClass.InitializePropertyInfos();
            PropertyInfo[] newProperty = new PropertyInfo[tempProperty.Length + 10];
            tempProperty.CopyTo(newProperty, 0);
            newProperty[newProperty.Length - 10] = thisType.GetProperty("ImageBytes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 9] = thisType.GetProperty("StereotypeType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 8] = thisType.GetProperty("ArrowTypeA", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 7] = thisType.GetProperty("ArrowTypeB", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 6] = thisType.GetProperty("Parent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 5] = thisType.GetProperty("ParentId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 4] = thisType.GetProperty("ViewFormat", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 3] = thisType.GetProperty("DiagramViewFormat", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 2] = thisType.GetProperty("Extends", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            newProperty[newProperty.Length - 1] = thisType.GetProperty("Rank", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return newProperty;
        }
        //private void ReloadOperations()
        //{
        //    while (attributes.Count > 0)
        //    {
        //        ((Operation)_operations[0]).Dispose();
        //    }
        //    string sql = "select * from OPERATION (nolock) where stereotype = " + Id.ToString();
        //    SqlCommand command = new SqlCommand(sql, Manager.Connection);
        //    try
        //    {
        //        SqlDataReader dr = command.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            Operation o = Manager.CreateObject("OPERATION") as Operation;
        //            o.Restore(dr);
        //        }
        //        dr.Close();
        //        _operations.Sort();
        //    }
        //    finally
        //    {
        //        command.Connection.Close();
        //        command.Dispose();
        //    }
        //}
        //protected override void CheckForPermissions(bool onNew)
        //{
        //    Account currentAccount = Manager.CurrentAccount;
        //    if (!currentAccount.Is(AccountType.atManage)) throw new Exception("У вас нет прав на модификацию стереотипов.\n Ваши права: " + currentAccount.Type.ToString());
        //}
        //protected override void AfterPost()
        //{
        //    base.AfterPost();
        //    ChildrenCollection attrs = Operations;
        //    for (int i = 0; i < attrs.Count; i++)
        //    {
        //        Operation o = (Operation)attrs[i];
        //        if (o.IsModified)
        //        {
        //            o.Post();
        //        }
        //    }
        //}
        //protected override void BeforeCancel()
        //{
        //    base.BeforeCancel();
        //    ChildrenCollection attrs = Operations;
        //    for (int i = 0; i < attrs.Count; i++)
        //    {
        //        Operation o = (Operation)attrs[i];
        //        if (o.IsModified)
        //        {
        //            o.Cancel();
        //        }
        //    }
        //}
        protected override void OnClone(Object o)
        {
            base.OnClone(o);
            /*Stereotype s = (Stereotype)o;
			s._operations = new ChildrenCollection();
			if (this.Operations.Count > 0) 
			{
				for (int i = 0; i < this._operations.Count; i++) 
				{
					Operation op = (Operation)((Operation)this._operations[i]).Clone();
					op.Stereotype = s;
				}
			}*/
        }

        //public Operation GetOperation(int ID)
        //{
        //    for (int i = 0; i < this.Operations.Count; i++)
        //    {
        //        Operation o = (Operation)Operations[i];
        //        if (o.Id.Equals(ID)) return o;
        //    }
        //    return null;
        //}
        //public Operation GetOperation(string theName)
        //{
        //    for (int i = 0; i < this.Operations.Count; i++)
        //    {
        //        Operation o = (Operation)Operations[i];
        //        if (o.Name == theName) return o;
        //    }
        //    if (Parent != null)
        //    {
        //        return Parent.GetOperation(theName);
        //    }
        //    return null;
        //}
        //public ChildrenCollection ClientOperations
        //{
        //    get
        //    {
        //        ChildrenCollection oc = null;
        //        if (Parent == null)
        //            oc = new ChildrenCollection();
        //        else
        //            oc = Parent.ClientOperations;

        //        ChildrenCollection ops = Operations;
        //        for (int i = 0; i < ops.Count; i++)
        //        {
        //            Operation op = (Operation)ops[i];
        //            if (op.InternalUse) continue;
        //            bool find = false;
        //            for (int j = 0; j < oc.Count; j++)
        //            {
        //                if (((Operation)oc[j]).Name == op.Name)
        //                {
        //                    oc[j] = op;
        //                    find = true;
        //                    break;
        //                }
        //            }
        //            if (!find) oc.Add(op);
        //        }
        //        return oc;
        //    }
        //}
        bool IStereotype.IsAbstract()
        {
            return StereotypeType == Dersa.Interfaces.StereotypeType.Abstract;
        }
        //IChildrenCollection IStereotype.Operations
        //{
        //    get
        //    {
        //        return Operations;
        //    }
        //}
        IStereotype IStereotype.Parent
        {
            get
            {
                return (IStereotype)Parent;
            }
        }
        //IOperation IStereotype.GetOperation(string theName)
        //{
        //    return (IOperation)GetOperation(theName);
        //}
        protected override void LoacateAfterPost()
        {
            base.LoacateAfterPost();
            this._parent._children.Sort();
        }
        //protected override void OnNew()
        //{
        //    base.OnNew();
        //    Parent = null;
        //}
        protected override void BeforeDrop()
        {
            if (_children.Count > 0) throw new Exception("Сначала удалите объекты нижнего уровня");
            if (attributes.Count > 0) throw new Exception("Сначала удалите аттрибуты");
            if (_operations.Count > 0) throw new Exception("Сначала удалите операции");
            base.BeforeDrop();
        }
        public void Dispose()
        {
            if ((_parent != null) && (_parent.Children.Contains(this)))
            {
                _parent.Children.Remove(this);
            }
            _parent = null;
            //if (_operations != null)
            //{
            //    while (_operations.Count > 0)
            //    {
            //        ((Operation)_operations[0]).Dispose();
            //    }
            //    //_operations = null;
            //}
            _imageBytes = null;
            base.Dispose();
        }
        private Parser[] GetParsedFormat(string str)
        {
            if (str == null) return null;
            IList fields = new ArrayList();
            int state = 0;
            string tmp = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (delimiters.IndexOf(str[i]) > -1)
                {
                    switch (state)
                    {
                        case 0:
                            {
                                tmp = str[i].ToString();
                                state = 2;
                                break;
                            }
                        case 1:
                            {
                                fields.Add(new Parser(ParseType.Characters, tmp));
                                tmp = str[i].ToString();
                                state = 2;
                                break;
                            }
                        case 2:
                            {
                                tmp = tmp + str[i];
                                break;
                            }
                    }
                }
                else
                {
                    switch (state)
                    {
                        case 0:
                            {
                                tmp = str[i].ToString();
                                state = 1;
                                break;
                            }
                        case 1:
                            {
                                tmp = tmp + str[i];
                                break;
                            }
                        case 2:
                            {
                                fields.Add(new Parser(ParseType.Delimeter, tmp));
                                tmp = str[i].ToString();
                                state = 1;
                                break;
                            }
                    }
                }
            }
            switch (state)
            {
                case 1:
                    fields.Add(new Parser(ParseType.Characters, tmp));
                    break;
                case 2:
                    fields.Add(new Parser(ParseType.Delimeter, tmp));
                    break;
            }
            if (fields.Count > 0)
            {
                Parser[] parser = new Parser[fields.Count];
                fields.CopyTo(parser, 0);
                return parser;
            }
            return null;
        }
    }
    public enum ParseType { Delimeter, Characters }
    public class Parser
    {
        public Parser(ParseType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }
        public ParseType Type;
        public string Value;
    }
}