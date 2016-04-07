using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisSqlService
{
    class Program
    {
        static RedisEndpoint conf = new RedisEndpoint() { Host = "127.0.0.1", Port = 6379 };
        static void Main(string[] args)
        {            
            using (IRedisClient client = new RedisClient(conf))
            {
                IRedisSubscription sub = null;
                using (sub = client.CreateSubscription())
                {
                    sub.OnMessage += (channel, message) =>
                    {
                        try
                        {
                            string serverName = message.Split('æ')[0];
                            string userCount = message.Split('æ')[1];
                            Console.WriteLine(DateTime.Now.ToString()+" : "+ serverName +" " +userCount);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    };
                }
                sub.SubscribeToChannels(new string[] { "UserCountService" });
            }

            Console.ReadLine();
        }
    }
}
