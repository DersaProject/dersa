using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class Xsd: ICompiledEntity
{
	public Xsd()
	{
	}
	public Xsd(IDersaEntity obj)
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
	#region PhisicalName
	public System.String PhisicalName;
	#endregion
	#region RootReference
	public System.String RootReference;
	#endregion
	#region TargetNamespace
	public System.String TargetNamespace = "http://wibs.dl/DocumentExchange";
	#endregion
	#endregion

	#region Операции
	public void Generate(System.Boolean dialog)
	{
		//if ((dialog)&&(!Static.Ask("Сгенерировать схему " + this.Name))) return;
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		
		string targetNamespace = this.GenerateTargetNamespace();
		string phisicalName = this.GetPhisicalName();
		sb.Append("<?xml version=\"1.0\" encoding=\"UTF-16\"?>\n");
		sb.Append("<xsd:schema xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " + 
				  "xmlns:sch=\"http://DIOS.Biztalk.Integration.Schemas.CommonPropertiesSchema\" " +
				  "xmlns:biz=\"http://schemas.microsoft.com/BizTalk/2003\" " +
				  "xmlns:wibs=\"http://wibs.dl\" " +
				  "targetNamespace=\"" + targetNamespace + 
				  "\" xmlns=\"" + targetNamespace + "\" elementFormDefault=\"qualified\">\n");
		sb.Append("\t<xsd:annotation>\n");
		sb.Append("\t\t<xsd:documentation>Schema name: " + phisicalName + "</xsd:documentation>\n");
		sb.Append("\t\t<xsd:appinfo>\n");
		sb.Append("\t\t\t<biz:schemaInfo root_reference=\"" + this.RootReference + "\"/>\n");
		sb.Append("\t\t\t<biz:imports>\n");
		sb.Append("\t\t\t\t<biz:namespace prefix=\"sch\" uri=\"http://DIOS.Biztalk.Integration.Schemas.CommonPropertiesSchema\" location=\"DIOS.Biztalk.Integration.Schemas.CommonPropertiesSchema\" />\n");
		sb.Append("\t\t\t</biz:imports>\n");
		
		sb.Append("\t\t</xsd:appinfo>\n");
		sb.Append("\t</xsd:annotation>\n");
		
		// Добавим тип guid
		sb.Append("\t<xsd:simpleType name=\"guid\">\n");
		sb.Append("\t\t<xsd:restriction base=\"xsd:string\">\n");
		sb.Append("\t\t\t<xsd:pattern value=\"[0-9A-Fa-f]{8}\\-?[0-9A-Fa-f]{4}\\-?[0-9A-Fa-f]{4}\\-?[0-9A-Fa-f]{4}\\-?[0-9A-Fa-f]{12}\" />\n");
		sb.Append("\t\t</xsd:restriction>\n");
		sb.Append("\t</xsd:simpleType>\n");
		
		sb.Append(this.GenerateXsdAttribute("SchemaName", "xsd:string", "Название схемы"));
		sb.Append(this.GenerateXsdAttribute("ClassName", "xsd:string", "Имя класса"));
		sb.Append(this.GenerateXsdAttribute("ID", "xsd:string", "Уникальный ключ"));
		sb.Append(this.GenerateXsdAttribute("Event", "xsd:string", "Событие, породившее создание документа"));
		
		Entity e = this.GetRootEntity();
		if (e == null) throw new NullReferenceException("В схеме не указан RootReference\nили он не совпадает ни с одним названием Entity");
		sb.Append(GenerateEntity(e, new System.Collections.ArrayList()));
		sb.Append("</xsd:schema>");
		string fileName = "\\\\DRAGO\\XmlScemas\\" + this.Name.Replace(" ", "_") + ".xsd";
		System.IO.StreamWriter sr = new System.IO.StreamWriter(fileName, false, new System.Text.UnicodeEncoding(false, true));
		sr.Write(sb.ToString());
		sr.Close();
		//Static.SaveToFile(fileName, sb.ToString(), System.Text.Encoding.Unicode);
		Static.Information("Сохранено:\n" + fileName);
	}
	public System.String GenerateEntity(Entity entity, System.Collections.IList stack)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		System.Console.WriteLine("Started: " + entity.Name);
		//System.Windows.Forms.Application.DoEvents();
		stack.Add(entity);
		
		System.Collections.IList entities = this.GetEntities();
		System.Collections.IList attrs = entity.GetCSharpAttributes(false);
		
		string entityName = entity.GetSqlName();
		
		System.Collections.Hashtable parentEntities = this.GetParentEntities(entity);
		System.Collections.Hashtable dependentEntities = this.GetDependentEntities(entity);
		
		/*if (entityName != this.RootReference)
		{
			foreach (Relation dependantRelation in dependentEntities.Keys)
			{
				Entity dependentEntity = (Entity)dependentEntities[dependantRelation];
				if (dependantRelation.MakeRef && (entity.Id == dependentEntity.Id || entity.InheritsFrom(dependentEntity)))
				{
					entityName = dependantRelation.MyKey(dependentEntity).ToUpper();
				}
			}
		}*/
		
		foreach (Attribute attr in attrs)
		{
			sb.Append(this.GenerateXsdElement("\t", entityName + "." + attr.GetSqlName(), Static.GetXsdType(attr.Type), attr.Null == "null"));
		}
		string targetNamespace = this.GenerateTargetNamespace();
		
		sb.Append("\t<xsd:element name=\"" + entityName + "\">\n");
		sb.Append("\t\t<xsd:annotation>\n");
		sb.Append("\t\t\t<xsd:appinfo>\n");
		sb.Append("\t\t\t\t<biz:recordInfo rootTypeName=\"" + entityName + "\" />\n");
		
		if (entityName == this.RootReference)
		{
			sb.Append("\t\t\t\t<biz:properties>\n");
			sb.Append("\t\t\t\t\t<biz:property name=\"sch:SchemaName\" xpath=\"/*[local-name()='" + entityName + "' and namespace-uri()='" + targetNamespace + "']/@*[local-name()='SchemaName' and namespace-uri()='" + targetNamespace + "']\" />\n");
			sb.Append("\t\t\t\t\t<biz:property name=\"sch:ClassName\" xpath=\"/*[local-name()='" + entityName + "' and namespace-uri()='" + targetNamespace + "']/@*[local-name()='ClassName' and namespace-uri()='" + targetNamespace + "']\" />\n");
			sb.Append("\t\t\t\t\t<biz:property name=\"sch:ID\" xpath=\"/*[local-name()='" + entityName + "' and namespace-uri()='" + targetNamespace + "']/@*[local-name()='ID' and namespace-uri()='" + targetNamespace + "']\" />\n");
			sb.Append("\t\t\t\t\t<biz:property name=\"sch:Event\" xpath=\"/*[local-name()='" + entityName + "' and namespace-uri()='" + targetNamespace + "']/@*[local-name()='Event' and namespace-uri()='" + targetNamespace + "']\" />\n");
		  	sb.Append("\t\t\t\t</biz:properties>\n");
		}
		
		if (entityName != this.RootReference)
		{
			foreach (Entity localEntity in entities)
			{
				if (localEntity == entity) continue;
				foreach (Relation parentRelation in parentEntities.Keys)
				{
					Entity parentEntity = (Entity)parentEntities[parentRelation];
					if (parentRelation.MakeAggMethod && localEntity.Id == parentEntity.Id) //|| localEntity.InheritsFrom(parentEntity)))
					{
						System.Collections.IList methods = parentRelation.GenerateCollectionAccessMethodForB();
						if (methods.Count == 0)
						{
							methods = parentRelation.GenerateCollectionAccessMethodForA();
						}
						if (methods.Count > 0)
						{
							sb.Append("\t\t\t\t<wibs:howToReceive type=\"method\" action=\"" + ((Method)methods[0]).Name + "\" />\n");
						}
					}
				}
				
				foreach (Relation dependantRelation in dependentEntities.Keys)
				{
					Entity dependentEntity = (Entity)dependentEntities[dependantRelation];
					if (dependantRelation.MakeRef && localEntity.Id == dependentEntity.Id)// || localEntity.InheritsFrom(dependentEntity)))
					{
						string refName = dependantRelation.GetRefName();
						if (refName != "")
						{
							sb.Append("\t\t\t\t<wibs:howToReceive type=\"property\" action=\"" + refName + "\" />\n");
						}
					}
				}
			}
		}
		
		sb.Append("\t\t\t</xsd:appinfo>\n");
		sb.Append("\t\t</xsd:annotation>\n");
		
		sb.Append("\t\t<xsd:complexType>\n");
		sb.Append("\t\t\t<xsd:sequence>\n");
		foreach (Attribute attr in attrs)
		{
			sb.Append("\t\t\t\t<xsd:element minOccurs=\"0\" maxOccurs=\"1\" ref=\"" + entityName + "." + attr.GetSqlName() + "\" />\n");
		}
		
		System.Collections.IList toGenerateEntities = new System.Collections.ArrayList();
		Entity rootEntity = this.GetRootEntity();
		
		foreach (Entity localEntity in entities)
		{
			if (localEntity == entity || localEntity == rootEntity) continue;
			foreach (Relation dependantRelation in dependentEntities.Keys)
			{
				Entity dependentEntity = (Entity)dependentEntities[dependantRelation];
				if (dependantRelation.MakeAggMethod && localEntity.Id == dependentEntity.Id)
				{
					sb.Append("\t\t\t\t<xsd:element minOccurs=\"0\" maxOccurs=\"unbounded\" ref=\"" + localEntity.GetSqlName() + "\" />\n");
					if (!toGenerateEntities.Contains(localEntity) && !stack.Contains(localEntity))
						toGenerateEntities.Add(localEntity);
				}
			}
			foreach (Relation parentRelation in parentEntities.Keys)
			{
				Entity parentEntity = parentEntities[parentRelation] as Entity;
				if (parentEntity == null) continue;
				// InheritFields используется для предотвращения циклических схем
				//if (parentRelation.MakeRef && (localEntity.Id == parentEntity.Id || localEntity.InheritsFrom(parentEntity)))
				if (parentRelation.MakeRef && localEntity.Id == parentEntity.Id)
				{
					string myKeyName = parentEntity.GetSqlName(); //parentRelation.OtherKey(parentEntity).ToUpper();
					sb.Append("\t\t\t\t<xsd:element minOccurs=\"0\" maxOccurs=\"1\" ref=\"" + myKeyName + "\" />\n");
					if (!toGenerateEntities.Contains(localEntity) && !stack.Contains(localEntity))
						toGenerateEntities.Add(localEntity);
				}
			}
		}
		sb.Append("\t\t\t</xsd:sequence>\n");
		
		if (entityName == this.RootReference)
		{
			sb.Append("\t\t\t<xsd:attribute ref=\"SchemaName\" />\n");
			sb.Append("\t\t\t<xsd:attribute ref=\"ClassName\" />\n");
			sb.Append("\t\t\t<xsd:attribute ref=\"ID\" />\n");
			sb.Append("\t\t\t<xsd:attribute ref=\"Event\" />\n");
		}
		
		sb.Append("\t\t</xsd:complexType>\n");
		sb.Append("\t</xsd:element>\n");
		
		foreach (Entity e in toGenerateEntities)
		{
			sb.Append(GenerateEntity(e, stack));
		}
		System.Console.WriteLine("Finished: " + entity.Name);
		//System.Windows.Forms.Application.DoEvents();
		return sb.ToString();
	}
	public string GenerateTargetNamespace()
	{
		return this.TargetNamespace + "/"+ this.GetPhisicalName();
	}
	public System.String GenerateXsdAttribute(System.String name, System.String typeName, System.String description)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.Append("\t<xsd:attribute name=\"" + name + "\" type=\"" + typeName + "\">\n");
		sb.Append("\t\t<xsd:annotation>\n");
		sb.Append("\t\t\t<xsd:documentation>" + description  + "</xsd:documentation>\n");
		sb.Append("\t\t</xsd:annotation>\n");
		sb.Append("\t</xsd:attribute>\n");
		return sb.ToString(); 
	}
	public System.String GenerateXsdElement(System.String prefix, System.String name, System.String typeName, System.Boolean nillable)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.Append(prefix + "<xsd:element name=\"" + name + "\"");
		if (nillable) sb.Append(" nillable=\"true\"");
		sb.Append(">\n");
		
		sb.Append(prefix + "\t<xsd:annotation>\n");
		sb.Append(prefix + "\t\t<xsd:appinfo>\n");
		sb.Append(prefix + "\t\t\t<biz:recordInfo rootTypeName=\"" + name.Replace(".", "_") + "\" />\n");
		sb.Append(prefix + "\t\t</xsd:appinfo>\n");
		sb.Append(prefix + "\t</xsd:annotation>\n");
		
		sb.Append(prefix + "\t<xsd:complexType>\n");
		sb.Append(prefix + "\t\t<xsd:simpleContent>\n");
		sb.Append(prefix + "\t\t\t<xsd:extension base=\"" + typeName + "\" />\n");
		sb.Append(prefix + "\t\t</xsd:simpleContent>\n");
		sb.Append(prefix + "\t</xsd:complexType>\n");
		sb.Append(prefix + "</xsd:element>\n");
		
		return sb.ToString();
	}
	public System.Collections.Hashtable GetDependentEntities(Entity entity)
	{
		System.Collections.Hashtable hash = new System.Collections.Hashtable();
		bool isNull = false;
		System.Collections.IList relations = entity.ARelations;
		for (int i = 0; i < relations.Count; i++)
		{
			ICompiledRelation cr = (ICompiledRelation)relations[i];
			if (cr is Inherit)
			{
				System.Collections.Hashtable dependantHash = GetDependentEntities((Entity)cr.B);
				System.Collections.IEnumerator iEnum = dependantHash.Keys.GetEnumerator();
				while (iEnum.MoveNext())
				{
					Relation r = (Relation)iEnum.Current;
					hash.Add(r, dependantHash[r]);
				}
			}
			else if (cr is Relation)
			{
				Relation rel = (Relation)cr;
				if (rel.GenerateForB(out isNull))
				{
					hash.Add(rel, rel.B);
				}
			}
		}
		relations = entity.BRelations;
		for (int i = 0; i < relations.Count; i++)
		{
			ICompiledRelation cr = (ICompiledRelation)relations[i];
			if (cr is Relation)
			{
				Relation rel = (Relation)cr;
				if (rel.GenerateForA(out isNull))
				{
					hash.Add(rel, rel.A);
				}
			}
		}
		return hash;
	}
	public System.Collections.IList GetEntities()
	{
		Map entities = new Map();
		System.Collections.IList relations = this.ARelations;
		for (int i = 0; i < relations.Count; i++)
		{
			ICompiledRelation rel = (ICompiledRelation)relations[i];
			if (rel is Inherit)
			{
				System.Collections.IList inheritEntities = (rel.B as Xsd).GetEntities();
				foreach(Entity e in inheritEntities)
				{
					entities.Add(e.GetSqlName(), e);
				}
			}
		}
		System.Collections.IList children = this.Children;
		for (int i = 0; i < children.Count; i++)
		{
			Entity obj = children[i] as Entity;
			if (obj == null) continue;
			entities.Add(obj.GetSqlName(), obj);
		}
		return entities;
	}
	public System.Collections.Hashtable GetParentEntities(Entity entity)
	{
		System.Collections.Hashtable hash = new System.Collections.Hashtable();
		bool isNull = false;
		System.Collections.IList relations = entity.ARelations;
		for (int i = 0; i < relations.Count; i++)
		{
			ICompiledRelation cr = (ICompiledRelation)relations[i];
			if (cr is Inherit)
			{
				System.Collections.Hashtable dependantHash = GetParentEntities((Entity)cr.B);
				System.Collections.IEnumerator iEnum = dependantHash.Keys.GetEnumerator();
				while (iEnum.MoveNext())
				{
					Relation r = (Relation)iEnum.Current;
					if (dependantHash[r] is Entity)
					{
						hash.Add(r, dependantHash[r]);
					}
				}
			}
			else if (cr is Relation)
			{
				Relation rel = (Relation)cr;
				if (rel.GenerateForA(out isNull) && rel.B is Entity)
				{
					hash.Add(rel, rel.B);
				}
			}
		}
		relations = entity.BRelations;
		for (int i = 0; i < relations.Count; i++)
		{
			ICompiledRelation cr = (ICompiledRelation)relations[i];
			if (cr is Relation)
			{
				Relation rel = (Relation)cr;
				if (rel.GenerateForB(out isNull) && rel.A is Entity)
				{
					hash.Add(rel, rel.A);
				}
			}
		}
		return hash;
	}
	public System.String GetPhisicalName()
	{
		string s = this.PhisicalName;
		if (s != null && !string.IsNullOrEmpty(s))
		{
			return s;
		}
		else
		{
			return this.Name.Replace(" ", "_");
		}
	}
	public Entity GetRootEntity()
	{
		System.Collections.IList entities = this.GetEntities();
		foreach (Entity e in entities)
		{
			if (e.GetSqlName() == this.RootReference) return e;
		}
		return null;
	}
	#endregion
}
}