using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class Transition: ICompiledRelation
{
	public Transition()
	{
	}
	public Transition(IRelation obj)
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
	#region FromTo
	public System.String FromTo;
	#endregion
	#region GetCanGo
	public System.String GetCanGo;
	#endregion
	#region GetGoParams
	public System.String GetGoParams;
	#endregion
	#region Go
	public System.String Go;
	#endregion
	#region TransitionDescription
	public System.String TransitionDescription;
	#endregion
	#region UseTransitionParams
	public System.Boolean UseTransitionParams = true;
	#endregion
	#region UseTransitionParamsFromStateEngine
	public System.Boolean UseTransitionParamsFromStateEngine = true;
	#endregion
	#endregion

	#region Операции
	public System.String GenerateInterfaceTransition()
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		/*
		System.Collections.IList children = this.Children;
		foreach(ICompiledEntity entity in children)
		{
			System.Collections.IList relations = entity.ARelations;
			
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Transition)
				{
					System.Reflection.FieldInfo fiA = rel.A.GetType().GetField("StateName");
					System.Reflection.FieldInfo fiB = rel.B.GetType().GetField("StateName");
					if (fiA == null || fiB == null) result.Append(" !!!! " + rel.A.Name + " -> " + rel.B.Name + "--" + "\n");
		
					string StateNameA = (string)fiA.GetValue(rel.A);
					string StateNameB = (string)fiB.GetValue(rel.B);
		
					result.Append(StateNameA + " -> " + StateNameB + "\n");
				}
			}
		}
		*/
		return result.ToString();
	}
	public System.String GenerateTransition()
	{
		return "";
	}
	public System.String GetFromTo()
	{
		if (this.FromTo != null && this.FromTo != "")
			return this.FromTo;
			
		System.Reflection.FieldInfo fiA = this.A.GetType().GetField("StateName");
		System.Reflection.FieldInfo fiB = this.B.GetType().GetField("StateName");
		
		string StateNameA = (string)fiA.GetValue(this.A);
		if (StateNameA == null || StateNameA == "")
			throw new Exception("У статуса " + this.A.Name + " не задано StateName");
		string StateNameB = (string)fiB.GetValue(this.B);
		if (StateNameB == null || StateNameB == "")
			throw new Exception("У статуса " + this.B.Name + " не задано StateName");
		return "From" + StateNameA + "To" + StateNameB;
	}
	public System.String GetLocalizedFromTo()
	{
		return this.A.Name + " -> " + this.B.Name;
	}
	#endregion
}
}