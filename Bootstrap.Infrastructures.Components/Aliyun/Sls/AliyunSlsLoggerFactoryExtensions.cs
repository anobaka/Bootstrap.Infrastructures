using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bootstrap.Infrastructures.Components.Aliyun.Sls
{
    public static class AliyunSlsLoggerFactoryExtensions
    {
		public static void AddAliyunSls(this ILoggingBuilder loggingBuilder, AliyunSlsOptions options)
        {
            loggingBuilder.AddProvider(new AliyunSlsProvider(Options.Create(options)));
        }
	}
}
