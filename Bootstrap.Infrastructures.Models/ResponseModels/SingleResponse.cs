namespace Bootstrap.Infrastructures.Models.ResponseModels
{
    public class SingleResponse<T> : BaseResponse
    {
        public T Data { get; set; }

        public SingleResponse()
        {
        }

        public SingleResponse(T data)
        {
            Data = data;
        }

        public SingleResponse(int code) : base(code)
        {
        }

        public SingleResponse(int code, string message) : base(code, message)
        {
        }
    }
}
