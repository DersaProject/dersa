using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;  
using Dersa.Interfaces;

namespace Dersa.Common
{

    public class NameComparer: IComparer
    {
        public int Compare(object x, object y)
        {
            if (!(x is ICompiled) || !(y is ICompiled))
                return 0;
            ICompiled X = x as ICompiled; 
            ICompiled Y = y as ICompiled;
            return string.Compare(X.Name, Y.Name);
        }
    }
    [Serializable]
	public class ChildrenCollection: ICollection, IList, IChildrenCollection
	{
		private ArrayList _list;
		//private ObjectComparer _comparer = new ObjectComparer();
		private bool _autoSorting = false;
		
		public ChildrenCollection(): base()
		{
			_list = new ArrayList();
		}
        public void Sort()
        {

        }

        public void Sort(IComparer _comparer)
		{
            _list.Sort(_comparer);
            /*
			lock(this)
			{
			}*/
        }
        public bool AutoSorting
		{
			get
			{
				return _autoSorting;
			}
			set
			{
				_autoSorting = value;
			}
		}
		public virtual IObject this[int i] 
		{
			get 
			{
				return (IObject)_list[i];
			}
			set
			{
				lock (this.SyncRoot)
				{
					_list[i] = value;
				}
			}
		}
		public virtual int Add(IObject o)
		{
			if (_list.Contains(o)) return _list.IndexOf(o);
			lock (this.SyncRoot)
			{
				_list.Add(o);
				if (_autoSorting)
				{
					Sort();
				}
				return _list.IndexOf(o);
			}
		}
		public void AddRange(ICollection collection)
		{
			lock (this.SyncRoot)
			{
				_list.AddRange(collection);
			}
		}
		public virtual void Insert(int index, IObject o)
		{
			if (_list.Contains(o))
			{
				Remove(o);
			}
			lock (this.SyncRoot)
			{
				_list.Insert(index, o);
			}
		}
		public virtual void Remove(IObject o)
		{
			lock (this.SyncRoot)
			{
				_list.Remove(o);  
			}
		}
		public void RemoveAt(int index) 
		{
			lock (this.SyncRoot)
			{
				_list.RemoveAt(index);
			}
		}
		public virtual int IndexOf(IObject o)
		{
			return _list.IndexOf(o);
		}
		public virtual bool Contains(IObject o)
		{
			return _list.Contains(o);
		}
		public void Clear()
		{
			_list.Clear();
		}
		public int Count
		{
			get
			{
				return _list.Count;
			}
		}
		public void CopyTo(System.Array array, int index) 
		{
			_list.CopyTo(array,index);
		}
		public bool IsSynchronized 
		{
			get
			{
				return _list.IsSynchronized;			
			}
		}
		public object SyncRoot 
		{
			get
			{
				return _list.SyncRoot;
			}
		}
		public System.Collections.IEnumerator GetEnumerator() 
		{
			return _list.GetEnumerator();
		}
		#region IList Members

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (IObject)value;
			}
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			this.Insert(index, (IObject)value);
		}

		void System.Collections.IList.Remove(object value)
		{
			this.Remove((IObject)value);
		}

		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains((IObject)value);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return this.IndexOf((IObject)value);
		}

		int System.Collections.IList.Add(object value)
		{
			return this.Add((IObject)value);
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion

        public IList GetInstance()
		{
			ArrayList list = new ArrayList();
			if ((_list.Count > 0)&&(_list[0] is IStereotypedObject))
			{
				lock (this.SyncRoot)
				{
                    //throw new Exception(_list.Count.ToString());
					for (int i = 0; i < _list.Count; i++)
					{
						list.Add(((IStereotypedObject)_list[i]).GetCompiledInstance());
					}
				}
			}
			return list;
		}
	}
}
