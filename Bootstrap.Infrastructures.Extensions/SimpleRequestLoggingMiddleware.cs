using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bootstrap.Infrastructures.Extensions
{
    //todo: add options
    public class SimpleRequestLoggingMiddleware
    {
        private readonly EventId _eventId = new EventId(0, nameof(SimpleRequestLoggingMiddleware));
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public SimpleRequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<InternalAuthenticationMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            using (var reqBody = new MemoryStream())
            {
                var originalReqBody = context.Request.Body;
                await originalReqBody.CopyToAsync(reqBody);
                reqBody.Seek(0, SeekOrigin.Begin);
                context.Request.Body = reqBody;

                using (var resBody = new MemoryStream())
                {
                    var originalResBody = context.Response.Body;
                    context.Response.Body = resBody;

                    await _next(context);

                    // logging
                    reqBody.Seek(0, SeekOrigin.Begin);
                    resBody.Seek(0, SeekOrigin.Begin);
                    string reqString, resString;
                    using (var reqSteamReader = new StreamReader(reqBody))
                    {
                        reqString = await reqSteamReader.ReadToEndAsync();
                        context.Request.Body = originalReqBody;
                    }

                    using (var resStreamReader = new StreamReader(resBody))
                    {
                        resString = await resStreamReader.ReadToEndAsync();
                        resBody.Seek(0, SeekOrigin.Begin);
                        await resBody.CopyToAsync(originalResBody);
                        context.Response.Body = originalResBody;
                    }

                    var authentication = await context.AuthenticateAsync();
                    // may be null
                    var principal = authentication.Principal;
                    var isAuthenticated = principal?.Identity.IsAuthenticated == true;
                    var lines = new List<string>
                    {
                        BuildDelimiter("request"),
                        $"url: {context.Request.GetDisplayUrl()}",
                        $"remote address: {context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}",
                        $"method: {context.Request.Method}",
                        $"headers: \n{string.Join("\n", context.Request.Headers.Select(t => $"\t{t.Key}: {t.Value}"))}",
                        $"body: {reqString}",
                        BuildDelimiter("session"),
                        $"authenticated: {isAuthenticated}",
                        $"name: {principal?.Identity.Name}",
                        $"claims: {(principal != null ? $"{{{string.Join(", ", principal.Claims.Select(t => $"{t.Type}: {t.Value}"))}}}" : null)}",
                        BuildDelimiter("response"),
                        $"status: {context.Response.StatusCode}",
                        $"headers: \n{string.Join("\n", context.Response.Headers.Select(t => $"\t{t.Key}: {t.Value}"))}",
                        $"body: {resString}"
                    };
                    _logger.LogInformation(_eventId, $"\n{string.Join("\n", lines)}");
                }
            }
        }

        private static string BuildDelimiter(string text)
        {
            var leftLength = (50 - text.Length) / 2;
            return $"{text.PadLeft(leftLength + text.Length, '-').PadRight(50, '-')}";
        }
    }
}