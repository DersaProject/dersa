using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using System.Collections;

namespace Dersa.Common
{
	[Serializable()]
	public class CachedObjects
    {
		public CachedObjects(){}


        private static Hashtable cachedEntities = new Hashtable();
        public static Hashtable CachedEntities
        {
            get
            {
                return cachedEntities;
            }
        }

        private static Hashtable cachedCompiledInstances = new Hashtable();
        public static Hashtable CachedCompiledInstances
        {
            get
            {
                return cachedCompiledInstances;
            }
        }

        #region Ìועמהû
        public static void ClearCache()
        {
            cachedEntities = new Hashtable();
            cachedCompiledInstances = new Hashtable();
        }
        #endregion
    }
}
