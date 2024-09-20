using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;

namespace Dersa.Common
{
    public class MessageManager
    {
        public static void ProcessMessages()
        {

        }

        public static string SetNewKeyForLoginIfEmpty(string currentMessageKey)
        {
            if (!string.IsNullOrEmpty(currentMessageKey))
                return currentMessageKey;
            return Guid.NewGuid().ToString();
        }
    }

}
