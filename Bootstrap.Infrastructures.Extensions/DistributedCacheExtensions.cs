using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetObjectAsync(this IDistributedCache cache, string key, object obj,
            DistributedCacheEntryOptions options = null)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            if (options == null)
            {
                await cache.SetAsync(key, data);
            }
            else
            {
                await cache.SetAsync(key, data, options);
            }
        }

        public static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
        {
            var json = await cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(json) ? JsonConvert.DeserializeObject<T>(json) : default(T);
        }
    }
}
