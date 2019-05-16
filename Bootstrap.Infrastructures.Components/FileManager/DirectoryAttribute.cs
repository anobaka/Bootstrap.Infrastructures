using System;

namespace Bootstrap.Infrastructures.Components.FileManager
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DirectoryAttribute : Attribute
    {
        public string Directory { get; set; }

        public DirectoryAttribute(string directory)
        {
            Directory = directory;
        }
    }
}