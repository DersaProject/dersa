using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class Relation: ICompiledRelation
{
	public Relation()
	{
	}
	public Relation(IRelation obj)
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
	#region AComment
	public System.String AComment = "";
	#endregion
	#region Aggregate
	public System.String Aggregate = "";
	#endregion
	#region AQty
	public System.String AQty="";
	#endregion
	#region ARole
	public System.String ARole = "";
	#endregion
	#region BComment
	public System.String BComment="";
	#endregion
	#region BQty
	public System.String BQty = "0..1";
	#endregion
	#region BRole
	public System.String BRole = "";
	#endregion
	#region BStringAttributes
	public System.String BStringAttributes = "";
	#endregion
	#region CashRef
	public System.Boolean CashRef = false;
	#endregion
	#region InheritFields
	public System.Boolean InheritFields = false;
	#endregion
	#region MakeAggMethod
	public System.Boolean MakeAggMethod = false;
	#endregion
	#region MakeFK
	public System.Boolean MakeFK = true;
	#endregion
	#region MakeIndex
	public System.Boolean MakeIndex = true;
    #endregion
        public System.Boolean RefIsInterface = false;
    #region MakeRef
    public System.Boolean MakeRef = true;
	#endregion
	#region RefName
	public System.String RefName = "";
	#endregion
	#region Set
	public System.String Set = "";
	#endregion
	#region SetRef
	public System.String SetRef = "";
	#endregion
	#endregion

	#region Операции
	public System.Collections.IList GenerateCollectionAccessMethodForA()
	{
		System.Collections.ArrayList methods = new System.Collections.ArrayList();
		if (!this.MakeAggMethod) return methods;
		bool isNull = false;
		if (GenerateForB(out isNull))
		{
			Entity A = (Entity)this.A;
			Entity B = (Entity)this.B;
			string childName = Static.GetCSharpObjectName(B);
			string pkName = A.GetPKName(); 	
			if (pkName == "") return methods;
		
			string role = this.ARole;
			if (role == "") role = pkName;
			
		
			string methodName = "Get" + childName;
			if (role != pkName)
			{
				methodName += Static.GetCSharpName(role);
			}
			if (A.Id == B.Id)
			{
				methodName += "Children";
			}
			else
			{
				methodName += "s";
			}
			Method method = new Method(null);
			method.Name = methodName;
			method.AccessModifier = "public";
			method.ReturnType = "IObjectCollection";
			method.Parameters = "";
			method.Text = "\t\t\treturn " + methodName + "(new ParameterCollection());";
			method.Interface = true;
			method.Transactional = false;
			methods.Add(method);
			
			string childSqlName = B.GetSqlName();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			//sb.Append("\t\t\tif (!Params.Contains(\"" + role +  "\"))\n");
			//sb.Append("\t\t\t{\n");
			//sb.Append("\t\t\t\tParams.Add(new Parameter(\"" + role +  "\", id));\n");
			//sb.Append("\t\t\t}\n");
			//sb.Append("\t\t\treturn Entity.Manager.GetEntity(\"" + childSqlName + "\").List(Params, true);");
			
			sb.Append("\t\t\tIParameter keyParam = new Parameter(\"" + role +  "\", id);\n");
			sb.Append("\t\t\tParams.Add(keyParam);\n");
			sb.Append("\t\t\tIObjectCollection resultCollection = Entity.Manager.GetEntity(\"" + childSqlName + "\").List(Params, true);\n");
			sb.Append("\t\t\tParams.Remove(keyParam);\n");
			sb.Append("\t\t\treturn resultCollection;\n");
		
			method = new Method(null);
			method.Name = methodName;
			method.AccessModifier = "public";
			method.ReturnType = "IObjectCollection";
			method.Parameters = "IParameterCollection Params";
			method.Text = sb.ToString();
			method.Interface = true;
			method.Transactional = false;
			methods.Add(method);
		}
		return methods;
	}
	public System.Collections.IList GenerateCollectionAccessMethodForB()
	{
		System.Collections.ArrayList methods = new System.Collections.ArrayList();
		if (!this.MakeAggMethod) return methods;
		bool isNull = false;
		if (GenerateForA(out isNull))
		{
			Entity A = (Entity)this.A;
			Entity B = (Entity)this.B;
			
			string childName = Static.GetCSharpObjectName(A);
			string pkName = B.GetPKName(); 	
			//Console.WriteLine(A.Name + " -> " + B.Name + " " + childName + " " + pkName);
			if (pkName == "") return methods;
			
			string role = this.BRole;
			if (role == "") role = pkName;
			
			string methodName = "Get" + childName;
			if (role != pkName)
			{
				methodName += Static.GetCSharpName(role);
			}
			if (A.Id == B.Id)
			{
				methodName += "Children";
			}
			else
			{
				methodName += "s";
			}
			Method method = new Method(null);
			method.Name = methodName;
			method.AccessModifier = "public";
			method.ReturnType = "IObjectCollection";
			method.Parameters = "";
			method.Text = "\t\t\treturn " + methodName + "(new ParameterCollection());";
			method.Interface = true;
			method.Transactional = false;
			methods.Add(method);
		
			string childSqlName = A.GetSqlName();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();	
		
			sb.Append("\t\t\tIParameter keyParam = new Parameter(\"" + role +  "\", id);\n");
			sb.Append("\t\t\tParams.Add(keyParam);\n");
			sb.Append("\t\t\tIObjectCollection resultCollection = Entity.Manager.GetEntity(\"" + childSqlName + "\").List(Params, true);\n");
			sb.Append("\t\t\tParams.Remove(keyParam);\n");
			sb.Append("\t\t\treturn resultCollection;\n");
			
			method = new Method(null);
			method.Name = methodName;
			method.AccessModifier = "public";
			method.ReturnType = "IObjectCollection";
			method.Parameters = "IParameterCollection Params";
			method.Text = sb.ToString();
			method.Interface = true;
			method.Transactional = false;
			methods.Add(method);
		}
		return methods;
	}
	public System.Boolean GenerateForA(out bool isNull)
	{
		string bMultiplicity = this.BQty == null? "": BQty;
		string aMultiplicity = this.AQty == null? "": AQty;
		isNull = bMultiplicity == "0..1" || aMultiplicity == "0..*";
		if ((aMultiplicity == "0..*")||(aMultiplicity == "1..*"))
		{
			return true;
		}
		else if (((bMultiplicity == "1")||(bMultiplicity == "0..1"))&&((aMultiplicity == "")||(aMultiplicity == "1")))
		{
			return true;
		}
		return false;
	}
	public System.Boolean GenerateForB(out bool isNull)
	{
		string bMultiplicity = this.BQty;
		string aMultiplicity = this.AQty;
		
		isNull = aMultiplicity == "0..1" || bMultiplicity == "0..*";
		if ((bMultiplicity == "0..*")||(bMultiplicity == "1..*"))
		{
			return true;
		}
		else if (((aMultiplicity == "1")||(aMultiplicity == "0..1"))&&((bMultiplicity == "")||(bMultiplicity == "1")))
		{
			return true;
		}
		return false;
	}
	public System.String GenerateInterfaceRefObjsForA()
	{
		if (!this.MakeRef) return "";
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		bool isNull = false;
		if (GenerateForA(out isNull))
		{
			Entity B = this.B as Entity;
			if (B == null) return "";
			string pkName = B.GetPKName(); 	
			if (pkName == "") return "";
			
			string phisicalName = B.GetSqlName(); 	
			string refFieldName = this.BRole;
			if (refFieldName == "")
			{
				refFieldName = pkName; 
			}
			string interf = "";
			if ((((Package)A.Parent).Namespace == ((Package)B.Parent).Namespace)&&(((Package)A.Parent).Namespace.Trim().Length > 0))
			{
				interf = ((Package)B.Parent).Namespace + ".I" + Static.GetCSharpObjectName(B);
			}
			else
			{
				interf = "IObject";		
			}
			bool notNull = false;
			if ((!isNull)&&(interf != "IObject"))
			{
				notNull = true;
			}
			string locolizedName = B.Name;
			if (this.BComment.Length > 0)
				locolizedName = this.BComment;
			sb.Append("\t\t[ObjectPropertyAttribute(\"" + locolizedName + "\", " + notNull.ToString().ToLower() + ", false)]\n");
			sb.Append("\t\t" + interf + " "  +  refFieldName + "Object" + "{get;set;}\n");
		}
		return sb.ToString();
	}
	public System.String GenerateInterfaceRefObjsForB()
	{
		if (!this.MakeRef) return "";
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		bool isNull = false;
		if (GenerateForB(out isNull))
		{
			Entity A = this.A as Entity;
			if (A == null) return "";
			string pkName = A.GetPKName(); 	
			if (pkName == "") return "";
			
			string phisicalName = A.GetSqlName(); 	
			string refFieldName = this.ARole;
			if (refFieldName == "")
			{
				refFieldName = pkName; 
			}
			string interf = "";
			if ((((Package)A.Parent).Namespace == ((Package)B.Parent).Namespace)&&(((Package)A.Parent).Namespace.Trim().Length > 0))
			{
				interf = ((Package)A.Parent).Namespace + ".I" + Static.GetCSharpObjectName(A);
			}
			else
			{
				interf = "IObject";
			}
				bool notNull = false;
			if ((!isNull)&&(interf != "IObject"))
			{
				notNull = true;
			}
			string locolizedName = A.Name;
			if (this.AComment.Length > 0)
				locolizedName = this.AComment;
			sb.Append("\t\t[ObjectPropertyAttribute(\"" + locolizedName + "\", " + notNull.ToString().ToLower() + ", false)]\n");
			sb.Append("\t\t" + interf + " "  +  refFieldName + "Object" + "{get;set;}\n");
		} 
		return sb.ToString();
	}
	public System.String GenerateRefObjsForA()
	{
		if (!this.MakeRef) return "";
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		bool isNull = false;
		if (GenerateForA(out isNull))
		{
			Entity B = this.B as Entity;
			if (B == null) return "";
		
			Attribute pk = B.GetPrimaryKey();
			string pkName = B.GetPKName();
			if (pkName == "") return "";
		
			string phisicalName = B.GetSqlName();
			string refFieldName = this.BRole;
			
			if (refFieldName == "")
			{
				refFieldName = pkName; 
			}
                string interf = "";
                string interf_prefix = "";
                if (this.RefIsInterface) interf_prefix = "I";
                if ((((Package)A.Parent).Namespace == ((Package)B.Parent).Namespace) && (((Package)A.Parent).Namespace.Trim().Length > 0))
                {
                    interf = ((Package)B.Parent).Namespace + "." + interf_prefix + Static.GetCSharpObjectName(B);
                }
                else
                {
                    interf = interf_prefix + "Object";
                }
                sb.Append("\t\t#region "  +  refFieldName + "Object" + "\n");
			bool notNull = false;
			if ((!isNull)&&(interf != "IObject"))
			{
				notNull = true;
			}
			string localizedName = B.Name;
			if (this.BComment.Length > 0)
				localizedName = this.BComment;
				
			if (this.CashRef)
			{
				sb.Append("\t\tprivate " + interf + " __"  +  refFieldName + "Object;" + "\n");
			}
			sb.Append("\t\t[ObjectPropertyAttribute(\"" + localizedName + "\", " + notNull.ToString().ToLower() + ", false)]\n");
			sb.Append("\t\t[ClassKeyName(\"" + phisicalName + "\", \"" + pkName + "\")]\n");
			sb.Append("\t\tpublic " + interf + " "  +  refFieldName + "Object" + "\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t\tget\n");
			sb.Append("\t\t\t{\n");
			sb.Append("\t\t\t\tif (this." + refFieldName + ".IsNull) return null;\n");
			if (this.CashRef)
			{
				sb.Append("\t\t\t\tif (this.__"  +  refFieldName + "Object == null)" + "\n");
				sb.Append("\t\t\t\t{" + "\n");
				sb.Append("\t\t\t\t\tIDersaEntity refEntity = Manager.GetEntity(\"" + phisicalName + "\");" + "\n");
				sb.Append("\t\t\t\t\tthis.__"  +  refFieldName + "Object = (" + interf + ")refEntity.GetObject(this." + refFieldName + ");" + "\n");
				sb.Append("\t\t\t\t}" + "\n");
				sb.Append("\t\t\t\telse" + "\n");
				sb.Append("\t\t\t\t{" + "\n");
				sb.Append("\t\t\t\t\ttry" + "\n");
				sb.Append("\t\t\t\t\t{" + "\n");
				sb.Append("\t\t\t\t\t\tstring test = this.__"  +  refFieldName + "Object.ToString();" + "\n");
				sb.Append("\t\t\t\t\t}" + "\n");
				sb.Append("\t\t\t\t\tcatch" + "\n");
				sb.Append("\t\t\t\t\t{" + "\n");
				sb.Append("\t\t\t\t\t\tIDersaEntity refEntity = Manager.GetEntity(\"" + phisicalName + "\");" + "\n");
				sb.Append("\t\t\t\t\t\tthis.__"  +  refFieldName + "Object = (" + interf + ")refEntity.GetObject(this." + refFieldName + ");" + "\n");
				sb.Append("\t\t\t\t\t}" + "\n");
				sb.Append("\t\t\t\t}" + "\n");
				sb.Append("\t\t\t\treturn this.__"  +  refFieldName + "Object;" + "\n");
			}
			else
			{
				sb.Append("\t\t\t\treturn (" + interf +")GetObject(\"" + phisicalName + "\", this." + refFieldName + ");\n");
			}
			sb.Append("\t\t\t}\n");
			
			string setRef = this.SetRef;
			if (setRef.Length > 0)
			{
				sb.Append("\t\t\tset\n\t\t\t{\n" + setRef + "\n\t\t\t}\n");
			}
			else 
			{
				sb.Append("\t\t\tset\n");
				sb.Append("\t\t\t{\n");
				sb.Append("\t\t\t\tif (value != null)\n");
				if (interf == "IObject")
				{
					sb.Append("\t\t\t\t\tthis." + refFieldName + " = (" + pk.GetCSharpType() + ")value[\"" + pkName + "\"];\n");
				}
				else
				{
					sb.Append("\t\t\t\t\t" + refFieldName + " = value." + pkName + ";\n");
				}
				sb.Append("\t\t\t\telse\n"); 
				sb.Append("\t\t\t\t\tthis." + refFieldName + " = System.DBNull.Value;\n");
				if (this.CashRef)
				{
					sb.Append("\t\t\t\tthis.__"  +  refFieldName + "Object = value;\n"); 
				}
				sb.Append("\t\t\t}\n");
			}
			sb.Append("\t\t}\n");
			sb.Append("\t\t#endregion\n");
		}
		return sb.ToString();
	}
	public System.String GenerateRefObjsForB()
	{
		if (!this.MakeRef) return "";
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		bool isNull = false;
		if (GenerateForB(out isNull))
		{
			Entity A = this.A as Entity;
			if (A == null) return "";
			
			Attribute pk = A.GetPrimaryKey();
			string pkName = A.GetPKName(); 
			if (pkName == "") return "";
				
			string phisicalName = A.GetSqlName(); 	
			string refFieldName = this.ARole;
			if (refFieldName == "")
			{
				refFieldName = pkName; 
			}
			string interf = "";
			if ((((Package)A.Parent).Namespace == ((Package)B.Parent).Namespace)&&(((Package)A.Parent).Namespace.Trim().Length > 0))
			{
				interf = ((Package)A.Parent).Namespace + ".I" + Static.GetCSharpObjectName(A);
			}
			else
			{
				interf = "IObject";
			}
			sb.Append("\t\t#region "  +  refFieldName + "Object" + "\n");
			bool notNull = false;
			if ((!isNull)&&(interf != "IObject"))
			{
				notNull = true;
			}
			string localizedName = A.Name;
			if (this.AComment.Length > 0)
				localizedName = this.AComment;
			if (this.CashRef)
			{
				sb.Append("\t\tprivate " + interf + " __"  +  refFieldName + "Object;" + "\n");
			}
			sb.Append("\t\t[ObjectPropertyAttribute(\"" + localizedName + "\", " + notNull.ToString().ToLower() + ", false)]\n");
			sb.Append("\t\t[ClassKeyName(\"" + phisicalName + "\", \"" + pkName + "\")]\n");
			sb.Append("\t\tpublic " + interf + " "  +  refFieldName + "Object" + "\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t\tget\n");
			sb.Append("\t\t\t{\n");
			sb.Append("\t\t\t\tif (this." + refFieldName + ".IsNull) return null;\n");
			if (this.CashRef)
			{
				sb.Append("\t\t\t\tif (this.__"  +  refFieldName + "Object == null)" + "\n");
				sb.Append("\t\t\t\t{" + "\n");
				sb.Append("\t\t\t\t\tIDersaEntity refEntity = Manager.GetEntity(\"" + phisicalName + "\");" + "\n");
				sb.Append("\t\t\t\t\tthis.__"  +  refFieldName + "Object = (" + interf + ")refEntity.GetObject(this." + refFieldName + ");" + "\n");
				sb.Append("\t\t\t\t}" + "\n");
				sb.Append("\t\t\t\telse" + "\n");
				sb.Append("\t\t\t\t{" + "\n");
				sb.Append("\t\t\t\t\ttry" + "\n");
				sb.Append("\t\t\t\t\t{" + "\n");
				sb.Append("\t\t\t\t\t\tstring test = this.__"  +  refFieldName + "Object.ToString();" + "\n");
				sb.Append("\t\t\t\t\t}" + "\n");
				sb.Append("\t\t\t\t\tcatch" + "\n");
				sb.Append("\t\t\t\t\t{" + "\n");
				sb.Append("\t\t\t\t\t\tIDersaEntity refEntity = Manager.GetEntity(\"" + phisicalName + "\");" + "\n");
				sb.Append("\t\t\t\t\t\tthis.__"  +  refFieldName + "Object = (" + interf + ")refEntity.GetObject(this." + refFieldName + ");" + "\n");
				sb.Append("\t\t\t\t\t}" + "\n");
				sb.Append("\t\t\t\t}" + "\n");
				sb.Append("\t\t\t\treturn this.__"  +  refFieldName + "Object;" + "\n");
			}
			else
			{
				sb.Append("\t\t\t\treturn (" + interf +")GetObject(\"" + phisicalName + "\", this." + refFieldName + ");\n");
			}
			sb.Append("\t\t\t}\n");
			
			string setRef = this.SetRef;
			if (setRef.Length > 0)
			{
				sb.Append("\t\t\tset\n\t\t\t{\n" + setRef + "\n\t\t\t}\n");
			}
			else 
			{
				sb.Append("\t\t\tset\n");
				sb.Append("\t\t\t{\n");
				sb.Append("\t\t\t\tif (value != null)\n");
				if (interf == "IObject")
				{
					sb.Append("\t\t\t\t\tthis." + refFieldName + " = (" + pk.GetCSharpType() + ")value[\"" + pkName + "\"];\n");
				}
				else
				{
					sb.Append("\t\t\t\t\t" + refFieldName + " = value." + pkName + ";\n");
				}
				sb.Append("\t\t\t\telse\n"); 
				sb.Append("\t\t\t\t\tthis." + refFieldName + " = System.DBNull.Value;\n");
				if (this.CashRef)
				{
					sb.Append("\t\t\t\tthis.__"  +  refFieldName + "Object = value;\n"); 
				}
				sb.Append("\t\t\t}\n");
			}
			sb.Append("\t\t}\n");
			sb.Append("\t\t#endregion\n");
		}
		return sb.ToString();
	}
	public System.Collections.IList GetAttributesForA()
	{
        //throw new Exception(this.B.Name);
        System.Collections.ArrayList attrs = new System.Collections.ArrayList();
		bool isNull = false;
		if (GenerateForA(out isNull))
		{
			if (this.B is Entity)
			{
				Entity B = (Entity)this.B;
				System.Collections.IList bAttrs = B.GetPKAttributes();
				string bRole = this.BRole;
				for (int i = 0; i < bAttrs.Count; i++)
				{
					Attribute attr = (Attribute)bAttrs[i];
					if ((bRole != null)&&(bRole != ""))
					{
						attr.PhisicalName = bRole;
					}
					if ((this.BComment != null)&&(this.BComment != ""))
					{
						attr.Name = this.BComment;
					}
					else if ((attr.PhisicalName != null)&&(attr.PhisicalName != ""))
					{
						attr.Name = B.Name.ToLower();
					}
					if (isNull)
					{
						attr.Null = "null";
					}
					else
					{
						attr.Null = "not null";
					}
					attr.Description = "FOREIGN KEY to " + B.GetSqlName() + " (" + B.Name + ')';
					attr.ReadOnly = false;
					//string SetCode = this.Set;
					string SetCode = this.GetSetCode();
					attr.Set = SetCode;
					if ((SetCode == null || SetCode == "") && this.CashRef)
					{
						string refFieldName = this.BRole;
						if (refFieldName == "")
						{
							Attribute pk = B.GetPrimaryKey();
							refFieldName = B.GetPKName();
						}
						attr.Set = 
							"\t\t\t\tthis._" + attr.PhisicalName + " = value;\n" +
							"\t\t\t\tthis.__" + refFieldName + "Object = null;";
					}
					attrs.Add(attr);
				}
			}
			if (this.B is Type)
			{
				Type T = (Type)this.B;
				System.Collections.IList bAttrs = T.GetPKAttributes();
				Attribute attr = (Attribute)bAttrs[0];	
				attr.Name = T.Name.ToLower();
				attr.Description = "FOREIGN KEY to " + T.GetSqlName() + " (" + T.Name + ')';
				if ((this.BComment != null)&&(this.BComment != ""))
				{
					attr.Name = this.BComment;
				}
				if ((this.BRole == null)||(this.BRole == ""))
				{
					attr.PhisicalName = T.GetSqlName().ToLower();
				}
				else
				{
					attr.PhisicalName = this.BRole;
				}
				string csTypeName = Static.GetCSharpObjectName(T);
				string csType = Static.GetCSharpNativeType(attr.Type);
				string csPropertyName = Static.GetCSharpName(attr.GetSqlName());
				if (isNull)
				{
					attr.Null = "null";
				}
				else
				{
					attr.Null = "not null";
				}
				if (T.GenerateTable)
				{
					attr.Set = this.Set;
				}
				else
				{
					attr.Get = "\t\t\t\treturn (" + csType + ")this." + csPropertyName + ";";
					attr.Set = "\t\t\t\tthis." + csPropertyName + " = (" + csTypeName + ")value.Value;";
				}
				attr.ReadOnly = false;
				attrs.Add(attr);
			}
			if (this.B is StateEngine)
			{
				StateEngine S = (StateEngine)this.B;
				System.Collections.IList bAttrs = S.GetPKAttributes();
				Attribute attr = (Attribute)bAttrs[0];	
				attr.Name = S.Name.ToLower();
				attr.Description = "FOREIGN KEY to " + S.GetSqlName() + " (" + S.Name + ')';
				if ((this.BComment != null)&&(this.BComment != ""))
				{
					attr.Name = this.BComment;
				}
				if ((this.BRole == null)||(this.BRole == ""))
				{
					attr.PhisicalName = S.GetSqlName().ToLower();
				}
				else
				{
					attr.PhisicalName = this.BRole;
				}
				string csTypeName = Static.GetCSharpObjectName(S);
				string csType = Static.GetCSharpNativeType(attr.Type);
				string csPropertyName = Static.GetCSharpName(attr.GetSqlName());
				if (isNull)
				{
					attr.Null = "null";
				}
				else
				{
					attr.Null = "not null";
				}
				if (S.GenerateTable)
				{
					attr.Set = this.Set;
				}
				else
				{
					attr.Get = "\t\t\t\treturn (" + csType + ")this." + csPropertyName + ";";
					attr.Set = "\t\t\t\tthis." + csPropertyName + " = (" + csTypeName + ")value.Value;";
				}
				attr.ReadOnly = false;
				attrs.Add(attr);
			}
		}
		return attrs;
	}
	public System.Collections.IList GetAttributesForB()
	{
		System.Collections.ArrayList attrs = new System.Collections.ArrayList();
		bool isNull = false;
		if (GenerateForB(out isNull))
		{
			if (this.A is Entity)
			{
				Entity A = (Entity)this.A;
				System.Collections.IList aAttrs = A.GetPKAttributes();
				string aRole = this.ARole;
				for (int i = 0; i < aAttrs.Count; i++)
				{
					Attribute attr = (Attribute)aAttrs[i];
					if ((aRole != null)&&(aRole != ""))
					{
						attr.PhisicalName = aRole;
					}
					if ((this.AComment != null)&&(this.AComment != ""))
					{
						attr.Name = this.AComment;
					}
					else if ((attr.PhisicalName != null)&&(attr.PhisicalName != ""))
					{
						attr.Name = A.Name.ToLower();
					}
					if (isNull)
					{
						attr.Null = "null";
					}
					else
					{
						attr.Null = "not null";
					}
					attr.Description = "FOREIGN KEY to " + A.GetSqlName() + " (" + A.Name + ')';
					attr.ReadOnly = false;
					attr.Set = this.Set;
					if ((this.Set == null || this.Set == "") && this.CashRef)
					{
						string refFieldName = this.ARole;
						if (refFieldName == "")
						{
							Attribute pk = A.GetPrimaryKey();
							refFieldName = A.GetPKName();
						}
						attr.Set = 
							"\t\t\t\tthis._" + attr.PhisicalName + " = value;\n" +
							"\t\t\t\tthis.__" + refFieldName + "Object = null;";
					}
					attrs.Add(attr);
				}
			}
			if (this.A is Type)
			{
				Type T = (Type)this.A;
				System.Collections.IList bAttrs = T.GetPKAttributes();
				Attribute attr = (Attribute)bAttrs[0];	
				attr.Name = T.Name.ToLower();
				attr.Description = "FOREIGN KEY to " + T.GetSqlName() + " (" + T.Name + ')';
				if ((this.AComment == null)&&(this.AComment == ""))
				{
					attr.Name = this.AComment;
				}
				if ((this.ARole == null)||(this.ARole == ""))
				{
					attr.PhisicalName = T.GetSqlName().ToLower();
				}
				else
				{
					attr.PhisicalName = this.ARole;
				}
				string csTypeName = Static.GetCSharpObjectName(T);
				string csType = Static.GetCSharpNativeType(attr.Type);
				string csPropertyName = Static.GetCSharpName(attr.GetSqlName());
				if (isNull)
				{
					attr.Null = "null";
				}
				else
				{
					attr.Null = "not null";
				}
				if (T.GenerateTable)
				{
					attr.Set = this.Set;
				}
				else
				{
					attr.Get = "\t\t\t\treturn (" + csType + ")this." + csPropertyName + ";";
					attr.Set = "\t\t\t\tthis." + csPropertyName + " = (" + csTypeName + ")value.Value;";
				}
				attr.ReadOnly = false;
				attrs.Add(attr);
			}
		}
		return attrs;
	}
	public System.String GetDescriptionForA()
	{
		string refFieldName = "";
		bool isNull = false;
		if (this.GenerateForA(out isNull))
		{
			refFieldName = this.BRole;
			if (refFieldName == "")
			{
				refFieldName = ((Entity)this.B).Name.ToLower();
			}
		}
		if (refFieldName != "")
		{
			return refFieldName;
		}
		return GetNameForA();
	}
	public System.String GetNameForA()
	{
		string refFieldName = "";
		bool isNull = false;
		if (this.GenerateForA(out isNull))
		{
			refFieldName = this.BRole;
			if (refFieldName == "")
			{
				refFieldName = ((Entity)this.B).GetPKName();
			}
		}
		if (refFieldName != "")
		{
			return refFieldName;
		}
		return "";
	}
	public System.String GetRefName()
	{
		if (this.RefName != null && this.RefName != "") return this.RefName;
		string refFieldName = "";
		bool isNull = false;
		if (this.GenerateForA(out isNull))
		{
			refFieldName = this.BRole;
			if (refFieldName == "")
			{
				refFieldName = ((Entity)this.B).GetPKName();
			}
		}
		else if (this.GenerateForB(out isNull))
		{
			refFieldName = this.ARole;
			if (refFieldName == "")
			{
				refFieldName = ((Entity)this.A).GetPKName();
			}
		}
		if (refFieldName != "")
		{
			return refFieldName + "Object";
		}
		return "";
	}
	public System.String GetSetCode()
	{
		if(this.Set != null && this.Set != string.Empty)
			return this.Set;
		if(this.BStringAttributes != null && this.BStringAttributes != string.Empty)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			Entity BEntity = this.B as Entity;
			if(BEntity != null)
			{
				sb.Append("\t\t\t\t_");
				string MyAttrName = "";
				if ((this.BRole != null)&&(this.BRole != ""))
				{
					MyAttrName = this.BRole;
				}
				else
					MyAttrName = BEntity.GetSqlName().ToLower();
				sb.Append(MyAttrName);
				sb.Append(" = value;");
				string[] AttrNames = this.BStringAttributes.Split(',');
				for(int n=0; n<AttrNames.Length; n++)
				{
					string AttrName = MyAttrName + "_" + AttrNames[n].Trim();
					sb.Append("\n\t\t\t\t_");
					sb.Append(AttrName);
					sb.Append(" = new SqlString();");
				}
			}
			return sb.ToString();
		}
		return string.Empty;
	}
	public System.String MyKey(ICompiledEntity obj)
	{
		string s = "";
		if (obj is Entity)
		{
			if (this.A is Entity && (this.A.Id == obj.Id || ((Entity)obj).InheritsFrom((Entity)A)))
			{
				if ((BRole != null)&&(BRole != ""))
					s = BRole;
				else 
					s = ((Entity)A).GetPK("", true, false);
			}
			else if (this.B is Entity && (B.Id == obj.Id || ((Entity)obj).InheritsFrom((Entity)B)))
			{
				if ((ARole != null)&&(ARole != ""))
					s = ARole;
				else
					s = ((Entity)B).GetPK("", true, false);	
			}
		}
		return s;
	}
	public ICompiledEntity Other(ICompiledEntity obj)
	{
		if (A.Id == obj.Id || (obj is Entity && A is Entity && ((Entity)obj).InheritsFrom((Entity)A))) return B;
		else if (B.Id == obj.Id || (obj is Entity && B is Entity && ((Entity)obj).InheritsFrom((Entity)B))) return A;
		return null;
	}
	public System.String OtherKey(ICompiledEntity obj)
	{
		string s = "";
		if (obj is Entity)
		{
			if (this.A is Entity && (this.A.Id == obj.Id || ((Entity)obj).InheritsFrom((Entity)A)))
			{
				if ((ARole != null)&&(ARole == ""))
					s = ARole;
				else
					s = ((Entity)B).GetPK("", true, false);	
			}
			else if (this.B is Entity && (B.Id == obj.Id || ((Entity)obj).InheritsFrom((Entity)B)))
			{
				if ((BRole != null)&&(BRole != ""))
					s = BRole;
				else 
					s = ((Entity)A).GetPK("", true, false);
			}
		}
		return s;
	}
	public void TestSetCode()
	{
		Console.WriteLine(this.GetSetCode());
	}
	#endregion
}
}