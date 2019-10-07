using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Dersa
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IQueryExecuteService" в коде и файле конфигурации.
    [ServiceContract]
    public interface IQueryExecuteService
    {
        [OperationContract]
        string GetText(string TextId, string token);
        [OperationContract]
        string GetUserToken(string name, string password);
        [OperationContract]
        string GetAttrValue(string attrName, string entityId, string userName = null);
        [OperationContract]
        string SetAttrValue(string attr_name, string entity_id, string attr_value, string token);
    }
}
