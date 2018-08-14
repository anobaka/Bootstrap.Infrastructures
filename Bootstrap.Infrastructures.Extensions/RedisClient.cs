using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Bootstrap.Infrastructures.Extensions
{
    public abstract class RedisClient 
    {
        protected readonly ILogger Logger;
        protected readonly EventId EventId;
        private DateTime _ipLatestCheckTime = DateTime.MinValue;
        private readonly string _hostName;
        private readonly string _connectionString;
        private string _latestIp;

        private ConnectionMultiplexer _connectionMultiplexer;

        protected RedisClient(string connectionString, ILoggerFactory loggerFactory)
        {
            var type = GetType();
            _connectionString = connectionString;
            Logger = loggerFactory.CreateLogger(type);
            var redisConfiguration = connectionString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
            _hostName = redisConfiguration.Substring(0, redisConfiguration.IndexOf(":", StringComparison.Ordinal));
            EventId = new EventId(0, type.Name);
        }

        public async Task<IDatabase> _getDbAsync(int dbIndex = -1)
        {
            return (await _connectAsync()).GetDatabase(dbIndex);
        }

        private async Task<ConnectionMultiplexer> _connectAsync(bool forceReconnect = false)
        {
            var ipChanged = false;
            if (string.IsNullOrEmpty(_latestIp) || DateTime.Now > _ipLatestCheckTime.AddMinutes(5))
            {
                var result = (await Dns.GetHostAddressesAsync(_hostName))[0].ToString();
                _ipLatestCheckTime = DateTime.Now;
                if (_latestIp != result)
                {
                    _latestIp = result;
                    ipChanged = true;
                }
            }
            if (forceReconnect || ipChanged)
            {
                _connectionMultiplexer?.Dispose();
                _connectionMultiplexer = null;
            }
            return _connectionMultiplexer ?? (_connectionMultiplexer =
                       await ConnectionMultiplexer.ConnectAsync(_connectionString.Replace(_hostName, _latestIp)));
        }

        public async Task ExecuteAsync(Func<IDatabase, Task> action, int dbIndex = -1, bool retryAfterReconnect = true)
        {
            var db = dbIndex > -1 ? await _getDbAsync(dbIndex) : await _getDbAsync();
            try
            {
                await action(db);
            }
            catch (RedisConnectionException e)
            {
                //todo: 测试连接失败重试
                Logger.LogError(EventId, e, $"redis connection error, failureType: {e.FailureType}");
                await _connectAsync(true);
                if (retryAfterReconnect)
                {
                    //只重试一次
                    await ExecuteAsync(action, dbIndex, false);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(EventId, e, "execute error");
            }
        }

        /// <summary>
        /// If fails, return default(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="dbIndex"></param>
        /// <param name="retryAfterReconnect"></param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(Func<IDatabase, Task<T>> action, int dbIndex = -1,
            bool retryAfterReconnect = true)
        {
            var db = dbIndex > -1 ? await _getDbAsync(dbIndex) : await _getDbAsync();
            try
            {
                return await action(db);
            }
            catch (RedisConnectionException e)
            {
                //todo: 测试连接失败重试
                Logger.LogError(EventId, e, $"redis connection error, failureType: {e.FailureType}");
                await _connectAsync(true);
                if (retryAfterReconnect)
                {
                    //只重试一次
                    return await ExecuteAsync(action, dbIndex, false);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(EventId, e, "execute error");
            }
            return default(T);
        }
    }
}
