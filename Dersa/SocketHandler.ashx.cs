using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebSockets;
using System.Net.WebSockets;
using Dersa.Models;
using Dersa.Common;
using Newtonsoft.Json;
using System.Reflection;
using Dersa.Interfaces;

namespace Dersa
{
    /// <summary>
    /// Сводное описание для SocketHandler
    /// </summary>
    public class SocketHandler : IHttpHandler
    {
        private int ticker = 0;
        //private System.Timers.Timer theTimer = new System.Timers.Timer(1000);
        private AspNetWebSocketContext lastContext;
        private string entityId;
        private ArraySegment<byte> readBuffer = new ArraySegment<byte>(new byte[1024]);
        private Task<WebSocketReceiveResult> readTask;

        private ArraySegment<byte> GetResponse(ArraySegment<byte> src, int count)
        {
            var result = new ArraySegment<byte>(new byte[count]);
            for (int i = 0; i < count; i++)
                result.Array[count-i-1] = src.Array[i];
            return result;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(WebSocketRequest);
                entityId = context.Request["id"];
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private async Task WebSocketRequest(AspNetWebSocketContext context)
        {
            lastContext = context;
            string userName = context.User.Identity.Name;
            //Получаем сокет клиента из контекста запроса
            var socket = context.WebSocket;
            //theTimer.Elapsed += TheTimer_Elapsed;
            //theTimer.Start();
            while (true)
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);

                // Ожидаем данные от него
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                var sendBuff = GetResponse(buffer, result.Count);
                Tuple<ICompiled, MethodInfo> MI = QueryControllerAdapter.GetMethodInfo("GetMethodInfo", int.Parse(entityId));
                //string paramsString = JsonConvert.SerializeObject(
                //    new object[]{
                //        new {Name = "src", Value = "qwerty" },
                //    });
                try
                {
                    object scriptMethodInfo = MI.Item2.Invoke(MI.Item1, new object[] { });   //"System.String src=\"hello\""
                    Tuple<object, MethodInfo> MIinner = scriptMethodInfo as Tuple<object, MethodInfo>;
                    var communicationObject = new
                        DersaIO
                    {
                        ReadLine = () => ReceiveTextFromClient() ,
                        ReadLineVirtually = () => ReceiveTextFromClientVirtually(),
                        WriteLine = s => SendTextToClient(s)
                    };
                    //{
                    //    ReadLine = new Func<string>(() => { return ReceiveTextFromClient(); }),
                    //    WriteLine = new Action<string>((s) => SendTextToClient(s))
                    //};
                    object scriptResult = MIinner.Item2.Invoke(MIinner.Item1, new object[] { userName, communicationObject });
                    string responseString = scriptResult is string ? (string)scriptResult : "result of execution " + entityId + " is not a string";

                    //object scriptResult = QueryControllerAdapter.GetMethodResult("Exec", int.Parse(entityId), "[0]");
                    //string responseString = scriptResult is string ? (string)scriptResult : "result of execution " + entityId + " is not a string";
                    //string responseString = QueryControllerAdapter.GetActionForParams(JsonConvert.SerializeObject(
                    //    new object[]{
                    //        new {Name = "objectid", Value = entityId },
                    //        new {Name = "method_name", Value = "Exec" },
                    //        new {Name = "result_is_already_formatted", Value = true}
                    //    }));
                    //await socket.SendAsync(sendBuff, WebSocketMessageType.Text, true, CancellationToken.None);
                    //await SendTextToClient(Encoding.UTF8.GetString(buffer.ToArray<byte>(), 0, result.Count));
                    //await SendTextToClient(responseString);
                }
                catch(Exception exc)
                {
                    while (exc.InnerException != null)
                        exc = exc.InnerException;
                    DIOS.Common.Logger.LogStatic(exc.Message);
                    await SendTextToClient(exc.Message);
                }
            }
        }
        //private void TheTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    ticker++;
        //    if (ticker > 5)
        //        ticker = 1;
        //    SendTextToClient(ticker.ToString());
        //}

        private string ReceiveTextFromClient()
        {
            var socket = lastContext.WebSocket;
            // Ожидаем данные от него
            var readResult = socket.ReceiveAsync(readBuffer, CancellationToken.None);
            readResult.Wait();
            string result = Encoding.UTF8.GetString(readBuffer.ToArray<byte>(), 0, readResult.Result.Count);
            return result;
        }
        private async Task SendTextToClient(string text)
        {
            var socket = lastContext.WebSocket;
            var sendBuff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(text));
            socket.SendAsync(sendBuff, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private string ReceiveTextFromClientVirtually()
        {
            //если пока ничего не пришло с клиента = возвращаем null
            //если с клиента что-то пришло - обновляем таск и возвращаем то, чо пришло
            var socket = lastContext.WebSocket;
            if (readTask == null)
            {
                readTask = socket.ReceiveAsync(readBuffer, CancellationToken.None);
            }
            if (readTask.Status == TaskStatus.RanToCompletion || readTask.Status == TaskStatus.Faulted || readTask.Status == TaskStatus.Canceled)
            {
                string result = Encoding.UTF8.GetString(readBuffer.ToArray<byte>(), 0, readTask.Result.Count);
                readTask = socket.ReceiveAsync(readBuffer, CancellationToken.None);//ставим новую задачу на чтение
                return result;
            }
            else
                return null;
        }
    }
}