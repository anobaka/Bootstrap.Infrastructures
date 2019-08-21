using System.Collections.Concurrent;
using Aliyun.Api.LogService;
using Aliyun.Api.LogService.Infrastructure.Protocol.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Infrastructures.Components.Aliyun.Sls
{
    public class AliyunSlsProvider : ILoggerProvider
    {
        private readonly IOptions<AliyunSlsOptions> _options;
        private readonly HttpLogServiceClient _client;

        private readonly ConcurrentDictionary<string, AliyunSlsLogger> _loggers =
            new ConcurrentDictionary<string, AliyunSlsLogger>();

        public AliyunSlsProvider(IOptions<AliyunSlsOptions> options)
        {
            _options = options;
            _client = LogServiceClientBuilders.HttpBuilder.Endpoint(options.Value.EndPoint, options.Value.Project)
                .Credential(options.Value.AccessId, options.Value.AccessKey).Build();
        }

        public void Dispose()
        {

        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, t => new AliyunSlsLogger(_client, _options, categoryName));
        }
    }
}