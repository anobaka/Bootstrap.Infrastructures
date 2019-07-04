using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Bootstrap.Infrastructures.Models.RequestModels
{
    public class ServiceHttpClientRequestModel
    {
        public HttpMethod Method { get; set; }
        public Dictionary<string, List<object>> QueryParameters { get; set; }
        public Dictionary<string, IEnumerable<object>> Headers { get; set; }
        public string RelativeUri { get; set; }
        public TimeSpan? Timeout { get; set; }
        public bool? BlockException { get; set; }

        private HttpContent _content;

        /// <summary>
        /// Support json only for now.
        /// </summary>
        public virtual HttpContent Content
        {
            get
            {
                if (_content == null)
                {
                    if (Body != null)
                    {
                        return _content = new StringContent(JsonConvert.SerializeObject(Body), Encoding.UTF8,
                            "application/json");
                    }
                }

                return _content;
            }
            set => _content = value;
        }

        public object Body { get; set; }
    }
}