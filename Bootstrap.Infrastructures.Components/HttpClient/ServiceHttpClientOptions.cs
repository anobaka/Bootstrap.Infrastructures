using System;

namespace Bootstrap.Infrastructures.Components.HttpClient
{
    public class ServiceHttpClientOptions
    {
        public TimeSpan? Timeout { get; set; }
        public string Endpoint { get; set; }
        public bool BlockException { get; set; }
    }
}
