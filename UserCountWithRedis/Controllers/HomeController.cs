using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using ServiceStack.Redis;
using System.Web.Configuration;

namespace UserCountWithRedis.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
    }

    public class Product : Hub
    {
        static RedisEndpoint conf = new RedisEndpoint() { Host = "127.0.0.1", Port = 6379 };
        static string serverName = WebConfigurationManager.AppSettings["ServerName"];


        public override async Task OnConnected()
        {
            string clientId = Context.ConnectionId;
            if (UserClass.users.IndexOf(clientId) == -1)
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    bool isAdmin = Context.Request.Headers["Referer"].Contains("Admin");
                    //Groups.Add(clientId, "Index");
                    if (!isAdmin)
                        UserClass.users.Add(clientId);
                    await CheckAndSetUserCount(client, UserClass.users.Count(), true, isAdmin);
                }

            }
        }

        /*public override async Task OnReconnected()
        {
            string clientId = Context.ConnectionId;
            if (users.IndexOf(clientId) == -1)
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    bool isAdmin = Context.Request.Headers["Referer"].Contains("Admin");
                    users.Add(clientId);
                    await CheckAndSetUserCount(client, users.Count(),isAdmin);
                }
            }            
        }*/

        public override async Task OnDisconnected(bool stopCalled)
        {
            string clientId = Context.ConnectionId;

            if (UserClass.users.IndexOf(clientId) > -1)
            {
                using (IRedisClient client = new RedisClient(conf))
                {
                    //bool isAdmin = Context.Request.Headers["Referer"].Contains("Admin"); //Disconnect'e Url bilgisi gelmiyor.
                    //if (!isAdmin)
                    UserClass.users.Remove(clientId); //Hicbir zaman admin olmadigi icin kontrole gerek yok.
                    await CheckAndSetUserCount(client, UserClass.users.Count(), false, false); 
                }
            }
        }

        public async Task CheckAndSetUserCount(IRedisClient client, int userCount, bool isConnect, bool isAdmin)
        {
            string userCountWithServer = serverName + "æ" + userCount.ToString();

            if (isAdmin && isConnect)
            {
                await Groups.Add(Context.ConnectionId, "admin");
            }
            else if (!client.ContainsKey("UserCount")) // && isConnect==true eklenirse reconnect durumunda Console'a deger yanlış yere gönderilmez.
            {
                lock (UserClass.lockObject)
                {
                    if (!client.ContainsKey("UserCount"))
                    {
                        TimeSpan span = new TimeSpan(0, 0, 0, 20, 0);
                        client.Set("UserCount", UserClass.users.Count, span);
                        client.PublishMessage("UserCountService", userCountWithServer);
                    }
                }
            }
            await Clients.Group("admin").refreshUserCount(userCountWithServer);
        }
    }
}