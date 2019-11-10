using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class DataSet: ICompiledEntity
{
	public DataSet()
	{
	}
	public DataSet(IDersaEntity obj)
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
	#region ����������� ��������
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
	#region ����������� ������
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

	#region ��������
	#region xml
	public System.String xml;
	#endregion
	#endregion

	#region ��������
	public void Generate()
	{
		Package package = this.Parent as Package;
		if (package == null) return;
		
		string fileName = package.GetDirectory() + "\\" + this.Name + ".xsd";
		
		Static.SaveToFile(fileName, this.GetText());
		Console.WriteLine("�����������: " + fileName);
		//System.Windows.Forms.Application.DoEvents();
	}
	public string GetText()
	{
		System.Collections.IList relations = this.ARelations;
		for (int i = 0; i < relations.Count; i++)
		{
			ICompiledRelation rel = (ICompiledRelation)relations[i];
			if (rel is Inherit)
			{
				DataSet parent = rel.B as DataSet;
				if (parent != null)
				return parent.GetText();
			}
		}
		return this.xml;
	}
	#endregion
}
}