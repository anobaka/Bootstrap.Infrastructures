using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class SessionExtensions
    {
        public static void Set(this ISession session, string key, object obj)
        {
            session.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var bytes = session.Get(key);
            if (bytes?.Any() == true)
            {
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
            }

            return default(T);
        }
    }
}
