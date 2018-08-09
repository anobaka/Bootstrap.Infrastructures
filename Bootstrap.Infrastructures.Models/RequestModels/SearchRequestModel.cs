using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Bootstrap.Infrastructures.Models.ResponseModels;
using Newtonsoft.Json;

namespace Bootstrap.Infrastructures.Models.RequestModels
{
    public class SearchRequestModel
    {
        /// <summary>
        /// 从1开始
        /// </summary>
        public virtual int PageIndex { get; set; } = 1;

        /// <summary>
        ///     最大值100, 默认20, 如需调整直接覆盖
        /// </summary>
        [Range(0, 100)]
        public virtual int PageSize { get; set; } = 20;

        [JsonIgnore] public int SkipCount => PageSize * (PageIndex - 1);
    }

    public static class SearchRequestModelExtensions
    {
        public static SearchResponse<T> BuildSearchResponse<T>(this SearchRequestModel model, List<T> data, int count,
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