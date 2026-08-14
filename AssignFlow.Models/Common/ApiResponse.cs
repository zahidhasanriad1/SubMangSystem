namespace AssignFlow.Models.Common;

/// <summary>
/// Provides the consistent response envelope returned by application endpoints.
/// </summary>
/// <typeparam name="T">The response payload type.</typeparam>
public class ApiResponse<T>
{
    public ApiResponse(T data, bool success = true, string message = "")
    {
        Data = data;
        Success = success;
        Message = message;
    }

    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
}
