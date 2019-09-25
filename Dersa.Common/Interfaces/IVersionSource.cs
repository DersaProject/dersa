using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Dersa.Interfaces
{
    public interface IVersionSource
    {
        string GenerateProduction();
        void CheckOut();
    }
}
