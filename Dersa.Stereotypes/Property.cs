using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class Property: ICompiledEntity
{
	public Property()
	{
	}
	public Property(IDersaEntity obj)
	{
		_object = obj;
		if (_object != null)
		{
			_name = _object.Name;
			_id = _object.Id;
		}
	}
	protected IDersaEntity _object;
	public IDersaEntity Object
	{
		get{ return _object;}
	}
	#region Наследуемые свойства
	#region Name
	protected System.String _name;
	public virtual System.String Name
	{
		get{ return _name;}
		set{ _name = value;}
	}
	#endregion
	#region Id
	protected System.Int32 _id;
	public virtual System.Int32 Id
	{
		get{ return _id;}
	}
	#endregion
	#region Parent
	protected Dersa.Interfaces.ICompiledEntity _parent;
	public virtual Dersa.Interfaces.ICompiledEntity Parent
	{
		get
		{
			if (_parent == null)
			{
				Dersa.Interfaces.IDersaEntity parent = _object.Parent;
				if (parent != null) _parent = parent.GetInstance();
				return _parent;
			}
			else
			{
				return _parent;
			}
		}
	}
	#endregion
	#region Children
	protected System.Collections.IList _children;
	public virtual System.Collections.IList Children
	{
		get
		{
			if (_children == null)
			{
				_children = _object.ChildrenInstance();
				return _children;
			}
			else
			{
				return _children;
			}
		}
	}
	#endregion
	#region ARelations
	protected System.Collections.IList _aRelations;
	public virtual System.Collections.IList ARelations
	{
		get
		{
			if (_aRelations == null)
			{
				_aRelations = _object.ARelationsInstance();
				return _aRelations;
			}
			else
			{
				return _aRelations;
			}
		}
	}
	#endregion
	#region BRelations
	protected System.Collections.IList _bRelations;
	public virtual System.Collections.IList BRelations
	{
		get
		{
			if (_bRelations == null)
			{
				_bRelations = _object.BRelationsInstance();
				return _bRelations;
			}
			else
			{
				return _bRelations;
			}
		}
	}
	#endregion
	#endregion
	#region Наследуемые методы
	public void SetParent(Dersa.Interfaces.ICompiledEntity parent)
	{
		this._parent = parent;
	}
	public void SetARelations(System.Collections.IList aRelations)
	{
		this._aRelations = aRelations;
	}
	public void SetBRelations(System.Collections.IList bRelations)
	{
		this._bRelations = bRelations;
	}
	public void SetChildren(System.Collections.IList children)
	{
		this._children = children;
	}
	#endregion

	#region Атрибуты
	#region AccessModifier
	public System.String AccessModifier = "public";
	#endregion
	#region CreatePrivateField
	public System.Boolean CreatePrivateField = false;
	#endregion
	#region Default
	public System.String Default;
	#endregion
	#region Get
	public System.String Get;
	#endregion
	#region Interface
	public System.Boolean Interface = false;
	#endregion
	#region ReadOnly
	public System.Boolean ReadOnly = true;
	#endregion
	#region Set
	public System.String Set;
	#endregion
	#region Static
	public System.Boolean Static = false;
	#endregion
	#region Type
	public System.String Type = "System.String";
	#endregion
	#endregion

	#region Операции
	public System.String Generate()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		bool isStatic = this.Static;
		string propertyType = this.Type;
		bool createPrivateField = this.CreatePrivateField;
		string privatePropertyName = GetPrivatePropertyName();
		if (createPrivateField)
		{
			sb.Append("\t\tprivate ");
			if (isStatic)
			{
				sb.Append("static ");
			}
			sb.Append(propertyType + " " + privatePropertyName);
			string propertyDefault = this.Default;
            if (propertyDefault == null) propertyDefault = "";
			if (propertyDefault.Length > 0)
			{
				sb.Append(" = " + propertyDefault);
			}
			sb.Append(";\n");
		}
		if (this.AccessModifier.IndexOf("private") > -1)
		{
			sb.Append("\t\t" + this.AccessModifier + " ");
		}
		else
		{
			sb.Append("\t\t" + this.AccessModifier + " ");
		}
		if (isStatic)
		{
			sb.Append("static ");
		}
		sb.Append(propertyType + " " + this.Name + "\n");
		sb.Append("\t\t{\n");

        if (this.Set == null) this.Set = "";
        if (this.Get == null) this.Get = "";
        string getCode = this.Get;
        string setCode = this.Set;
		
		if (getCode.Length > 0)
		{
			sb.Append("\t\t\tget\n" +  "\t\t\t{\n"  + getCode + "\n" + "\t\t\t}\n");
		}
		else if (createPrivateField)
		{
			sb.Append("\t\t\tget\n\t\t\t{\n\t\t\t\treturn this." + privatePropertyName + ";\n\t\t\t}\n");
		}
		if (setCode.Length > 0)
		{
			sb.Append("\t\t\tset\n" +  "\t\t\t{\n"  + setCode + "\n" + "\t\t\t}\n");
		}
		sb.Append("\t\t}\n");
		return sb.ToString();
	}
	public System.String GenerateInterface()
	{
		string accessModifier = this.AccessModifier;
		if (accessModifier != "public") return "";
		if (!Interface) return "";
		
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		string propertyType = this.Type;
		sb.Append("\t\t" + propertyType + " " + this.Name + " {");
		if ((this.Get.Length > 0)||(CreatePrivateField))
		{
			sb.Append("get;");
		}
		
		if (this.Set.Length > 0)
		{
			sb.Append("set;");
		}
		sb.Append("}\n");
		return sb.ToString();
	}
	public System.String GetPrivatePropertyName()
	{
		string s = this.Name;
		string ret = "_";
		for (int i = 0; i < s.Length; i++)
		{
			char buf = s[i];
			if (buf == ' ') buf = '_';
			if (i == 0)
			{
				buf = Char.ToLower(buf);
			}
			ret += buf;
		}
		return ret;
	}
	#endregion
}
}
