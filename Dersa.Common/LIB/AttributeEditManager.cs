using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;

namespace Dersa.Common
{
    public class AttributeEditManager
    {
        private class AttributeEditState
        {
            public AttributeEditState(string _login, string _key, DateTime _occur, string _sourceText)
            {
                login = _login;
                key = _key;
                occur = _occur;
                sourceText = _sourceText;
            }
            private string login;
            private string key;
            private DateTime occur;
            private string sourceText;

            public string Login
            {
                get
                {
                    return login;
                }
            }
            public string Key
            {
                get
                {
                    return key;
                }
            }
            public DateTime Occur
            {
                get
                {
                    return occur;
                }
            }
            public string SourceText
            {
                get
                {
                    return sourceText;
                }
            }
        }
        private static Hashtable attrStateTable = new Hashtable();
        private static Hashtable keysTable = new Hashtable();

        public static string MarkForEdit(int entity, string attrName, string login, string sourceText)
        {
            string canEditDiagnostic = CanEdit(entity, attrName, login);
            if (string.IsNullOrEmpty(canEditDiagnostic))
            {
                string loginKey = GetKeyForLogin(login);
                AttributeEditState[] exState = attrStateTable[GetHashKey(entity, attrName)] as AttributeEditState[];
                if(exState == null)
                    attrStateTable[GetHashKey(entity, attrName)] = new AttributeEditState[] { new AttributeEditState(login, loginKey, DateTime.Now, sourceText) };
                HttpCookie editKeyCookie = HttpContext.Current.Request.Cookies["editKey"];
                if (editKeyCookie == null || editKeyCookie.Value == "")
                {
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("editKey", loginKey));
                }

                return loginKey;
            }
//            throw new Exception(canEditDiagnostic);
            return canEditDiagnostic;
        }
        public static string CanEdit(int entity, string attrName, string login)
        {
            AttributeEditState[] attributeEditStates = attrStateTable[GetHashKey(entity, attrName)] as AttributeEditState[];
            if (attributeEditStates == null)
                return "";
            else
            {
                if (attributeEditStates.Length < 1)
                    return "";
                if (attributeEditStates[0].Login == login)
                    return "";
                return string.Format("Атрибут {0} сущности {1} редактируется пользователем {2}", attrName, entity, attributeEditStates[0].Login);
            }
        }
        public static bool CanPost(int entity, string attrName, string login, string key)
        {
            AttributeEditState[] attrEditArray = attrStateTable[GetHashKey(entity, attrName)] as AttributeEditState[];
            if (attrEditArray == null)
                return false;
            if (attrEditArray.Length < 1)
                return false;
            return (key == attrEditArray[0].Key);
        }
        public static void MarkForFree(int entity, string attrName)
        {
            attrStateTable[GetHashKey(entity, attrName)] = null;
        }
        private static string GetHashKey(int id, string name)
        {
            return id.ToString(0 + "_" + name);
        }

        private static string GetKeyForLogin(string login)
        {
            string loginKey = keysTable[login] as string;
            if (loginKey == null)
            {
                loginKey = Guid.NewGuid().ToString();
                keysTable[login] = loginKey;
            }
            return loginKey;
        }
    }

}
