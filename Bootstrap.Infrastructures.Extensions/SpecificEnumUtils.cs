using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class SpecificEnumUtils<TEnum> where TEnum : struct
    {
        public static List<TEnum> Values = Enum.GetValues(SpecificTypeUtils<TEnum>.Type).Cast<TEnum>().ToList();

        private static readonly ConcurrentDictionary<TEnum, string> DisplayNames =
            new ConcurrentDictionary<TEnum, string>();

        public static string GetDisplayName(TEnum value)
        {
            return DisplayNames.GetOrAdd(value, t =>
            {
                var type = value.GetType();
                var name = Enum.GetName(type, value);
                if (name != null)
                {
                    var field = type.GetField(name);
                    if (field != null)
                    {
                        if (Attribute.GetCustomAttribute(field,
                            typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                        {
                            return attr.Description;
                        }
                    }
                }

                return value.ToString();
            });
        }
    }
}