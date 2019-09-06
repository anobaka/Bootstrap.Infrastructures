using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bootstrap.Infrastructures.Extensions
{
    public class FixSessionIdAttribute : ActionFilterAttribute
    {
        public FixSessionIdAttribute()
        {
            Order = int.MinValue;
        }

        private const string InitSessionKey = "Initialized";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Session.GetInt32(InitSessionKey).HasValue)
            {
                context.HttpContext.Session.SetInt32(InitSessionKey, 1);
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}