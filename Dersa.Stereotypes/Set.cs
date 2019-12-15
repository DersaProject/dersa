using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Set: StereotypeBaseE, ICompiledEntity
	{
		public Set(){}

		public Set(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Description = "";

		#region Ìועמהû
		#region Value
		public System.String Value(System.String name)
		{
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if ((obj is Const)&&(obj.Name == name))
				{
					return ((Const)obj).Value;
				}
			}
			
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					Set bSet = rel.B as Set;
					if (bSet != null)
					{
						string result = bSet.Value(name);
						if ((result != null)&&(result.Length > 0))
							return result;
					}
				}
			}
			return "";
		}
		#endregion
		#region Has
		public System.Boolean Has(System.String name)
		{
			System.Collections.IList children = this.Children;
			for (int i = 0; i < children.Count; i++)
			{
				ICompiledEntity obj = (ICompiledEntity)children[i];
				if ((obj is Const)&&(obj.Name == name))
				{
					return true;
				}
			}
			
			System.Collections.IList relations = this.ARelations;
			for (int i = 0; i < relations.Count; i++)
			{
				ICompiledRelation rel = (ICompiledRelation)relations[i];
				if (rel is Inherit)
				{
					Set bSet = rel.B as Set;
					if (bSet != null)
					{
						bool result = bSet.Has(name);
						if (result)
							return result;
					}
				}
			}
			return false;
		}
		#endregion
		#endregion
	}
}
