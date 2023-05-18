namespace UserService.Filters
{
    public class Result<T>
    {
        public Result(int responseCode, bool status, string message, T data)
        {
            ResponseCode = responseCode;
            IsSuccess = status;
            Message = message;
            Data = data;
        }
        public Result(int responseCode, bool status, string message)
        {
            ResponseCode = responseCode;
            IsSuccess = status;
            Message = message;
        }
        public int ResponseCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }



        public static Result<T> Success(string message, T data)
        {
            return new Result<T>(0, true, message, data);
        }
        public static Result<T> Success(string message)
        {
            return new Result<T>(0, true, message);
        }
        public static Result<T> Failure(string message)
        {
            return new Result<T>(199, false, message);
        }
    }
    //public class ResponseResult<T>
    //{
    //    public ResponseResult(int responseCode, bool status, string message, T data)
    //    {
    //        ResponseCode = responseCode;
    //        IsSuccess = status;
    //        Messages = message;
    //        Data = data;
    //    }
    //    public ResponseResult(int responseCode, bool status, string message)
    //    {
    //        ResponseCode = responseCode;
    //        IsSuccess = status;
    //        Messages = message;
    //    }
    //    public int ResponseCode { get; set; }
    //    public bool IsSuccess { get; set; }
    //    public string Messages { get; set; }
    //    public T? Data { get; set; }



    //    public static ResponseResult<T> Success(string message, T data)
    //    {
    //        return new ResponseResult<T>(200, true, message, data);
    //    }
    //    public static ResponseResult<T> Success(string message)
    //    {
    //        return new ResponseResult<T>(200, true, message);
    //    }
    //    public static ResponseResult<T> Failure(string message)
    //    {
    //        return new ResponseResult<T>(417, false, message);
    //    }
    //}
}
