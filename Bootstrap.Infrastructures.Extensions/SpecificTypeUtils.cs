using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Bootstrap.Infrastructures.Extensions
{
    public class SpecificTypeUtils<T>
    {
        public static Type Type = typeof(T);
        public static TypeInfo TypeInfo => Type.GetCachedTypeInfo();

        public static bool IsTypeOfNullable => TypeInfo.IsTypeOfNullable();

        public static PropertyInfo IdProperty => Type.GetKeyProperty();
    }
}