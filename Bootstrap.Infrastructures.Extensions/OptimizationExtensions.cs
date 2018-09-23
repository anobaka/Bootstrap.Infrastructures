using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class OptimizationExtensions
    {
        public static void OptimizeSingleAndListData<T, TData>(this T data, Func<T, TData> getSingleData,
            Func<T, IEnumerable<TData>> getListData, Action<TData> setSingleData,
            Action<IEnumerable<TData>> setListData) where TData : class
        {
            var single = getSingleData(data);
            var list = getListData(data)?.Where(t => t != null && t != single).Distinct().ToList();
            if (list?.Any() == true)
            {
                if (single != null)
                {
                    list.Add(single);
                    single = null;
                }
                else
                {
                    if (list.Count == 1)
                    {
                        single = list.FirstOrDefault();
                        list = null;
                    }
                }
            }

            setSingleData(single);
            setListData(list);
        }
    }
}