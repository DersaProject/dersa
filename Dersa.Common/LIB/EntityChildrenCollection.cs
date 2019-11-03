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
		public new IDersaEntity this[int i] 
		{
			get 
			{
				return (IDersaEntity)base[i];
			}
			set
			{
				base[i] = value;
			}
		}
		public int Add(IDersaEntity e)
		{
			return base.Add(e);
		}
		public void Insert(int index, IDersaEntity e)
		{
			base.Insert(index, e);
		}
		public void Remove(IDersaEntity e)
		{
			base.Remove(e);
		}
		public int IndexOf(IDersaEntity e)
		{
			return base.IndexOf(e);
		}
		public bool Contains(IDersaEntity e)
		{
			return base.Contains(e);
		}
	}
}
