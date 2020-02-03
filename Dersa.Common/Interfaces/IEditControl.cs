using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dersa.Interfaces
{
    public interface IEditControl
    {
        string dataField { get; set; }
        #region Методы
        string GenerateCS();
        string CSTypeName();
        #endregion
    }
}
