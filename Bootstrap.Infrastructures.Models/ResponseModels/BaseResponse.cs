using System.Runtime.InteropServices.ComTypes;

namespace Bootstrap.Infrastructures.Models.ResponseModels
{
    public class BaseResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public BaseResponse()
        {
        }

        public BaseResponse(int code)
        {
            Code = code;
        }

        public BaseResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}