using System;
using System.Net;

namespace Bootstrap.Infrastructures.Models.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public int ErrorCode { get; }

        public HttpResponseException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpResponseException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpResponseException(HttpStatusCode statusCode, int errorCode)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public HttpResponseException(HttpStatusCode statusCode, int errorCode, string message) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public HttpResponseException(HttpStatusCode statusCode, int errorCode, string message, Exception inner) : base(
            message,
            inner)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}