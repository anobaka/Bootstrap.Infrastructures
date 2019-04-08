using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, TypeInfo> Cache = new ConcurrentDictionary<Type, TypeInfo>();
        private static readonly CSharpCodeProvider Compiler = new CSharpCodeProvider();

        private static readonly ConcurrentDictionary<Type, PropertyInfo> IdProperties =
            new ConcurrentDictionary<Type, PropertyInfo>();

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
            return Compiler.GetTypeOutput(newType);
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

        public static PropertyInfo GetKeyProperty(this Type type,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            return IdProperties.GetOrAdd(type, t1 =>
            {
                var properties = t1.GetProperties(bindingFlags)
                    .Where(t => t.GetCustomAttribute<NotMappedAttribute>() == null).ToList();
                foreach (var p in properties)
                {
                    if (p.GetCustomAttribute<KeyAttribute>() != null)
                    {
                        return p;
                    }
                }

                var possibleKeyNames = new[] {"Id", $"{t1.Name}Id"};
                return possibleKeyNames.Select(p => properties.FirstOrDefault(t => t.Name.Equals(p)))
                    .FirstOrDefault(key => key != null);
            });
        }
    }
}