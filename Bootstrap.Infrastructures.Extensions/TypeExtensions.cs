using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, TypeInfo> Cache = new ConcurrentDictionary<Type, TypeInfo>();
        private static readonly CSharpCodeProvider _compiler = new CSharpCodeProvider();

        public static bool IsTypeOfNullable(this TypeInfo typeInfo)
        {
            return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static TypeInfo GetCachedTypeInfo(this Type type)
        {
            return Cache.GetOrAdd(type, t => t.GetTypeInfo());
        }

        public static bool IsTypeOfNullable(this Type type)
        {
            return type.GetCachedTypeInfo().IsTypeOfNullable();
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
            var typeInfo = t.GetCachedTypeInfo();
            return typeInfo.IsPrimitive || typeInfo.IsEnum || type == typeof(string) || type == typeof(decimal);
        }

        public static string GetCSharpRepresentation(this Type t)
        {
            if (t.IsGenericType)
            {
                var genericArgs = t.GetGenericArguments().ToList();

                return GetCSharpRepresentation(t, genericArgs);
            }

            var newType = new CodeTypeReference(t);
            return _compiler.GetTypeOutput(newType);
        }

        public static string GetCSharpRepresentation(this Type t, List<Type> availableArguments)
        {
            if (t.IsGenericType)
            {
                var value = t.Name;
                if (value.IndexOf("`") > -1)
                {
                    value = value.Substring(0, value.IndexOf("`"));
                }

                if (t.DeclaringType != null)
                {
                    // This is a nested type, build the nesting type first
                    value = GetCSharpRepresentation(t.DeclaringType, availableArguments) + "+" + value;
                }

                // Build the type arguments (if any)
                var argString = "";
                var thisTypeArgs = t.GetGenericArguments();
                for (var i = 0; i < thisTypeArgs.Length && availableArguments.Count > 0; i++)
                {
                    if (i != 0) argString += ", ";

                    argString += availableArguments[0].GetCSharpRepresentation();
                    availableArguments.RemoveAt(0);
                }

                // If there are type arguments, add them with < >
                if (argString.Length > 0)
                {
                    value += "<" + argString + ">";
                }

                return value;
            }

            return t.Name;
        }
    }
}