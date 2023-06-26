namespace PeepoGuessrApi.Entities.Response;

public class ErrorDto<T> where T : new()
{
    public int Code { get; set; }
    public string Method { get; set; }
    public T? Request { get; set; }
    
    public ErrorDto(int code, string method, T? request)
    {
        Code = code;
        Method = method;
        Request = request;
    }
}