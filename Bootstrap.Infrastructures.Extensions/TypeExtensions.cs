using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, TypeInfo> Cache = new ConcurrentDictionary<Type, TypeInfo>();

        public static bool IsTypeOfNullable(this TypeInfo typeInfo)
        {
            return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static TypeInfo GetTypeInfo(this Type type)
        {
            return Cache.GetOrAdd(type, t => t.GetTypeInfo());
        }

        public static bool IsTypeOfNullable(this Type type)
        {
            return type.GetTypeInfo().IsTypeOfNullable();
        }

        public static Type GetMostBaseClassOfNullableType(this Type type)
        {
            var t = type;
            while (t.IsTypeOfNullable())
            {
                t = t.GetGenericArguments()[0];
            }

            return t;
        }

        public static bool IsSimple(this Type type)
        {
            var t = type.GetMostBaseClassOfNullableType();
            var typeInfo = t.GetTypeInfo();
            return typeInfo.IsPrimitive || typeInfo.IsEnum || type == typeof(string) || type == typeof(decimal);
        }
    }
}