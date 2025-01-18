using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using Dersa.Models;
using Dersa.Common;

namespace Dersa.Controllers
{
    public class SocketController : Controller
    {
        public string DisconnectClient()
        {
            return MessageManager.DisconnectClient();
        }
        public string ClientMessage(string message)
        {
            var context = HttpContext;
            string clientName = context.User.Identity.Name + "_client";
            return Message(message, clientName);
        }
        public string Message(string message, string user)
        {
            try
            {
                MessageManager.AcceptMessageForUser(user, message);
                return $"Accepted: {message} for user {user}";
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
        public void Connect(string clientLogin = "")
        {
            var context = HttpContext;
            if (context.IsWebSocketRequest)
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    DIOS.Common.Logger.LogStatic("WS request from non-authenticated user " + clientLogin);
                    context.Response.Cookies.Add(new System.Web.HttpCookie("login", clientLogin + "_client"));
                }
                DIOS.Common.Logger.LogStatic($"request from {context.User.Identity.Name} is of WS type");
                try
                {
                    context.AcceptWebSocketRequest(MessageManager.WebSocketRequest);
                }
                catch (Exception exc)
                {
                    DIOS.Common.Logger.LogStatic($"Error: {exc.Message}");
                }
            }
        }
    }
}
