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

        public static void Reset(string login)
        {
            MarkForFree(login);
            CurrentEditKey = "";
        }

        private static string CurrentEditKey
        {
            get
            {
                HttpCookie editKeyCookie = HttpContext.Current.Request.Cookies["editKey"];
                if (editKeyCookie == null)
                    return null;
                return editKeyCookie.Value;
            }
            set
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("editKey", value));
            }
        }
        public static string MarkForEdit(int entity, string attrName, string login, string sourceText)
        {
            string canEditDiagnostic = CanEdit(entity, attrName, login);
            if (string.IsNullOrEmpty(canEditDiagnostic))
            {
                string loginKey = "";
                string currentKey = CurrentEditKey;
                if (string.IsNullOrEmpty(currentKey))
                {
                    loginKey = GetNewKeyForLogin();
                    CurrentEditKey =  loginKey;
                }
                else
                    loginKey = currentKey;
                AttributeEditState[] exState = attrStateTable[GetHashKey(entity, attrName)] as AttributeEditState[];
                if(exState == null)
                    attrStateTable[GetHashKey(entity, attrName)] = new AttributeEditState[] { new AttributeEditState(login, loginKey, DateTime.Now, sourceText) };

                return loginKey;
            }
            throw new Exception(canEditDiagnostic);
//            return canEditDiagnostic;
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
                if (attributeEditStates[0].Login == login && attributeEditStates[0].Key == CurrentEditKey)
                    return "";
                return string.Format("Атрибут {0} сущности {1} редактируется пользователем {2}", attrName, entity, attributeEditStates[0].Login);
            }
        }
        public static bool CanPost(int entity, string attrName, string login)
        {
            AttributeEditState[] attrEditArray = attrStateTable[GetHashKey(entity, attrName)] as AttributeEditState[];
            if (attrEditArray == null)
                return false;
            if (attrEditArray.Length < 1)
                return false;
            return (CurrentEditKey == attrEditArray[0].Key);
        }
        public static void MarkForFree(string login)
        {
            string currentKey = CurrentEditKey;
            if (string.IsNullOrEmpty(currentKey))
                return;
            string[] editEntries = new string[attrStateTable.Keys.Count];
            attrStateTable.Keys.CopyTo(editEntries, 0);
            foreach (string key in editEntries)
            {
                AttributeEditState[] attrEditArray = attrStateTable[key] as AttributeEditState[];
                if (attrEditArray == null || attrEditArray.Length == 0)
                    continue;
                if (attrEditArray[0].Login == login && attrEditArray[0].Key == currentKey)
                    attrStateTable[key] = null;
            }
        }
        public static void MarkForFree(string login, int entityId)
        {
            string[] attributeNames = DersaUtil.GetAttributeNames(entityId);
            foreach(string attrName in attributeNames)
            {
                MarkForFree(login, entityId, attrName);
            }
        }
        public static void MarkForFree(string login, int entity, string attrName)
        {
            string hashKey = GetHashKey(entity, attrName);
            AttributeEditState[] attrEditArray = attrStateTable[hashKey] as AttributeEditState[];
            if (attrEditArray == null)
                return;
            if (attrEditArray.Length < 1)
                return;
            if (attrEditArray[0].Login != login)
                return;
            attrStateTable[hashKey] = null;
        }
        private static string GetHashKey(int id, string name)
        {
            return id.ToString(0 + "_" + name);
        }

        private static string GetNewKeyForLogin()
        {
            return Guid.NewGuid().ToString();
        }
    }

}
