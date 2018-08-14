using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bootstrap.Infrastructures.Extensions
{
    public class InternalAuthenticationMiddleware
    {
        private readonly IHostingEnvironment _env;
        private readonly EventId _eventId = new EventId(0, nameof(InternalAuthenticationMiddleware));
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public InternalAuthenticationMiddleware(IHostingEnvironment env, RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _env = env;
            _next = next;
            _logger = loggerFactory.CreateLogger<InternalAuthenticationMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/internal", StringComparison.OrdinalIgnoreCase))
            {
                var fromInternal = true;
                var forwarded = context.Request.Headers["X-Forwarded-For"];
                for (var i = forwarded.Count - 1; i > 0; i--)
                {
                    if (!StringUtils.IsPrivateIpV4(forwarded[i]))
                    {
                        fromInternal = false;
                        break;
                    }
                }

                if (fromInternal)
                {
                    if (!StringUtils.IsPrivateIpV4(context.Connection.RemoteIpAddress.ToString()))
                    {
                        fromInternal = false;
                    }
                }

                if (!fromInternal)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    return;
                }
            }

            await _next(context);
        }
    }
}