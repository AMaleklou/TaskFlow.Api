namespace TaskFlow.Api.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }

        public ApiResponse(bool success, string message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }

    public static class ApiResponse
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Success")
            => new ApiResponse<T>(true, message, data);

        public static ApiResponse<T> Fail<T>(string message)
            => new ApiResponse<T>(false, message, default);

        public static ApiResponse<T> Fail<T>(string message, T? data = default)
            => new ApiResponse<T>(false, message, data);
    }
}