namespace Application.ViewModel;

public class Result
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Object { get; set; }
    
    public static Result SuccessResult(string? msg = null)
    {
        return new Result { Success = true, Message = msg};
    }
    
    public static Result FailResult(string errors)
    {
        return new Result { Success = false, Message = errors };
    }
    
    public static Result ObjectResult(object obj, string? msg = null)
    {
        return new Result { Success = true, Message = msg, Object = obj};
    }
}