using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class FuncExtensions
    {
        public static Func<TResource, object> BuildKeySelector<TResource>() => BuildKeySelector<TResource, object>();

        public static Func<TResource, TKey> BuildKeySelector<TResource, TKey>() =>
            ExpressionExtensions.BuildKeySelector<TResource, TKey>().Compile();
    }
}