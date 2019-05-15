using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Infrastructures.Extensions
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UploadDirectoryAttribute : Attribute
    {
        public string Directory { get; set; }

        public UploadDirectoryAttribute(string directory)
        {
            Directory = directory;
        }
    }
}