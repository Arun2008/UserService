namespace AuthenticationService.Filters
{
    public class ResponseResult<T>
    {
        public ResponseResult(int responseCode, bool status, string message, T data)
        {
            ResponseCode = responseCode;
            IsSccess = status;
            Messages = message;
            Data = data;
        }
        public ResponseResult(int responseCode, bool status, string message)
        {
            ResponseCode = responseCode;
            IsSccess = status;
            Messages = message;
        }
        public int ResponseCode { get; set; }
        public bool IsSccess { get; set; }
        public string Messages { get; set; }
        public T? Data { get; set; }



        public static ResponseResult<T> Success(string message, T data)
        {
            return new ResponseResult<T>(200, true, message, data);
        }
        public static ResponseResult<T> Success(string message)
        {
            return new ResponseResult<T>(200, true, message);
        }
        public static ResponseResult<T> Failure(string message)
        {
            return new ResponseResult<T>(417, false, message);
        }
    }
}
