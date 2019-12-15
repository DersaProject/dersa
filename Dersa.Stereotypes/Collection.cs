using System;  
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
[Serializable()]
public class Collection: ICompiledEntity
{
	public Collection()
	{
	}
	public Collection(IEntity obj)
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
	protected Dersa.Interfaces.ICompiledEntity _parent;
	public virtual Dersa.Interfaces.ICompiledEntity Parent
	{
		get
		{
			if (_parent == null)
			{
				Dersa.Interfaces.IEntity parent = _object.Parent;
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
	#region FileName
	public System.String FileName = "initscript.sql";
	#endregion
	#region FolderName
	public System.String FolderName = "d:\\Git\\tsuks\\";
	#endregion
	#endregion

	#region Операции
	public string sql_Generate()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		System.Collections.IList relations = this.ARelations;
		System.Collections.ArrayList SL = new System.Collections.ArrayList();
		System.Collections.Hashtable T = new System.Collections.Hashtable();
		System.Collections.ArrayList Rels = new System.Collections.ArrayList();
		
		for (int i = 0; i < relations.Count; i++)
		{
			ICompiledRelation rel = (ICompiledRelation)relations[i];
			if (rel is Inherit)
			{
				string P = (rel as Inherit).Prefix;
				SL.Add(P);
				T.Add(P, rel);
				//Entity rB = (Entity)rel.B;
				//sb.Append("\n");
				//sb.Append(rB.sql_Generate(false, true, true, true, true));
			}
		}
		
		
		SL.Sort();
		foreach(string key in SL)
		{
			Rels.Add(T[key] as Inherit);
		}
		
		foreach(Inherit Inh in Rels)
		{
				Entity rB = (Entity)Inh.B;
				sb.Append("\n");
				if(Inh.NamePrefix.IndexOf("A") >= 0)
				{
					if(Inh.NamePrefix.IndexOf("T") >= 0)
						sb.Append(rB.sql_GenerateTriggers(false));
					if(Inh.NamePrefix.IndexOf("P") >= 0)
						sb.Append(rB.sql_GenerateStoredProcedures(false));
				}
				else
				{
					sb.Append(rB.sql_Generate(false, Inh.NamePrefix.IndexOf("D") >= 0, Inh.NamePrefix.IndexOf("P") >= 0, Inh.NamePrefix.IndexOf("T") >= 0, true));
		//System.Boolean dialog = true, System.Boolean doDrop = true, System.Boolean doSP = true, System.Boolean doTrig = true, System.Boolean doRef = true
					sb.Append(rB.sql_Alter(false, false, false));
		//System.Boolean dialog = true, System.Boolean doSP = true, System.Boolean doTrig = true
				}
		}
		
		string res = sb.ToString();
		System.Collections.IList children = this.Children;
		for (int i = 0; i < children.Count; i++)
		{
			ICompiledEntity obj = (ICompiledEntity)children[i];
			if (obj is Script)
			{
				Script script = (Script)obj;
				if (script.Name == "Corrections")
				{
					res = script.Execute(this, new object[]{res}).ToString();
				}
			}
		}
		string fileName = FolderName + FileName;
		Static.SaveToFile(fileName, res);
		Console.WriteLine(res);
		return res;
	}
	#endregion
}
}