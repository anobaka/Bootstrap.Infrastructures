using System.Collections.Generic;

namespace Bootstrap.Infrastructures.Models.ResponseModels
{
    public class ListResponse<T> : SingletonResponse<List<T>>
    {
        public ListResponse()
        {
        }

        public ListResponse(List<T> data) : base(data)
        {
        }

        public ListResponse(int code) : base(code)
        {
        }

        public ListResponse(int code, string message) : base(code, message)
        {
        }
    }
}
