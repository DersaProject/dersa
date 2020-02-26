using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dersa.Controllers
{
    public static class QueryControllerExtender
    {
        public static string Test(this QueryController queryController, string x)
        {
            return x;
        }
    }
}