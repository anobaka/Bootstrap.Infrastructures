using System;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Infrastructures.Extensions;
using Bootstrap.Infrastructures.Models.ResponseModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bootstrap.Infrastructures.Components.Middlewares
{
    public class SimpleExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly EventId _eventId = new EventId(0, nameof(SimpleExceptionHandlingMiddleware));
        protected readonly string AjaxResponse;

        public SimpleExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<SimpleExceptionHandlingMiddleware>();
            AjaxResponse = JsonConvert.SerializeObject(new BaseResponse(500, "Internal Server Error"),
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(_eventId, e, context.BuildRequestInformation());
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.Headers[HeaderNames.CacheControl] = "no-cache";
                    context.Response.Headers[HeaderNames.Pragma] = "no-cache";
                    context.Response.Headers[HeaderNames.Expires] = "-1";
                    context.Response.Headers.Remove(HeaderNames.ETag);
                    //Default behavior
                    if (context.Request.AcceptJson())
                    {
                        await context.Response.WriteAsync(AjaxResponse, Encoding.UTF8);
                    }
                    else
                    {
                        context.Response.Redirect("/");
                    }
                }
            }
        }
    }

    public static class ExceptionHandlingMiddlewareServiceCollectionExtensions
    {
        public static IApplicationBuilder UseSimpleExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SimpleExceptionHandlingMiddleware>();
        }
    }
}