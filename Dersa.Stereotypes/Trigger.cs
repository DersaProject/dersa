using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class Trigger: ICompiledEntity
{
	public Trigger()
	{
	}
	public Trigger(IDersaEntity obj)
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
	#region Active
	public System.Boolean Active = true;
	#endregion
	#region InsteadOf
	public System.Boolean InsteadOf = false;
	#endregion
	#region Sequence
	public System.String Sequence;
	#endregion
	#region Source
	public System.String Source = "SQL";
	#endregion
	#region SQL
	public System.String SQL;
	#endregion
	#region TableExt
	public System.String TableExt;
	#endregion
	#region Type
	public System.String Type;
	#endregion
	#endregion

	#region Операции
	public System.String Drop(System.Boolean dialog, Dersa.Interfaces.ICompiledEntity owner)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		string sqlName = this.GetSqlName(owner);
		
		sb.Append("if exists (select * from sysobjects where id = object_id('dbo." + sqlName + "') and sysstat & 0xf = 8)\n");
		sb.Append("\tdrop trigger dbo." + sqlName + "\n");
		sb.Append("go\n\n");
		
		string sOut = sb.ToString();
        //if (dialog)
        //{
        //    SqlExecForm.Exec(sOut);
        //    return "";
        //}
		return sOut;
	}
	public System.String Generate(System.Boolean dialog, Dersa.Interfaces.ICompiledEntity owner)
	{
		if (Active == false) return "";
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.Append(Drop(false, owner));
		
		if ((SQL == null)||(SQL.Length == 0)) return "";
		string sqlName = GetSqlName(owner);
		
		sb.Append("create trigger dbo." + sqlName + " on dbo.");
		
		if (owner == null)
		{
			owner = this.Parent;
		}
		string ownerSqlName = GetOwnerSqlName(owner);
		
		sb.Append(ownerSqlName + TableExt + "\n");
		if (owner is View || this.InsteadOf)
		{
			sb.Append("instead of " + Type + "\nas\n");
		}
		else
		{
			sb.Append("for " + Type + "\nas\n");
		}
		if(Source == "SQL")
		{
			sb.Append(this.SQL);
		}
		else if (Source == "CSharp")
		{
			object result = Static.CompileAndExecuteAditionalMethod(owner, "System.String", this.Name, this.SQL);
			if (result != null)
			{
				sb.Append((string)result);
			}
		}
		
		sb.Append("\ngo\n\n");
		if ((this.Sequence != null)&&(this.Sequence.Length > 0))
		{
			sb.Append("exec dbo.sp_settriggerorder @triggername = '" + sqlName + "', @order = '" + Sequence + "', @stmttype = '" + Type + "'\n");
			sb.Append("\ngo\n\n");
		}
		string sOut = sb.ToString();
        //if (dialog)
        //{
        //    string serverName = "";
        //    string dbName = "";
        //    if (Parent is Entity && Parent.Parent is Package)  
        //    {
        //        serverName = ((Package)Parent.Parent).GetDatabaseServer();
        //        dbName = ((Package)Parent.Parent).GetDBName();
        //    }
        //    SqlExecForm.Exec(sOut, serverName, dbName);
        //    return "";
        //}
		return sOut;
	}
	public System.String GetOwnerSqlName(Dersa.Interfaces.ICompiledEntity owner)
	{
		if (owner == null)
		{
			owner = this.Parent;
			if (owner == null)
			{
				return "";
			}
		}
		string ownerSqlName = "";
		if (owner is Entity) ownerSqlName = ((Entity)owner).GetSqlName();
		else if (owner is View) ownerSqlName = ((View)owner).GetSqlName();
		return ownerSqlName;
	}
	public System.String GetSqlName(Dersa.Interfaces.ICompiledEntity owner)
	{
		string ownerSqlName = GetOwnerSqlName(owner);
		if (ownerSqlName.Length > 0)
		{
			return ownerSqlName + TableExt + "$" + this.Name;
		}
		return this.Name;
	}
	#endregion
}
}