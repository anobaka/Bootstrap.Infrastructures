using System.Collections.Generic;

namespace Bootstrap.Infrastructures.Models.ResponseModels
{
    public class SearchResponse<T> : SingleResponse<IEnumerable<T>>
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}