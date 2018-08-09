namespace Bootstrap.Infrastructures.Models.ResponseModels
{
    public class SingletonResponse<T> : BaseResponse
    {
        public T Data { get; set; }

        public SingletonResponse()
        {
        }

        public SingletonResponse(T data)
        {
            Data = data;
        }

        public SingletonResponse(int code) : base(code)
        {
        }

        public SingletonResponse(int code, string message) : base(code, message)
        {
        }
    }
}
