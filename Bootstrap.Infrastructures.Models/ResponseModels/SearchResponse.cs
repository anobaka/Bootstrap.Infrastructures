namespace Bootstrap.Infrastructures.Models.ResponseModels
{
    public class SearchResponse<T> : ListResponse<T>
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}