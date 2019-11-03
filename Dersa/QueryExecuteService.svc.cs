using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using DIOS.Common;
using Dersa.Models;
using Dersa.Common;
using DIOS.WCF.Core;

namespace Dersa
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "QueryExecuteService" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы QueryExecuteService.svc или QueryExecuteService.svc.cs в обозревателе решений и начните отладку.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class QueryExecuteService : IQueryExecuteService
    {
        public string GetText(string TextId, string token)
        {
            string userName = WcfCoreUtil.VerifyUser(token);
            //string userName = Cryptor.Decrypt(token, "DERSA");
            return QueryControllerAdapter.GetString(TextId, false, userName);//._query;
        }
        public string GetUserToken(string name, string password)
        {
            return GetToken(name);
        }
        public static string GetToken(string name)
        {
            return WcfCoreUtil.GetToken(name, "", false);
            //return Cryptor.Encrypt(name, "DERSA");
        }
        public string GetAttrValue(string attrName, string entityId, string userToken = null)
        {
            try
            {
                string userName = WcfCoreUtil.VerifyUser(userToken);
                if (userName == null)
                    userName = "ServiceUser";
                string result = DersaUtil.GetAttributeValue(userName, int.Parse(entityId), attrName, -1);
                DIOS.WCF.Core.WcfCoreUtil.RemoveUserFromUserTable();
                return result;
                //System.Data.DataTable T = DM.ExecuteSPWithParams("ENTITY$GetAttribute", new object[] { entityId, attrName, userName, DersaUtil.GetPassword(userName) });
                //string result = "";
                //if (T.Rows.Count > 0)
                //    result = T.Rows[0]["Value"].ToString();
                //return result;
            }
            catch(Exception exc)
            {
                return "";
            }
        }
        public string SetAttrValue(string attr_name, string entity_id, string attr_value, string token)
        {
            try 
            {
                string userName = WcfCoreUtil.VerifyUser(token);
                //Cryptor.Decrypt(token, "DERSA");
                DersaUtil.SetAttributeValue(new DersaAnonimousSqlManager(), userName, AttributeOwnerType.Entity, entity_id, attr_name, -1, attr_value);
                //NodeControllerAdapter.SetTextProperty(int.Parse(entity_id), attr_name, attr_value, userName);
                return "";
            }
            catch(Exception exc)
            {
                return exc.Message;
            }
        }
    }
}
