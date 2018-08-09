using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public class StringUtils
    {
        private static readonly Random Random = new Random();

        public static string GetRandomNumber(int length)
        {
            return Random.Next(Convert.ToInt32(Math.Pow(10, length))).ToString($"D{length}");
        }
    }
}
