using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class StateEngine: ICompiledEntity
{
	public StateEngine()
	{
	}
	public StateEngine(IDersaEntity obj)
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
	#region Accusativus
	public System.String Accusativus;
	#endregion
	#region Default
	public System.String Default;
	#endregion
	#region GenerateTable
	public System.Boolean GenerateTable = false;
	#endregion
	#region Get
	public System.String Get;
	#endregion
	#region NameSQLType
	public System.String NameSQLType = "varchar(40)";
	#endregion
	#region Null
	public System.String Null = "null";
	#endregion
	#region PhisicalName
	public System.String PhisicalName;
	#endregion
	#region Set
	public System.String Set;
	#endregion
	#region SqlType
	public System.String SqlType = "int";
	#endregion
	#region UseTransitionParams
	public System.Boolean UseTransitionParams = true;
	#endregion
	#region DIOSDefault
	public System.String DIOSDefault;
	#endregion
	#endregion

	#region Операции
	public System.String GenerateAvailableTransitions()
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		System.Collections.IList children = this.Children;
		
		result.Append("\t\t#region GetAvailableTransitions\n");
		result.Append("\t\t[ObjectMethod(\"\", false)]\n");
		result.Append("\t\tpublic IObjectCollection GetAvailableTransitions()\n");
		result.Append("\t\t{\n");
		result.Append("\t\t\tIDersaEntity SEEntity = Manager.GetEntity(\"SE_AVAILABLE_METHOD\");\n");
		result.Append("\t\t\tIObjectCollection result = SEEntity.EmptyList();\n");
		result.Append("\t\t\tIndexerPropertyDescriptorCollection objectProperties = (IndexerPropertyDescriptorCollection)SEEntity.GetObjectProperties();\n");
		result.Append("\t\t\tstring diag = string.Empty;\n");
		result.Append("\t\t\tbool avail = false;\n");
		
		result.Append("\t\t\tswitch(this." + Static.GetCSharpObjectName(this) + ")\n");
		result.Append("\t\t\t{\n");
		int id = 0;
		foreach(ICompiledEntity entity in children)
		{
			System.Collections.IList relations = entity.ARelations;
			if (entity is StartState || entity is State)
			{	
				result.Append("\t\t\t\tcase " + Static.GetCSharpObjectName(this) + "." + entity.Name + ":\n");
				for (int i = 0; i < relations.Count; i++)
				{
					ICompiledRelation rel = (ICompiledRelation)relations[i];
					if (rel is Transition && 
						(rel.A is StartState || rel.A is State || rel.A is StopState) &&
						(rel.B is StartState || rel.B is State || rel.B is StopState))
					{
						Transition tr = rel as Transition;
						result.Append("\t\t\t\t\tdiag = this.CanGo" + tr.GetFromTo() + ";\n");
						result.Append("\t\t\t\t\tavail = diag == string.Empty;\n");
						id++;
						result.Append("\t\t\t\t\tresult.Add(new UniStructView(new object[]{new SqlInt32(" + id.ToString() + "),");
						if (tr.TransitionDescription == null || tr.TransitionDescription == string.Empty)
							result.Append(" new SqlString(\"Перейти к " + tr.B.Name + "\"),");
						else
							result.Append(" new SqlString(\"" + tr.TransitionDescription + "\"),");
						result.Append(" new SqlString(\"Go" + tr.GetFromTo() + "\"),");
						result.Append(" new SqlBoolean(avail), new SqlString(diag),");
						result.Append(" new SqlString(\"GetGo" + tr.GetFromTo() + "Params\")},");
						result.Append(" objectProperties));\n");
					}
				}
				result.Append("\t\t\t\t\tbreak;\n");
			}
		}
		result.Append("\t\t\t\tdefault:\n");
		result.Append("\t\t\t\t\tbreak;\n");
		result.Append("\t\t\t}\n");
		result.Append("\t\t\treturn result;\n");
		result.Append("\t\t}\n");
		result.Append("\t\t#endregion GetAvailableTransitions\n");
		return result.ToString();
	}
	public System.String GenerateEngine()
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		
		result.Append("\t\t#region StateEngine " + this.Name + "\n");
		result.Append(this.GenerateAvailableTransitions());
		System.Collections.IList children = this.Children;
		foreach(ICompiledEntity entity in children)
		{
			System.Collections.IList relations = entity.ARelations;
			
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Transition && 
					(rel.A is StartState || rel.A is State || rel.A is StopState) &&
					(rel.B is StartState || rel.B is State || rel.B is StopState))
				{
					Transition tr = rel as Transition;
					bool useTransitionParams = tr.UseTransitionParams;
					if (tr.UseTransitionParamsFromStateEngine)
						useTransitionParams = this.UseTransitionParams;
					result.Append("\t\t#region " + tr.GetLocalizedFromTo() + " <" + tr.TransitionDescription + ">\n");
					/* Property */
					result.Append("\t\tpublic string CanGo" + tr.GetFromTo() + "\n");
					result.Append("\t\t{\n");
					result.Append("\t\t\tget\n");
					result.Append("\t\t\t{\n");
					if (tr.GetCanGo != null && tr.GetCanGo != "")
					{
						result.Append(tr.GetCanGo + "\n");
					}
					else
					{
						result.Append("\t\t\t	if (!this.Entity.CheckCanExecuteMethod(\"Go" + tr.GetFromTo() + "\"))\n");
						result.Append("\t\t\t		return \"Вы не имеете прав на операцию \\\"" + tr.TransitionDescription + "\\\"\";\n");
						result.Append("\t\t\t	if (this." + Static.GetCSharpObjectName(this) + " != " + Static.GetCSharpObjectName(this)+ "." + tr.A.Name + ")\n");
						result.Append("\t\t\t		return \"Нельзя " + tr.TransitionDescription + " " + this.Accusativus + " в статусе \" + this." + Static.GetCSharpObjectName(this) + ".ToString();\n");
						result.Append("\t\t\t	return string.Empty;\n");
					}
					result.Append("\t\t\t}\n");
					result.Append("\t\t}\n");
		
					/* Go method */
					result.Append("\t\t[ObjectMethod(\"\", true)]\n");
					if (useTransitionParams)
						result.Append("\t\tpublic void Go" + tr.GetFromTo() + "(IParameterCollection Params)\n");
					else
						result.Append("\t\tpublic void Go" + tr.GetFromTo() + "()\n");
					result.Append("\t\t{\n");
					if (tr.Go != null && tr.Go != "")
					{
						result.Append(tr.Go + "\n");
					}
					else
					{
						result.Append("\t\t\tstring canGo = this.CanGo" + tr.GetFromTo() + ";\n");
						result.Append("\t\t\tif (canGo != string.Empty)\n");
						result.Append("\t\t\t\tthrow new SimpleException(canGo);\n");
						result.Append("\t\t\tbool pooled = this.ObjState == ObjectState.Pooled;\n");
						result.Append("\t\t\tif (pooled) this.Load();\n");
						if (useTransitionParams)
						{
							result.Append("\t\t\tif (Params != null && Params.Count > 0)\n");
							result.Append("\t\t\t\tthis.ApplyParams(Params);\n");
						}
						result.Append("\t\t\tthis." + Static.GetCSharpObjectName(this) + " = " + Static.GetCSharpObjectName(this)+ "." + tr.B.Name + ";\n");
						result.Append("\t\t\tif (pooled) this.Post();\n");
					}
					result.Append("\t\t}\n");
					
					if (useTransitionParams)
					{
						/* Get Go Params method */
						result.Append("\t\t[ObjectMethod(\"\", false)]\n");
						result.Append("\t\tpublic IObjectCollection GetGo" + tr.GetFromTo() + "Params()\n");
						result.Append("\t\t{\n");
						if (tr.GetGoParams != null && tr.GetGoParams != "")
						{
							result.Append(tr.GetGoParams + "\n");
						}
						else
						{
							result.Append("\t\t\tIDersaEntity SEEntity = Manager.GetEntity(\"SE_METHOD_PARAM\");\n");
							result.Append("\t\t\tIObjectCollection result = SEEntity.EmptyList();\n");
							result.Append("\t\t\tIndexerPropertyDescriptorCollection objectProperties = (IndexerPropertyDescriptorCollection)SEEntity.GetObjectProperties();\n");
							if (tr.TransitionDescription == null || tr.TransitionDescription == string.Empty)
								result.Append("\t\t\tresult.Add(new UniStructView(new object[]{new SqlInt32(1), new SqlString(\"Перейти в состояние " + tr.B.Name + "\"), SqlString.Null, SqlString.Null, new SqlBoolean(false), SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null}, objectProperties));\n");
							else
								result.Append("\t\t\tresult.Add(new UniStructView(new object[]{new SqlInt32(1), new SqlString(\"" + tr.TransitionDescription + "\"), SqlString.Null, SqlString.Null, new SqlBoolean(false), SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null, SqlString.Null}, objectProperties));\n");
							result.Append("\t\t\treturn result;\n");
						}
						result.Append("\t\t}\n");
					}
					result.Append("\t\t#endregion " + tr.GetLocalizedFromTo() + " <" + tr.TransitionDescription + ">\n");
				}
			}
		}
		result.Append("\t\t#endregion StateEngine " + this.Name + "\n");
		return result.ToString();
	}
	public System.String GenerateEnum()
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		
		result.Append("\tpublic enum " + Static.GetCSharpObjectName(this) + ": " + Static.GetCSharpNativeType(this.SqlType) + " {");
		string comma = "";
		System.Collections.IList children = this.Children;
		for(int i = 0; i < children.Count; i++)
		{
			string name = "";
			string value = "";
			object obj = children[i];
			if (obj is StartState)
			{
				name = (obj as StartState).Name;
				value = (obj as StartState).Value;
			}
			else if (obj is State)
			{
				name = (obj as State).Name;
				value = (obj as State).Value;
			}
			else if (obj is StopState)
			{
				name = (obj as StopState).Name;
				value = (obj as StopState).Value;
			}
			if (name != "")
			{
				result.Append(comma + name);
				if ((value != null)&&(value != ""))
				{
					result.Append(" = " + value);			
				}
				comma = ", ";
				}
		}
		result.Append("}\n");
		
		return result.ToString();
	}
	public System.String GenerateInterfaceEngine()
	{
		System.Text.StringBuilder result = new System.Text.StringBuilder();
		
		result.Append("\t\t#region StateEngine " + this.Name + "\n");
		result.Append("\t\t\t[ObjectMethod(\"\", false)]\n");
		result.Append("\t\t\tIObjectCollection GetAvailableTransitions();\n");
		System.Collections.IList children = this.Children;
		foreach(ICompiledEntity entity in children)
		{
			System.Collections.IList relations = entity.ARelations;
            result.Append("\n//" + entity.Name + " relations.Count = " + relations.Count.ToString() + "\n");
			
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Transition && 
					(rel.A is StartState || rel.A is State || rel.A is StopState) &&
					(rel.B is StartState || rel.B is State || rel.B is StopState))
				{
					Transition tr = rel as Transition;
					bool useTransitionParams = tr.UseTransitionParams;
					if (tr.UseTransitionParamsFromStateEngine)
						useTransitionParams = this.UseTransitionParams;
					result.Append("\t\t#region " + tr.GetLocalizedFromTo() + "\n");
					/* Property */
					result.Append("\t\t\t[ObjectPropertyAttribute(\"можно " + tr.GetLocalizedFromTo() + "\", true, false)]\n");
					result.Append("\t\t\tstring CanGo" + tr.GetFromTo() + "{get;}\n");
					/* Go method */
					result.Append("\t\t\t[ObjectMethod(\"\", true, false)]\n");
					if (useTransitionParams)
						result.Append("\t\t\tvoid Go" + tr.GetFromTo() + "(IParameterCollection Params);\n");
					else
						result.Append("\t\t\tvoid Go" + tr.GetFromTo() + "();\n");
					
					if (useTransitionParams)
					{
						/* Get Go Params method */
						result.Append("\t\t\t[ObjectMethod(\"\", false, true)]\n");
						result.Append("\t\t\tIObjectCollection GetGo" + tr.GetFromTo() + "Params();\n");
					}
					result.Append("\t\t#endregion " + tr.GetLocalizedFromTo() + "\n");
				}
			}
		}
		result.Append("\t\t#endregion StateEngine " + this.Name + "\n");
		return result.ToString();
	}
	public System.Collections.IList GetPKAttributes()
	{
		System.Collections.ArrayList attrs = new System.Collections.ArrayList();
		Attribute attr = new Attribute(null);
		attr.Name = "#";
		attr.PhisicalName = this.GetSqlName();
		attr.Type = this.SqlType;
		attr.Null = "not null";
		attr.Default = this.Default;
		attr.Description = "PRIMARY KEY";
		attr.Get = this.Get;
		attr.Set = this.Set;
		attr.DIOSDefault = this.DIOSDefault;
		attrs.Add(attr);
		return attrs;
	}
	public System.String GetSqlName()
	{
		string s = this.PhisicalName;
		if (!string.IsNullOrEmpty(s))
		{
			return s;
		}
		else
		{
			return this.Name.ToUpper();
		}
	}
	#endregion
}
}