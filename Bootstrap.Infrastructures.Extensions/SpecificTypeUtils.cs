using System;
using System.Reflection;

namespace Bootstrap.Infrastructures.Extensions
{
    public class SpecificTypeUtils<T>
    {
        public static Type Type = typeof(T);
        public static TypeInfo TypeInfo => Type.GetCachedTypeInfo();

        public static bool IsTypeOfNullable => TypeInfo.IsTypeOfNullable();
    }
}