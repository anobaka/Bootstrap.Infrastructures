using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Bootstrap.Infrastructures.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string XForwardedForHeader = "x-forwarded-for";
        private const string AjaxHeader = "X-Requested-With";
        private const string AjaxHeaderValue = "XMLHttpRequest";

        public static string GetClientIp(this HttpRequest request)
        {
            return request.Headers.TryGetValue(XForwardedForHeader, out var values)
                ? values.FirstOrDefault().Split(',')[0].Trim()
                : request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request.Headers.TryGetValue(AjaxHeader, out var value) &&
                   AjaxHeaderValue.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool AcceptJson(this HttpRequest request)
        {
            return request.Headers[HeaderNames.Accept].Contains("json");
        }
    }
}
