using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Bootstrap.Infrastructures.Components.Middleware;
using Bootstrap.Infrastructures.Models.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Bootstrap.Infrastructures.Components.Filters
{
    /// <summary>
    /// Use <see cref="SimpleRestfulApiExceptionHandlingMiddleware"/> with this filter.
    /// </summary>
    public class SimpleRestfulApiInvalidPayloadFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid || await HandleModelStateErrors(context, context.ModelState))
            {
                await next();
            }
        }

        protected virtual Task<bool> HandleModelStateErrors(ActionExecutingContext context,
            ModelStateDictionary modelState)
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