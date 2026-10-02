namespace MiniAccounting.Application.Common.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public T? Data { get; set; }

    public ApiError? Error { get; set; }

    public static ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResponse(ApiError error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error
        };
    }
}