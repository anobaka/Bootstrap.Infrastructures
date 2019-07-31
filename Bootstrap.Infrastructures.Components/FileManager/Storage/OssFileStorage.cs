using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aliyun.OSS;
using Bootstrap.Infrastructures.Models.ResponseModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Infrastructures.Components.FileManager.Storage
{
    public class OssFileStorage : IFileStorage
    {
        private readonly OssClient _client;
        private readonly IOptions<OssFileStorageOptions> _options;
        private readonly ILogger<OssFileStorage> _logger;

        public OssFileStorage(IOptions<OssFileStorageOptions> options, ILogger<OssFileStorage> logger)
        {
            _options = options;
            _logger = logger;
            _client = new OssClient(options.Value.Endpoint, options.Value.AccessKeyId, options.Value.AccessKeySecret);
        }

        public Task<SingletonResponse<string>> Save(string relativeFilename, Stream file)
        {
            return Task.Run(() =>
            {
                try
                {
                    relativeFilename = (_options.Value.RootPath + "/" + relativeFilename).Trim('/');
                    _client.PutObject(_options.Value.BucketName, relativeFilename, file);
                    return new SingletonResponse<string>(
                        $"{_options.Value.Domain}/{string.Join("/", relativeFilename.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).Select(WebUtility.UrlEncode))}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new SingletonResponse<string>
                    {
                        Code = (int) FileStorageUploadResponseCode.Error,
                        Message = ex.Message
                    };
                }
            });
        }
    }
}