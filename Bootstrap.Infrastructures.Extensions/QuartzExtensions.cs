using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class QuartzExtensions
    {
        public static T GetData<T>(this IJobExecutionContext context)
        {
            return (T) context.MergedJobDataMap.FirstOrDefault(t => t is T).Value;
        }

        public static IEnumerable<T> GetAllData<T>(this IJobExecutionContext context)
        {
            return context.MergedJobDataMap.Where(t => t.Value is T).Select(t => (T) t.Value);
        }

        public static T GetData<T>(this IJobExecutionContext context, string key)
        {
            return (T) context.MergedJobDataMap[key];
        }
    }
}