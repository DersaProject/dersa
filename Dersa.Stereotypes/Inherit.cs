using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class Inherit: ICompiledRelation
{
	public Inherit()
	{
	}
	public Inherit(IRelation obj)
	{
		_object = obj;
		if (_object != null)
		{
			_name = _object.Name;
			_id = _object.Id;
		}
	}
	protected IRelation _object;
	public IRelation Object
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
	#region A
	protected Dersa.Interfaces.ICompiledEntity _a;
	public virtual Dersa.Interfaces.ICompiledEntity A
	{
		get
		{
			if (_a == null)
			{
				Dersa.Interfaces.IDersaEntity a = _object.A;
				if (a != null) _a = a.GetInstance();
				return _a;
			}
			else
			{
				return _a;
			}
		}
	}
	#endregion
	#region B
	protected Dersa.Interfaces.ICompiledEntity _b;
	public virtual Dersa.Interfaces.ICompiledEntity B
	{
		get
		{
			if (_b == null)
			{
				Dersa.Interfaces.IDersaEntity b = _object.B;
				if (b != null) _b = b.GetInstance();
				return _b;
			}
			else
			{
				return _b;
			}
		}
	}
	#endregion
	#endregion
	#region Наследуемые методы
	public void SetA(Dersa.Interfaces.ICompiledEntity a)
	{
		this._a = a;
	}
	public void SetB(Dersa.Interfaces.ICompiledEntity b)
	{
		this._b = b;
	}
	#endregion

	#region Атрибуты
	#region ContA
	public System.String ContA;
	#endregion
	#region ContB
	public System.String ContB;
	#endregion
    #region InheritAttributes
    public System.Boolean InheritAttributes = true;
    #endregion 
	#region InheritFKLinks
	public System.Boolean InheritFKLinks = true;
	#endregion
	#region InheritGetAccessors
	public System.Boolean InheritGetAccessors = true;
	#endregion
	#region InheritIndices
	public System.Boolean InheritIndices = false;
	#endregion
	#region InheritInterfaces
	public System.Boolean InheritInterfaces = true;
	#endregion
	#region InheritMethods
	public System.Boolean InheritMethods = false;
	#endregion
	#region InheritPK
	public System.Boolean InheritPK = false;
	#endregion
	#region InheritProcedures
	public System.Boolean InheritProcedures = true;
	#endregion
	#region InheritProperties
	public System.Boolean InheritProperties = true;
	#endregion
	#region InheritRefs
	public System.Boolean InheritRefs = true;
	#endregion
	#region InheritScripts
	public System.Boolean InheritScripts = true;
	#endregion
	#region InheritSetAccessors
	public System.Boolean InheritSetAccessors = true;
	#endregion
	#region InheritTriggers
	public System.Boolean InheritTriggers = true;
	#endregion
	#region InheritUsing
	public System.Boolean InheritUsing = false;
	#endregion
	#region InheritViewAttributes
	public System.Boolean InheritViewAttributes = true;
	#endregion
	#region NamePrefix
	public System.String NamePrefix;
	#endregion
	#region Prefix
	public System.String Prefix;
	#endregion
	#endregion

	#region Операции
	#endregion
}
}