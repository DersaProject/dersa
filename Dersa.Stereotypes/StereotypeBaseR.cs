using System;  
using System.Runtime.Serialization;
using Modeler.Interfaces;
using System.Windows.Forms;

namespace ModelerStereotypes
{
[Serializable()]
public class StereotypeBaseR: ICompiledEntity
{
	public StereotypeBaseR()
	{
	}
	public StereotypeBaseR(IEntity obj)
	{
		_object = obj;
		if (_object != null)
		{
			_name = _object.Name;
			_id = _object.Id;
		}
	}
	protected IEntity _object;
	public IEntity Object
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
	protected Modeler.Interfaces.ICompiledEntity _parent;
	public virtual Modeler.Interfaces.ICompiledEntity Parent
	{
		get
		{
			if (_parent == null)
			{
				Modeler.Interfaces.IEntity parent = _object.Parent;
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
	public void SetParent(Modeler.Interfaces.ICompiledEntity parent)
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
	#endregion

	#region Операции
	#endregion
}
}