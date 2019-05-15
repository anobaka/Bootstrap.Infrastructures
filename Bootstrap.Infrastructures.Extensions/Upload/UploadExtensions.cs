using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions.Upload
{
    public static class UploadExtensions
    {
        private static readonly ConcurrentDictionary<object, string> Directories =
            new ConcurrentDictionary<object, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">Business layer file type with <see cref="UploadDirectoryAttribute"/>.</param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string BuildFilename<T>(this T type, string filename)
        {
            var dir = Directories.GetOrAdd(type, x =>
            {
                var t = type.GetType();
                var name = Enum.GetName(t, type);
                if (name != null)
                {
                    var field = t.GetField(name);
                    if (field != null)
                    {
                        if (Attribute.GetCustomAttribute(field,
                            typeof(UploadDirectoryAttribute)) is UploadDirectoryAttribute attr)
                        {
                            return attr.Directory;
                        }
                    }
                }

                return type.ToString();
            });
            return
                $"{dir}/{Path.GetFileNameWithoutExtension(filename)}-{DateTime.Now:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString()}-{Path.GetExtension(filename)}"
                    .Replace("//", "/");
        }
    }
}