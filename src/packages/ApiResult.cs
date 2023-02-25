namespace Arma.Demo.Core;

public class ApiResult<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public bool Error { get; set; }

    public ApiResult(string action, Exception ex)
    {
        Error = true;
        Message = $"{typeof(T)}.{action}: {ex.Message}";
    }

    public ApiResult(T data, string message = "Operation completed successfully")
    {
        Data = data;
        Message = message;
    }

    public ApiResult(ValidationResult validation)
    {
        Error = true;
        Message = validation.Message;
    }
}