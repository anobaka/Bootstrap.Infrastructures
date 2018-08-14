using System.Collections.Generic;
using Bootstrap.Infrastructures.Models.RequestModels;
using Bootstrap.Infrastructures.Models.ResponseModels;

namespace Bootstrap.Infrastructures.Components.Extensions
{
    public static class SearchRequestModelExtensions
    {
        public static SearchResponse<T> BuildResponse<T>(this SearchRequestModel model, List<T> data, int count,
            int code = 0, string message = null)
        {
            return new SearchResponse<T>
            {
                PageSize = model.PageSize,
                PageIndex = model.PageIndex,
                Data = data,
                TotalCount = count,
                Code = code,
                Message = message
            };
        }
    }
}
