using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class ObjectExtensions
    {
        public static void SetKeyPropertyValue(this object instance, object keyValue)
        {
            var keyProperty = instance.GetType().GetKeyProperty();
            keyProperty.SetValue(instance, keyValue);
        }

        public static TValue GetKeyPropertyValue<TValue>(this object instance)
        {
            var keyProperty = instance.GetType().GetKeyProperty();
            return (TValue) keyProperty.GetValue(instance);
        }

        public static object GetKeyPropertyValue(this object instance) => GetKeyPropertyValue<object>(instance);
    }
}