using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrap.Infrastructures.Extensions
{
    public class FileUtils
    {
        public static async Task Save(string fullname, string content, FileMode mode = FileMode.Create,
            Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(fullname))
            {
                throw new ArgumentNullException(nameof(fullname));
            }

            var dir = Path.GetDirectoryName(fullname);
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentException();
            }

            Directory.CreateDirectory(dir);
            using (var fs = new FileStream(fullname, mode, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var sw = new StreamWriter(fs, encoding ?? Encoding.UTF8))
                {
                    await sw.WriteAsync(content);
                }
            }
        }

        public static async Task<string> ReadAsync(string fullname)
        {
            using (var fs = new FileStream(fullname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fs))
                {
                    return await sr.ReadToEndAsync();
                }
            }
        }
    }
}