using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Dersa.Interfaces
{
    public interface ICacheProvider
    {
        Hashtable CachedEntities { get; }
        Hashtable CachedCompiledInstances { get; }
    }
}
