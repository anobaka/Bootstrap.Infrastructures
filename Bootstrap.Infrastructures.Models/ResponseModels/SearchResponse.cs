using System.Collections.Generic;

namespace Bootstrap.Infrastructures.Models.ResponseModels
{
    public class SearchResponse<T> : ListResponse<T>
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public SearchResponse()
        {
        }

        public SearchResponse(List<T> data, int totalCount, int pageIndex, int pageSize) : base(data)
        {
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public SearchResponse(int code) : base(code)
        {
        }

        public SearchResponse(int code, string message) : base(code, message)
        {
        }
    }
}