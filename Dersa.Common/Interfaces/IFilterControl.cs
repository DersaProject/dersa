using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dersa.Interfaces
{
    public interface IFilterControl
    {
        string dataField { get; set; }
    }

    public interface IFilterControlWithPredicate: IFilterControl
    {
        string predicate { get; set; }
    }
}
