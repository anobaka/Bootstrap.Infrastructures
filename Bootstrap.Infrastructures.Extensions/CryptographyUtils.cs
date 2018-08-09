using System;
using System.Security.Cryptography;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    public class CryptographyUtils
    {
        public static string Md5(string str)
        {
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "");
        }
    }
}
