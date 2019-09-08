using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class ConcurrentBagExtensions
    {
        public static ConcurrentBag<T> Replace<T>(this ConcurrentBag<T> t, T old, T @new) =>
            new ConcurrentBag<T>(t.Except(new[] {old})) {@new};

        public static void AddRange<T>(this ConcurrentBag<T> t, IEnumerable<T> newData)
        {
            foreach (var d in newData)
            {
                t.Add(d);
            }
        }

        public static ConcurrentBag<T> Remove<T>(this ConcurrentBag<T> t, T data) =>
            new ConcurrentBag<T>(t.Except(new[] {data}));

        public static ConcurrentBag<T> RemoveRange<T>(this ConcurrentBag<T> t, IEnumerable<T> data) =>
            new ConcurrentBag<T>(data);
    }
}