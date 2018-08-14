using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bootstrap.Infrastructures.Extensions
{
    public class StringUtils
    {
        private static readonly Random Random = new Random();

        public static string GetRandomNumber(int length)
        {
            return Random.Next(Convert.ToInt32(Math.Pow(10, length))).ToString($"D{length}");
        }
        public static bool IsPublicIpV4(string ip)
        {
            return Regex.IsMatch(ip,
                @"^([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(?<!172\.(16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31))(?<!127)(?<!^10)(?<!^0)\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(?<!192\.168)(?<!172\.(16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31))\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(?<!\.255$)$");
        }

        public static bool IsPrivateIpV4(string ip)
        {
            return Regex.IsMatch(ip,
                @"(^127\.)|(^10\.) |(^172\.1[6-9]\.)|(^172\.2[0-9]\.)|(^172\.3[0-1]\.)|(^192\.168\.)|(^::1)");
        }
    }
}
