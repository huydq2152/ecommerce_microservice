using System.Text.Json.Serialization;

namespace Shared.SeedWork;

public class ApiResult<T>
{
    [JsonConstructor]
    public ApiResult(bool isSucceeded, string message = null)
    {
        isSucceeded = isSucceeded;
        Message = message;
    }

    public ApiResult(bool isSucceeded, T data, string message = null)
    {
        IsSucceeded = isSucceeded;
        Data = data;
        Message = message;
    }

    public bool IsSucceeded { get; set; }
    public string Message { get; set; }
    public T Data { get; }
}