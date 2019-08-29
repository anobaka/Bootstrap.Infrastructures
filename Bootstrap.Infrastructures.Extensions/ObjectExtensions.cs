using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class ObjectExtensions
    {
        public static void SetKeyPropertyValue(this object instance, object keyValue) =>
            instance.GetType().GetKeyProperty().SetValue(instance, keyValue);

        public static TValue GetKeyPropertyValue<TValue>(this object instance) =>
            (TValue) instance.GetType().GetKeyProperty().GetValue(instance);

        public static object GetKeyPropertyValue(this object instance) => GetKeyPropertyValue<object>(instance);

        public static T Copy<T>(this T t) =>
            t == null ? default : JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(t));
    }
}