using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsDefined<T>(this T e) where T : Enum
        {
            return SpecificEnumUtils<T>.Values.Contains(e);
        }
    }
}