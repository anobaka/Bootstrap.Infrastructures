using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Infrastructures.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bootstrap.Infrastructures.Components.HttpClient
{
    public class ServiceHttpClient<TOptions> : System.Net.Http.HttpClient
        where TOptions : ServiceHttpClientOptions, new()
    {
        protected IOptions<TOptions> Options;

        public ServiceHttpClient(IOptions<TOptions> options)
        {
            Options = options;
        }

        protected virtual async Task<T> InvokeAsync<T>(ServiceHttpClientRequestModel request,
            CancellationToken? cancellationToken = null)
        {
            var m = Convert(request);

            CancellationToken ct;
            var timeout = request.Timeout ?? Options.Value.Timeout;
            if (timeout.HasValue)
            {
                var cts = new CancellationTokenSource(timeout.Value);
                ct = cts.Token;
            }

            try
            {
                var rsp = await SendAsync(m, ct);
                rsp.EnsureSuccessStatusCode();
                var data = await rsp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch
            {
                if (!(request.BlockException ?? Options.Value.BlockException)) throw;
            }

            return default;
        }

        protected HttpRequestMessage Convert(ServiceHttpClientRequestModel request)
        {
            var url = Regex.Replace($"{Options.Value.Endpoint}/{request.RelativeUri}", "/{2,}", "/")
                .Replace(":/", "://");
            if (request.QueryParameters?.Any() == true)
            {
                if (!url.Contains("?")) url += "?";

                var qs = new List<string>();

                qs.AddRange(request.QueryParameters.Where(t => t.Value?.Any() == true).SelectMany(t =>
                    t.Value.Select(p => $"{WebUtility.UrlEncode(t.Key)}={WebUtility.UrlEncode(p.ToString())}")));

                qs.AddRange(request.QueryParameters.Where(t => t.Value?.Any() != true)
                    .Select(t => $"{WebUtility.UrlEncode(t.Key)}="));

                url += string.Join("&", qs);
            }

            var m = new HttpRequestMessage(request.Method, url)
            {
                Content = request.Content
            };
            if (request.Headers?.Any() == true)
                foreach (var k in request.Headers)
                    m.Headers.Add(k.Key, k.Value.ToString());

            return m;
        }
    }
}