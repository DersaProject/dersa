using System;
using System.Collections;
using Dersa.Interfaces;

namespace Dersa.Common
{
	/// <summary>
	/// Summary description for EntityChildrenCollection.
	/// </summary>
	[Serializable]
	public class EntityChildrenCollection : ChildrenCollection, IList, ICollection
	{
		public EntityChildrenCollection(): base()
		{
		}
		public new IEntity this[int i] 
		{
			get 
			{
				return (IEntity)base[i];
			}
			set
			{
				base[i] = value;
			}
		}
		public int Add(IEntity e)
		{
			return base.Add(e);
		}
		public void Insert(int index, IEntity e)
		{
			base.Insert(index, e);
		}
		public void Remove(IEntity e)
		{
			base.Remove(e);
		}
		public int IndexOf(IEntity e)
		{
			return base.IndexOf(e);
		}
		public bool Contains(IEntity e)
		{
			return base.Contains(e);
		}
	}
}
