using System;
using ServiceStack.Redis;

namespace RevStack.Redis
{
    public class RedisDataContext
    {
        private const string DEFAULT_HOST= "localhost";
        private const int DEFAULT_PORT = 6379;
        private readonly string _host;
        private readonly int _port;
        public RedisDataContext()
        {
            _host = DEFAULT_HOST;
            _port = DEFAULT_PORT;
        }
        public RedisDataContext(string host,int port)
        {
            _host = host;
            _port = port;
        }
        public IRedisClient Client()
        {
            return new RedisClient(_host, _port);
        }
      
    }
}
