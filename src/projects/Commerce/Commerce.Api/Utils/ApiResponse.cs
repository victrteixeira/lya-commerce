namespace Commerce.Api.Utils;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool Succeeded { get; set; }
    public string? Message { get; set; }

    public static ApiResponse<T> Fail(string errorMessage) => new ApiResponse<T>
        { Succeeded = false, Message = errorMessage };

    public static ApiResponse<T> Success(T? data, string successMessage) => new ApiResponse<T>
        { Data = data, Succeeded = true, Message = successMessage };
}