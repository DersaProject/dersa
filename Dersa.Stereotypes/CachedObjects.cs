using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Collections;

namespace DersaStereotypes
{
	[Serializable()]
	public class CachedObjects : StereotypeBaseE, ICompiledEntity, ICacheProvider
    {
		public CachedObjects(){}

		public CachedObjects(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

        private static Hashtable cachedEntities = new Hashtable();
        public Hashtable CachedEntities
        {
            get
            {
                return cachedEntities;
            }
        }
        public static Hashtable GetCachedEntities()
        {
            return cachedEntities;
        }

        private static Hashtable cachedCompiledInstances = new Hashtable();
        public Hashtable CachedCompiledInstances
        {
            get
            {
                return cachedCompiledInstances;
            }
        }
        public static Hashtable GetCachedCompiledInstances()
        {
            return cachedCompiledInstances;
        }

        #region Ìועמהû
        #endregion
    }
}
