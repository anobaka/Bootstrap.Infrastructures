﻿using System;
using System.IO;
using System.Threading.Tasks;
using Aliyun.OSS;
using Bootstrap.Infrastructures.Components.FileUploader;
using Bootstrap.Infrastructures.Models.ResponseModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Infrastructures.Components.FileManager
{
    public class OssFileStorage : IFileStorage
    {
        private readonly OssClient _client;
        private readonly IOptions<OssClientFileStorageOptions> _options;
        private readonly ILogger<OssFileStorage> _logger;

        public OssFileStorage(IOptions<OssClientFileStorageOptions> options, ILogger<OssFileStorage> logger)
        {
            _options = options;
            _logger = logger;
            _client = new OssClient(options.Value.Endpoint, options.Value.AccessKeyId, options.Value.AccessKeySecret);
        }

        public Task<SingletonResponse<string>> Upload(string relativeFilename, Stream file)
        {
            return Task.Run(() =>
            {
                try
                {
                    _client.PutObject(_options.Value.BucketName, relativeFilename, file);
                    return new SingletonResponse<string>($"{_options.Value.Domain}/{relativeFilename}");
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