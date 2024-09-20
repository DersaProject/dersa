using System;
using System.Collections;
using Newtonsoft.Json;
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
        private SocketControllerAdapter()
        {
        }

        private static async void ProcessMessages(WebSocket socket)
        {
            while (true)
            {
                if (messageTable.Count > 0)
                {
                    foreach (string userName in messageTable.Keys)
                    {
                        await SendTextToClient(socket, messageTable[userName].ToString());
                        messageTable.Remove(userName);
                    }
                }
                Thread.Sleep(100);
            }
        }
        private static SocketControllerAdapter staticAdapter = new SocketControllerAdapter();
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
                bool differentSessions = true;//(exContext != null) && (exContext.Cookies["messageKey"]?.Value != wsContext.Cookies["messageKey"]?.Value);
                if (differentSessions)
                {
                    await SendTextToClient(wsContext.WebSocket, "Вы зашли в систему в другом браузере. Рекомендуется закрыть эту сессию. Сообщения для пользователя получает только браузер, который соединился последним.");
                }
                contextTable[userName] = wsContext;
                if (differentSessions)
                {
                    await SendTextToClient(wsContext.WebSocket, "У вас есть открытые ранее сессии. Рекомендуется закрыть эти сессии. Сообщения для пользователя получает только браузер, который соединился последним.");
                }
            }
            catch (Exception exc)
            {
                DIOS.Common.Logger.LogStatic($"Error WS request processing {exc.Message}");
            }

            ProcessMessages(wsContext.WebSocket);

        }

        private static async Task SendTextToClient(WebSocket socket, string text)
        {
            //var socket = lastContext.WebSocket;
            //var context = contextTable[userName] as AspNetWebSocketContext;
            //if (context == null)
            //{
            //    DIOS.Common.Logger.LogStatic($"user {userName} has no saved WS contexts");
            //}
            //else
            //{
            //    //Получаем сокет клиента из контекста запроса  
            //    var socket = context.WebSocket;
                var sendBuff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(text));
                socket.SendAsync(sendBuff, WebSocketMessageType.Text, true, CancellationToken.None);
            //}
        }
    }
}