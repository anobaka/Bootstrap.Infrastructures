using System.Linq;
using System.Net;
using Bootstrap.Infrastructures.Components.Middleware;
using Bootstrap.Infrastructures.Models.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Bootstrap.Infrastructures.Components.Filters
{
    /// <summary>
    /// Use <see cref="RestfulApiSimpleExceptionHandlingMiddleware"/> with this filter.
    /// </summary>
    public class RestfulApiSimpleInvalidDataFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                if (context.ModelState.Any(t => t.Value.Errors.Any(a => a.Exception is JsonReaderException)))
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest, "The payload is not invalid JSON");
                }
                throw new HttpResponseException(HttpStatusCode.BadRequest, string.Join("; ",
                    context.ModelState.Select(
                        t =>
                            $"{t.Key.Substring(0, 1)?.ToLower()}{t.Key.Substring(1)} {string.Join(", ", t.Value.Errors.Select(e => e.ErrorMessage))}"
                                .Trim())));
            }
        }
    }
}