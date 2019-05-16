using System;
using System.IO;
using System.Threading.Tasks;
using Bootstrap.Infrastructures.Components.FileManager.Storage;
using Bootstrap.Infrastructures.Models.ResponseModels;
using Microsoft.Extensions.Options;

namespace Bootstrap.Infrastructures.Components.FileManager
{
    public class FileManager
    {
        private readonly IFileStorage _storage;
        private readonly IOptions<FileManagerOptions> _options;

        public FileManager(IFileStorage storage, IOptions<FileManagerOptions> options)
        {
            _storage = storage;
            _options = options;
        }

        public async Task<SingletonResponse<string>> Save(object type, Stream file, string rawFilename)
        {
            if (string.IsNullOrEmpty(rawFilename))
            {
                throw new ArgumentNullException(rawFilename);
            }

            var relativeFilename = type.BuildFilename(rawFilename);

            if (!string.IsNullOrEmpty(_options.Value.RootDirectory))
            {
                relativeFilename = Path.Combine(_options.Value.RootDirectory, relativeFilename);
            }

            return await _storage.Save(relativeFilename, file);
        }
    }
}