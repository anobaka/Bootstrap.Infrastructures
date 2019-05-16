using System;
using System.Collections.Concurrent;
using System.IO;

namespace Bootstrap.Infrastructures.Components.FileManager
{
    public static class FileManagerExtensions
    {
        private static readonly ConcurrentDictionary<object, string> Directories =
            new ConcurrentDictionary<object, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">Business layer file type with <see cref="DirectoryAttribute"/>.</param>
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
                            typeof(DirectoryAttribute)) is DirectoryAttribute attr)
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