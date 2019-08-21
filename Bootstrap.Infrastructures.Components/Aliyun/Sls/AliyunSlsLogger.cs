using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aliyun.Api.LogService;
using Aliyun.Api.LogService.Domain.Log;
using Aliyun.Api.LogService.Infrastructure.Protocol.Http;
using Bootstrap.Infrastructures.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Infrastructures.Components.Aliyun.Sls
{
    public class AliyunSlsLogger : ILogger
    {
        private readonly HttpLogServiceClient _client;
        private readonly string _categoryName;
        private readonly List<LoggerOptions> _loggers;
        private readonly string _localIp;
        private readonly IOptions<AliyunSlsOptions> _options;
        private readonly object _lock = new object();

        /// <summary>
        /// Logger - Topic - Logs
        /// </summary>
        private readonly ConcurrentDictionary<LoggerOptions, ConcurrentDictionary<string, ConcurrentBag<LogInfo>>>
            _logBuffer =
                new ConcurrentDictionary<LoggerOptions, ConcurrentDictionary<string, ConcurrentBag<LogInfo>>>();

        public AliyunSlsLogger(HttpLogServiceClient client, IOptions<AliyunSlsOptions> options, string categoryName)
        {
            _client = client;
            _categoryName = categoryName;
            _options = options;
            _localIp = NetworkExtensions.GetLocalIpAddress();
            foreach (var option in options.Value.Loggers)
            {
                if (string.IsNullOrEmpty(option.Category) || _categoryName.StartsWith(option.Category))
                {
                    if (option.Filter == null ||
                        (option.Filter.AcceptOnMatch && option.Filter.CategoryToMatch?.Any(t =>
                             _categoryName.StartsWith(t, StringComparison.OrdinalIgnoreCase)) == true) ||
                        !option.Filter.AcceptOnMatch && option.Filter.CategoryToMatch?.Any(t =>
                            _categoryName.StartsWith(t, StringComparison.OrdinalIgnoreCase)) == false)
                    {
                        if (_loggers == null)
                        {
                            _loggers = new List<LoggerOptions>();
                        }

                        _loggers.Add(option);
                    }
                }
            }
        }

        private bool _isEnabled(LogLevel logLevel, LoggerOptions options)
            => options.MinLogLevel <= logLevel && options.MaxLogLevel >= logLevel;

        //todo: 并发测试
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                Task.Run(() =>
                {
                    var validLoggers = _loggers.Where(l => _isEnabled(logLevel, l)).ToList();
                    if (validLoggers.Any())
                    {
                        #region 构造LogItem

                        var contents = new Dictionary<string, string>
                        {
                            {"message", formatter(state, exception)},
                            {"level", logLevel.ToString()},
                            {"eventId", $"{eventId.Id}, {eventId.Name}"},
                            {"logger", _categoryName}
                        };
                        if (exception != null)
                        {
                            contents.Add("exception",
                                $"{exception.Message}{(string.IsNullOrEmpty(exception.StackTrace) ? null : "\n")}{exception.StackTrace}");
                            var tmp = exception.InnerException;
                            var layer = 0;
                            //防止死循环
                            while (tmp != null && layer < 10)
                            {
                                layer++;
                                contents.Add($"exception-inner{layer}",
                                    $"{tmp.Message}{(string.IsNullOrEmpty(exception.StackTrace) ? null : "\n")}{tmp.StackTrace}");
                                tmp = tmp.InnerException;
                            }
                        }

                        var logInfo = new LogInfo
                        {
                            Contents = contents,
                            Time = DateTimeOffset.Now
                        };

                        #endregion

                        var toBePushedLogs = new ConcurrentDictionary<string, List<LogInfo>>();
                        foreach (var logger in validLoggers)
                        {
                            var topic = (logger.ForceUseDefaultTopic
                                            ? logger.DefaultTopic
                                            : eventId.Name ?? logger.DefaultTopic) ?? logLevel.ToString().ToLower();
                            //丢至缓冲区
                            var currentBuffer = _logBuffer.GetOrAdd(logger,
                                f => new ConcurrentDictionary<string, ConcurrentBag<LogInfo>>());
                            var topicBufferedItems =
                                currentBuffer.GetOrAdd(topic, f => new ConcurrentBag<LogInfo>());
                            topicBufferedItems.Add(logInfo);

                            var reachGlobalLimit = false;
                            if (logger.GlobalBufferSize.HasValue)
                            {
                                lock (_lock)
                                {
                                    //全局缓冲区已满，则全部推送
                                    if (_logBuffer[logger].Sum(t1 => t1.Value?.Count) >= logger.GlobalBufferSize.Value)
                                    {
                                        if (_logBuffer.TryRemove(logger, out var tmpLogBuffer))
                                        {
                                            reachGlobalLimit = true;
                                            toBePushedLogs =
                                                new ConcurrentDictionary<string, List<LogInfo>>(
                                                    tmpLogBuffer.ToDictionary(t => t.Key, t => t.Value.ToList()));
                                        }
                                    }
                                }
                            }

                            if (!reachGlobalLimit)
                            {
                                // 单Topic缓冲区已满，则推送
                                var topicBufferSize = logger.TopicBufferSize?.ContainsKey(topic) == true
                                    ? logger.TopicBufferSize[topic]
                                    : logger.DefaultTopicBufferSize;
                                //如果topic缓冲区已满，则推送当前topic
                                lock (_lock)
                                {
                                    if (topicBufferedItems.Count >= topicBufferSize)
                                    {
                                        //推送当前topic
                                        if (currentBuffer.TryRemove(topic, out var logItems))
                                        {
                                            toBePushedLogs.GetOrAdd(topic, t => new List<LogInfo>()).AddRange(logItems);
                                        }
                                    }
                                }
                            }
                        }

                        //需要立即发送
                        if (toBePushedLogs?.Any() == true)
                        {
                            var requests = toBePushedLogs.Select(t => new PostLogsRequest(_options.Value.LogStore,
                                new LogGroupInfo {Topic = t.Key, Logs = t.Value, Source = _localIp}));
                            foreach (var r in requests)
                            {
                                var response = _client.PostLogStoreLogsAsync(r);
                            }
                        }
                    }
                });
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && _loggers?.Any() == true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        private class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}