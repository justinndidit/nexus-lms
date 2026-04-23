namespace webApi.Shared.Responses;
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string? TraceId { get; set; }
    public string Action {get; set;} = string.Empty;
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(
        T data,
        int statusCode = 200,
        string message = "success",
        string? traceId = null,
        string action = ""
)
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            TraceId = traceId,
            Action = action
        };
    }
    public static ApiResponse<T> Created(
        T data,
        string message = "created successfully",
        string? traceId = null,
        string action = ""
)
    {
        return SuccessResponse(data, 201, message, traceId, action);
    }
    public static ApiResponse<T> Fail(
            string message,
            List<string>? errors = null,
            int statusCode = 400,
            string? traceId = null,
            string action = ""
        )
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors,
                TraceId = traceId,
                Action = action
            };
    }
    public static ApiResponse<T> Error(
        string message = "internal server error",
        List<string>? errors = null,
        int statusCode = 500,
        string? traceId = null,
        string action = ""
    )
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors,
                TraceId = traceId,
                Action = action
            };
        }
}