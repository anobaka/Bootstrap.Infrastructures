using System;
using System.IO;
using System.Threading.Tasks;
using Bootstrap.Infrastructures.Components.FileUploader;
using Bootstrap.Infrastructures.Extensions.Upload;
using Bootstrap.Infrastructures.Models.ResponseModels;

namespace Bootstrap.Infrastructures.Components.FileManager
{
    public class FileManager
    {
        private readonly IFileStorage _storage;

        public FileManager(IFileStorage storage)
        {
            _storage = storage;
        }

        public async Task<SingletonResponse<string>> Upload(object type, Stream file, string rawFilename)
        {
            if (string.IsNullOrEmpty(rawFilename))
            {
                throw new ArgumentNullException(rawFilename);
            }

            return await _storage.Upload(type.BuildFilename(rawFilename), file);
        }
    }
}