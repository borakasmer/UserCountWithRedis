using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UserCountWithRedis
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            UserClass.users.Clear();
        }
    }

    public static class UserClass
    {
        public static List<string> users = new List<string>();
        public static object lockObject = new Object();
    }

}
