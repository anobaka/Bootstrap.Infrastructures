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

        public static byte[] Crc16(byte[] data)
        {
            var len = data.Length;
            if (len > 0)
            {
                ushort crc = 0xFFFF;
                for (var i = 0; i < len; i++)
                {
                    crc = (ushort) (crc ^ (data[i]));
                    for (var j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort) ((crc >> 1) ^ 0xA001) : (ushort) (crc >> 1);
                    }
                }

                var hi = (byte) ((crc & 0xFF00) >> 8); //High
                var lo = (byte) (crc & 0x00FF); //Low

                return new[] {lo, hi};
            }

            return new byte[] {0, 0};
        }
    }
}