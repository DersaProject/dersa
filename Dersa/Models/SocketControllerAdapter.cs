using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dersa.Common;
using DIOS.Common;
using DIOS.Common.Interfaces;
using Newtonsoft.Json;
using System.Reflection;
using Dersa.Interfaces;
using DersaStereotypes;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web.WebSockets;

namespace Dersa.Models
{
    public class SocketControllerAdapter
    {
        //private AspNetWebSocketContext lastContext;
        private static Hashtable contextTable = new Hashtable();
        private static Hashtable messageTable = new Hashtable();

        public static void AcceptMessageForUser(string user, string message)
        {
            messageTable[user] = message;
        }
        public static async Task WebSocketRequest(AspNetWebSocketContext wsContext)
        {
            try
            {
                DIOS.Common.Logger.LogStatic("start processing the request");
                //lastContext = context;
                string userName = wsContext.User.Identity.Name;
                var exContext = contextTable[userName] as AspNetWebSocketContext;
                if (exContext != null)
                {
                    string exUserInfo = JsonConvert.SerializeObject(new
                    {
                        Origin = exContext.Origin,
                        Path = exContext.Path,
                        PathInfo = exContext.PathInfo,
                        HostAddress = exContext.UserHostAddress,
                        HostName = exContext.UserHostName
                    });
                    DIOS.Common.Logger.LogStatic($"Ex User {exUserInfo} Ex Key {exContext.Cookies["messageKey"]?.Value}");
                    DIOS.Common.Logger.LogStatic($"New User {wsContext.UserAgent} New Key {wsContext.Cookies["messageKey"]?.Value}");
                }
                bool differentSessions = (exContext != null) && (exContext.Cookies["messageKey"]?.Value != wsContext.Cookies["messageKey"]?.Value);
                if (differentSessions)
                {
                    await SendTextToClient(userName, "Вы зашли в систему в другом браузере. Рекомендуется закрыть эту сессию. Сообщения для пользователя получает только браузер, который соединился последним.");
                }
                contextTable[userName] = wsContext;
                if (differentSessions)
                {
                    await SendTextToClient(userName, "У вас есть открытые ранее сессии. Рекомендуется закрыть эти сессии. Сообщения для пользователя получает только браузер, который соединился последним.");
                }
                while (true)
                {
                    if (messageTable[userName] != null)
                    {
                        await SendTextToClient(userName, messageTable[userName].ToString());
                        messageTable[userName] = null;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception exc)
            {
                DIOS.Common.Logger.LogStatic($"Error WS request processing {exc.Message}");
            }
        }

        private static async Task SendTextToClient(string userName, string text)
        {
            //var socket = lastContext.WebSocket;
            var context = contextTable[userName] as AspNetWebSocketContext;
            if (context == null)
            {
                DIOS.Common.Logger.LogStatic($"user {userName} has no saved WS contexts");
            }
            else
            {
                //Получаем сокет клиента из контекста запроса  
                var socket = context.WebSocket;
                var sendBuff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(text));
                socket.SendAsync(sendBuff, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}