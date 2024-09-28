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
using System.Net.PeerToPeer;

namespace Dersa.Models
{
    public class SocketControllerAdapter
    {
        //private AspNetWebSocketContext lastContext;
        private static Hashtable contextTable = new Hashtable();
        private static Hashtable messageTable = new Hashtable();
        //private static int N = 0;
        private static Dictionary<string, int> UserSessionId = new Dictionary<string, int>();

        private static void IncrementSessionId(string messageKey)
        {
            if(UserSessionId.ContainsKey(messageKey))
                UserSessionId[messageKey]++;
            else
                UserSessionId.Add(messageKey, 1);
        }

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
                string currentMessageKey = wsContext.Cookies["messageKey"]?.Value;
                IncrementSessionId(currentMessageKey);
                var T = new Tuple<string, int>(currentMessageKey, UserSessionId[currentMessageKey]);
                var exContext = contextTable[userName] as AspNetWebSocketContext;
                string exMessageKey = exContext?.Cookies["messageKey"]?.Value;
                bool differentSessions = (exContext != null && exMessageKey != currentMessageKey);
                if (differentSessions)
                {
                    await SendTextToClient(userName, "Вы зашли в систему в другом браузере. Рекомендуется закрыть эту сессию. Сообщения для пользователя получает только браузер, который соединился последним.");
                }
                contextTable[userName] = wsContext;
                if (differentSessions)
                {
                    await SendTextToClient(userName, "У вас есть открытые ранее сессии. Рекомендуется закрыть эти сессии. Сообщения для пользователя получает только браузер, который соединился последним.");
                }
                while (T.Item1 != currentMessageKey || T.Item2 == UserSessionId[currentMessageKey])//this becomes false when the mthod is called next time for the same user
                {
                    if (messageTable[userName] != null)
                    {
                        await SendTextToClient(userName, messageTable[userName].ToString());
                        messageTable[userName] = null;
                    }
                    Thread.Sleep(100);
                }
                DIOS.Common.Logger.LogStatic($"WS changed context User = {userName}({currentMessageKey}); N = {UserSessionId[currentMessageKey]}");
            }
            catch (Exception exc)
            {
                DIOS.Common.Logger.LogStatic($"Error WS request processing {exc.Message}");
            }
        }
        public static async Task WebSocketAnonimousRequest(AspNetWebSocketContext wsContext)
        {
            DIOS.Common.Logger.LogStatic("start processing the request");
            await SendTextToSomebody(wsContext, JsonConvert.SerializeObject(new { MethodName = "SetUserName", MethodArgs = new { name = "GCG"} }));
            while(true)
            {
                if (messageTable[""] != null)
                {
                    await SendTextToSomebody(wsContext, messageTable[""].ToString());
                    messageTable[""] = null;
                }
                Thread.Sleep(100);
            }
        }

        private static async Task SendTextToClient(string userName, string text)
        {
            try
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
            catch (Exception exc)
            {
                DIOS.Common.Logger.LogStatic($"Error Send Text {exc.Message}");
            }
        }

        private static async Task SendTextToSomebody(AspNetWebSocketContext context, string text)
        {
            try
            {
                //var socket = lastContext.WebSocket;
                if (context == null)
                {
                    DIOS.Common.Logger.LogStatic($"WS context should not be null");
                }
                else
                {
                    //Получаем сокет клиента из контекста запроса  
                    var socket = context.WebSocket;
                    byte[] bytesToSend = Encoding.UTF8.GetBytes(text);

                    //отправляем заголовок с длиной текста
                    var sendHeaderBuff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { length = bytesToSend.Length })));
                    socket.SendAsync(sendHeaderBuff, WebSocketMessageType.Text, true, CancellationToken.None);
                    var sendBuff = new ArraySegment<byte>(bytesToSend);
                    socket.SendAsync(sendBuff, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception exc)
            {
                DIOS.Common.Logger.LogStatic($"Error Send Text {exc.Message}");
            }
        }
    }
}