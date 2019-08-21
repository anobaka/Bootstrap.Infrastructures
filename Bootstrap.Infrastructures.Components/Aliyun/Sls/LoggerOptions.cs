using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Bootstrap.Infrastructures.Components.Aliyun.Sls
{
    public class LoggerOptions
    {
        #region 缓冲区设置
        public int DefaultTopicBufferSize { set; get; } = 1;
        public int? GlobalBufferSize { set; get; } = null;
        /// <summary>
        /// 如果不指定topic，则使用string.Empty作为key
        /// </summary>
        public IDictionary<string, int> TopicBufferSize { set; get; }
        #endregion

        #region 阿里云配置
//        public string Project { set; get; }
//        public string LogStore { set; get; }
        public string DefaultTopic { set; get; }
        public bool ForceUseDefaultTopic { set; get; }
        public string Source { get; set; }
        #endregion

        #region 通用配置
        public LogLevel MinLogLevel { set; get; } = LogLevel.Trace;
        public LogLevel MaxLogLevel { set; get; } = LogLevel.Critical;
        /// <summary>
        /// 指定Logger前缀
        /// </summary>
        public string Category { set; get; }
        public MatchFilter Filter { set; get; }
        public class MatchFilter
        {
            public string[] CategoryToMatch { set; get; }
            public bool AcceptOnMatch { set; get; }
        }
        #endregion
    }
}
