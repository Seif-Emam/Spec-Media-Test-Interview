namespace TestIjnterview.Common.Models;

/// <summary>
/// Domain and Application Result type for functional flow without unnecessary exceptions.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string Message { get; }
    public int StatusCode { get; }
    public IDictionary<string, string[]>? Errors { get; }

    private Result(bool isSuccess, T? value, string message, int statusCode, IDictionary<string, string[]>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
        StatusCode = statusCode;
        Errors = errors;
    }

    public static Result<T> Success(T value, string message = "Success", int statusCode = 200)
    {
        return new Result<T>(true, value, message, statusCode);
    }

    public static Result<T> Failure(string message, int statusCode = 400, IDictionary<string, string[]>? errors = null)
    {
        return new Result<T>(false, default, message, statusCode, errors);
    }

    public static Result<T> NotFound(string message = "Resource not found.")
    {
        return new Result<T>(false, default, message, 404);
    }

    public static Result<T> Conflict(string message)
    {
        return new Result<T>(false, default, message, 409);
    }

    public ApiResponse<T> ToApiResponse()
    {
        return IsSuccess
            ? ApiResponse<T>.Ok(Value!, Message, StatusCode)
            : ApiResponse<T>.Fail(Message, StatusCode, Errors);
    }
}
