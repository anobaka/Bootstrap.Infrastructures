using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Infrastructures.Extensions;
using Bootstrap.Infrastructures.Models.Exceptions;
using Bootstrap.Infrastructures.Models.ResponseModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bootstrap.Infrastructures.Components.Middlewares
{
    public class SimpleRestfulApiExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        private readonly JsonSerializerSettings _jsonSerializerSettings =
            new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

        private readonly EventId _eventId = new EventId(0, nameof(SimpleRestfulApiExceptionHandlingMiddleware));

        public SimpleRestfulApiExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<SimpleRestfulApiExceptionHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError(_eventId, "An unhandled exception has occurred: " + ex.Message, ex);

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning(_eventId,
                        "The response has already started, the error handler will not be executed.");
                    throw;
                }
                context.Response.Clear();
                context.Response.Headers[HeaderNames.CacheControl] = "no-cache";
                context.Response.Headers[HeaderNames.Pragma] = "no-cache";
                context.Response.Headers[HeaderNames.Expires] = "-1";
                context.Response.Headers.Remove(HeaderNames.ETag);
                context.Response.StatusCode = (int) ex.StatusCode;
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(
                                new BaseResponse {Code = ex.ErrorCode, Message = ex.Message}, _jsonSerializerSettings),
                            Encoding.UTF8);
                        return;
                }
                context.Response.StatusCode = (int) ex.StatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError(_eventId, e, context.BuildRequestInformation());
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }
        }
    }

    public static class RestfulApiExceptionHandlingMiddlewareServiceCollectionExtensions
    {
        public static IApplicationBuilder UseRestfulApiSimpleExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SimpleRestfulApiExceptionHandlingMiddleware>();
        }
    }
}