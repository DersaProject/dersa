using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class Ref: ICompiledEntity
{
	public Ref()
	{
	}
	public Ref(IDersaEntity obj)
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
	#region Interface
	public System.String Interface = "IObject";
	#endregion
	#region Prefix
	public System.String Prefix = "source";
	#endregion
	#region PropName
	public System.String PropName;
	#endregion
	#region RefObjectIdType
	public System.String RefObjectIdType = "int";
	#endregion
	#region set_class
	public System.String set_class;
	#endregion
	#region set_ref
	public System.String set_ref;
	#endregion
	#endregion

	#region Операции
	public System.String Generate()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		
		string pName = GenerateName(); 
		string lowerName = pName.ToLower();
		string interf = this.Interface;
		string prefix = this.Prefix;
		
		sb.Append("\t\tpublic " + interf + " "  + pName + "Ref" + "\n");
		sb.Append("\t\t{\n");
		sb.Append("\t\t\tget\n");
		sb.Append("\t\t\t{\n");
		sb.Append("\t\t\t\tif ((this." + prefix+ "_ref.IsNull)||(this." + prefix + "_class.IsNull)) return null;\n");
		sb.Append("\t\t\t\treturn GetObject(this." + prefix + "_class, this." + prefix + "_ref) as " + interf + ";\n");
		sb.Append("\t\t\t}\n");
		sb.Append("\t\t\tset\n");
		sb.Append("\t\t\t{\n");
		sb.Append("\t\t\t\tif (value!= null)\n");
		sb.Append("\t\t\t\t{\n");
		sb.Append("\t\t\t\t\tthis." + prefix + "_class = value.Entity.ClassName;\n");
		sb.Append("\t\t\t\t\tthis." + prefix + "_ref = (" + Static.GetCSharpNativeType(this.RefObjectIdType) + ")value.id;\n");
		sb.Append("\t\t\t\t}\n");
		sb.Append("\t\t\t\telse\n"); 
		sb.Append("\t\t\t\t{\n");
		sb.Append("\t\t\t\t\tthis." + prefix+ "_class = new SqlString();\n");
		sb.Append("\t\t\t\t\tthis." + prefix + "_ref = new " + Static.GetCSharpType(this.RefObjectIdType) + "();\n");
		sb.Append("\t\t\t\t}\n");
		sb.Append("\t\t\t}\n");
		sb.Append("\t\t}\n");
		return sb.ToString();
	}
	public System.String GenerateInterface()
	{
		string pName = GenerateName(); 
		string interf = this.Interface;
		return "\t\t" + interf + " "  + pName + "Ref{get;set;}" + "\n";
	}
	public System.String GenerateName()
	{
		string name = this.PropName;
		if (name != null && name.Length > 0)
		{
			return name;
		}
		return this.Name;
	}
	#endregion
}
}