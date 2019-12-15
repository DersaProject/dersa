using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class Interface: ICompiledEntity
{
	public Interface()
	{
	}
	public Interface(IDersaEntity obj)
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
	#region Interfaces
	public System.String Interfaces;
	#endregion
	#region PhisicalName
	public System.String PhisicalName;
	#endregion
	#endregion

	#region Операции
	public void GenerateInterface(System.Boolean dialog)
	{
		//if ((dialog)&&(!Static.Ask("Сгенерировать Интерфейс " + this.Name))) return;
		System.Text.StringBuilder sbInterface = new System.Text.StringBuilder();
		
		Package package = this.Parent as Package;
		if (package == null) return;
		
		string interfaces = this.Interfaces;
		string interfacesNamespace = package.Namespace;
		string globalName = package.GetGlobalName();
		string cSharpName = this.GetPhisicalName();
		
		System.Collections.IList attrs = this.GetAttributes();
		
		sbInterface.Append(package.GetUsing("using System;\nusing " + globalName + ".Common;\nusing " + globalName + ".Common.Interfaces;\n"));
		
		sbInterface.Append("\nnamespace " + interfacesNamespace + "\n{\n"); 
		sbInterface.Append("\tpublic interface " + cSharpName);
		if (interfaces.Length > 0)
		{
			sbInterface.Append(": " + interfaces);
		}
		sbInterface.Append("\n");
		sbInterface.Append("\t{\n"); 
		
		System.Text.StringBuilder sbProperties = new System.Text.StringBuilder();
		string coma = "";
		foreach (Attribute attr in attrs)
		{
			string attr_name = attr.Name;
			string attr_phisical_name = attr.GetSqlName();
			string attr_null = attr.Null;
			string attr_wibs_type = attr.GetDIOSType();
			bool attr_readonly = attr.ReadOnly;
			
			sbProperties.Append(coma + attr_phisical_name);
			coma = ", ";
		
			string objectPropertyAttribute = "\t\t[ObjectPropertyAttribute(\"" + attr_name + "\"";
			if (attr_null == "not null")
			{
				objectPropertyAttribute += ", true";
			}
			else
			{
				objectPropertyAttribute += ", false";
			}
			objectPropertyAttribute += ")]\n";
			sbInterface.Append(objectPropertyAttribute);
			sbInterface.Append("\t\t" + attr_wibs_type + " " + attr_phisical_name + "{get;");
			if (!attr_readonly)
			{
				sbInterface.Append("set;");
			}
			sbInterface.Append("}\n");	
		}
		sbInterface.Append("\t\t#region Refs\n");
		System.Collections.IList children = this.Children;
		for (int i = 0; i < children.Count; i++)
		{
			Ref reff = children[i] as Ref;
			if (reff == null) continue;
			sbInterface.Append(reff.GenerateInterface());
		}
		sbInterface.Append("\t\t#endregion\n");
		
		sbInterface.Append("\t\t#region Методы\n");
		sbInterface.Append(this.GenerateMethods());
		sbInterface.Append("\t\t#endregion\n");
		sbInterface.Append("\t}\n");
		sbInterface.Append("}");
		
		string interfaceFileName = package.GetDirectory() + "\\" + cSharpName + ".cs";
		
		Static.SaveToFile(interfaceFileName, sbInterface.ToString());
		if (dialog)
		{
			Static.Information(interfaceFileName);
		}
	}
	public System.String GenerateMethods()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		System.Collections.IList methods = this.GetMethods();
		for (int i = 0; i < methods.Count; i++)
		{
			Method method = methods[i] as Method;
			if (method != null) sb.Append(method.GenerateInterface());
		}
		return sb.ToString();
	}
	public System.Collections.IList GetAttributes()
	{
		Map attributes = new Map();
		System.Collections.IList relations = this.ARelations;
		for (int i = 0; i < relations.Count; i++)
		{
			ICompiledRelation rel = (ICompiledRelation)relations[i];
			if (rel is Inherit)
			{
				System.Collections.IList inDictionary = ((Interface)rel.B).GetAttributes();
				foreach (Attribute a in inDictionary)
				{
					attributes.Add(a.GetSqlName(), a);
				}
			}
		}
		System.Collections.IList children = this.Children;
		for (int i = 0; i < children.Count; i++)
		{
			ICompiledEntity obj = (ICompiledEntity)children[i];
			if (obj is Attribute)
			{
				attributes.Add(((Attribute)obj).GetSqlName(), obj);
			}
			else if (obj is Ref)
			{
				Ref reff = obj as Ref;
				string pName = reff.GenerateName(); 
				string prefix = reff.Prefix;
				
				string key = prefix + "_ref";
				Attribute attr = new Attribute(null);
				attr.Name = key;
				attr.Type = reff.RefObjectIdType;
				attr.Null = "null";
				attr.Default = "";
				attr.Description = "";
				attributes.Add(attr.GetSqlName(), attr);
				
				key = prefix + "_class";
				attr = new Attribute(null);
				attr.Name = key;
				attr.Type = "varchar(128)";
				attr.Null = "null";
				attr.Default = "";
				attr.Description = "";
				attributes.Add(attr.GetSqlName(), attr);
			}
			else if (obj is Property)
			{
				Property prop = obj as Property;
				Attribute attr = new Attribute(null);
				attr.Name = prop.Name;
				attr.DIOSType = prop.Type;
				attr.Null = "null";
				attr.Default = "";
				attr.Description = "";
				attr.ReadOnly = prop.ReadOnly;
				attributes.Add(attr.GetSqlName(), attr);
			}
		}
		return attributes;
	}
	public System.String GetFullName()
	{
		Package package = this.Parent as Package;
		if (package == null) return "";
		if (package.Namespace != null && package.Namespace.Trim().Length > 0)
			return package.Namespace + "." + this.GetPhisicalName();
		else
			return this.GetPhisicalName();
		
	}
	public System.Collections.IList GetMethods()
	{
		System.Collections.ArrayList methods = new System.Collections.ArrayList();
		System.Collections.IList children = this.Children;
		for (int i = 0; i < children.Count; i++)
		{
			Method obj = children[i] as Method;
			if (obj == null) continue;
			methods.Add(obj);
		}
		System.Collections.IList relations = this.ARelations;
		for (int i = 0; i < relations.Count; i++)
		{
			ICompiledRelation rel = (ICompiledRelation)relations[i];
			if (rel is Inherit)
			{
				System.Collections.IList inheritMethods = (rel.B as Interface).GetMethods();
				methods.AddRange(inheritMethods);
			}
		}
		return methods;
	}
	public System.String GetPhisicalName()
	{
		string s = this.PhisicalName;
		if ((s != null)&&(!string.IsNullOrEmpty(s)))
		{
			return s;
		}
		else
		{
			return "I" + Static.GetCSharpObjectName(this);
		}
	}
	#endregion
}
}