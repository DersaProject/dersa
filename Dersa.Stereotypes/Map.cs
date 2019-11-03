using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class Map: System.Collections.IList
{
	public Map()
	{
	}
	#region Наследуемые свойства
	#endregion
	#region Наследуемые методы
	#endregion

	#region Атрибуты
	#region _hashList
	public System.Collections.Hashtable _hashList = new System.Collections.Hashtable();
	#endregion
	#region _innerList
	public System.Collections.ArrayList _innerList = new System.Collections.ArrayList();
	#endregion
	#endregion

	#region Операции
	public System.Int32 Add(object key, object value)
	{
		RemoveDuplicateKey(key);
		_hashList.Add(key, value);
		return _innerList.Add(value);
	}
	public Map Clone()
	{
		//this._hashList.AddRange(range._hashList);
		Map newMap = new Map();
		newMap._innerList.AddRange(this._innerList);
		foreach (string key in this._hashList.Keys)
		{
			newMap._hashList.Add(key, this._hashList[key]);
		}
		return newMap;
	}
	public System.Int32 Count
	{
		get
		{
			return _innerList.Count;
		}
	}
	public object KeyAt(int index)
	{
		object o = _innerList[index];
		foreach (object key in _hashList.Keys)
		{
			object objInHash = _hashList[key];
			if (objInHash == null) throw new Exception("Объект пустой " + key.ToString());
			if (objInHash.Equals(o))
			{
				return key;
			}
		}
		throw new Exception("Не найден ключ объекта " + o.ToString());
	}
	private void RemoveDuplicate(object value)
	{
		ICompiledEntity newEntity = value as ICompiledEntity;
		if (newEntity == null) throw new Exception("В коллекции могут содержаться только объекты ICompiledEntity");
		RemoveDuplicateKey(newEntity.Name);
	}
	private void RemoveDuplicateKey(object key)
	{
		object entity = _hashList[key];
		if (entity != null) 
		{
			_hashList.Remove(key);
			(this as System.Collections.IList).Remove(entity);
		}
	}
	void System.Collections.ICollection.CopyTo(Array array, int index)
	{
		throw new Exception("Хрен");
	}
	System.Boolean System.Collections.ICollection.IsSynchronized
	{
		get
		{
			return _innerList.IsSynchronized;
		}
	}
	object System.Collections.ICollection.SyncRoot
	{
		get
		{
			return _innerList.SyncRoot;
		}
	}
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return _innerList.GetEnumerator();
	}
	System.Int32 System.Collections.IList.Add(object value)
	{
		ICompiledEntity newEntity = value as ICompiledEntity;
		if (newEntity == null) throw new Exception("В коллекции могут содержаться только объекты ICompiledEntity");
		return Add(newEntity.Name, value);
	}
	void System.Collections.IList.Clear()
	{
		_innerList.Clear();
		_hashList.Clear();
	}
	System.Boolean System.Collections.IList.Contains(object value)
	{
		return _innerList.Contains(value);
	}
	System.Int32 System.Collections.IList.IndexOf(object value)
	{
		return _innerList.IndexOf(value);
	}
	void System.Collections.IList.Insert(int index, object value)
	{
		ICompiledEntity newEntity = value as ICompiledEntity;
		RemoveDuplicate(newEntity);
		_hashList.Add(newEntity.Name, newEntity);
		_innerList.Insert(index, value);
	}
	System.Boolean System.Collections.IList.IsFixedSize
	{
		get
		{
			return false;
		}
	}
	System.Boolean System.Collections.IList.IsReadOnly
	{
		get
		{
			return false;
		}
	}
	void System.Collections.IList.Remove(object value)
	{
		RemoveDuplicate(value);
		_innerList.Remove(value);
	}
	void System.Collections.IList.RemoveAt(int index)
	{
		ICompiledEntity entity = (this as System.Collections.IList)[index] as ICompiledEntity;
		RemoveDuplicate(entity);
		_innerList.RemoveAt(index);
	}
	public object this[int index]
	{
		get
		{
			return _innerList[index];
		}
		set
		{
			ICompiledEntity newEntity = value as ICompiledEntity;
			RemoveDuplicate(newEntity);
			_hashList.Add(newEntity.Name, newEntity);
			_innerList[index] = newEntity;
		}
	}
	#endregion
}
}