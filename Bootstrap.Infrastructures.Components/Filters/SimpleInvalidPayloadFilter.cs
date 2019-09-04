using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Infrastructures.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bootstrap.Infrastructures.Components.Filters
{
    public class SimpleInvalidPayloadFilter : ActionFilterAttribute
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings =
            new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                await next();
            }
            else
            {
                await HandleModelStateErrors(context, context.ModelState);
            }
        }

        protected virtual Task<bool> HandleModelStateErrors(ActionExecutingContext context,
            ModelStateDictionary modelState)
        {
            var message = string.Join("; ",
                context.ModelState.Select(
                    t =>
                        $"{t.Key.Substring(0, 1)?.ToLower()}{t.Key.Substring(1)} {string.Join(", ", t.Value.Errors.Select(e => e.ErrorMessage))}"
                            .Trim()));
            context.Result = new ContentResult
            {
                Content = JsonConvert.SerializeObject(new BaseResponse(400, message), _jsonSerializerSettings),
                ContentType = "application/json"
            };
            return Task.FromResult(true);
        }
    }
}