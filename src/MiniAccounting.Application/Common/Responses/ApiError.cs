namespace MiniAccounting.Application.Common.Responses;

public class ApiError
{
    public string Message { get; set; }

    public Dictionary<string, string[]> Errors { get; set; }

    public ApiError(
        string message,
        Dictionary<string, string[]> errors)
    {
        Message = message;
        Errors = errors;
    }
}