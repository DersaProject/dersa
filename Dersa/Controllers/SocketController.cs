using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using Dersa.Models;

namespace Dersa.Controllers
{
    public class SocketController : Controller
    {
        public string Test(string user, string message)
        {
            try
            {
                SocketControllerAdapter.AcceptMessageForUser(user, message);
                return $"Accepted: {message} for user {user}";
            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
        public void Connect()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var context = HttpContext;
                if (context.IsWebSocketRequest)
                {
                    DIOS.Common.Logger.LogStatic($"request from {context.User.Identity.Name} is of WS type");
                    try
                    {
                        context.AcceptWebSocketRequest(SocketControllerAdapter.WebSocketRequest);
                    }
                    catch (Exception exc)
                    {
                        DIOS.Common.Logger.LogStatic($"error {exc.Message}");
                    }
                }
            }
            else
                DIOS.Common.Logger.LogStatic("WS request from non-authenticated user");
        }

    }
}
