using Microsoft.Extensions.Logging;

namespace Bootstrap.Infrastructures.Extensions
{
    public class CacheRedisClient : RedisClient
    {
        public CacheRedisClient(string connectionString, ILoggerFactory loggerFactory) : base(connectionString,
            loggerFactory)
        {
        }
    }
}