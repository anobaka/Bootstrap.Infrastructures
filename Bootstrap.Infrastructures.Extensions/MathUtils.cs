using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public class MathUtils
    {
        private static readonly Random Random = new Random();

        public static int GetRandomInteger()
        {
            return Random.Next();
        }

        public static int GetRandomInteger(int maxValue)
        {
            return Random.Next(maxValue);
        }
    }
}
