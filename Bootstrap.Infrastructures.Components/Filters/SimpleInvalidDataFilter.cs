using System.Linq;
using Bootstrap.Infrastructures.Models.Constants;
using Bootstrap.Infrastructures.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bootstrap.Infrastructures.Components.Filters
{
    public class SimpleInvalidDataFilter : ActionFilterAttribute
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings =
            new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new ContentResult
                {
                    Content = JsonConvert.SerializeObject(
                        new BaseResponse((int) ResponseCode.InvalidPayload,
                            string.Join("; ",
                                context.ModelState.Select(
                                    t =>
                                        $"{t.Key.Substring(0, 1)?.ToLower()}{t.Key.Substring(1)} {string.Join(", ", t.Value.Errors.Select(e => e.ErrorMessage))}"
                                            .Trim()))), _jsonSerializerSettings),
                    ContentType = "application/json"
                };
            }
        }
    }
}