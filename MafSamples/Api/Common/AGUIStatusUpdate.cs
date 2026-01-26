namespace Api.Common;

public class AGUISnapshot<T>(string type, T data)
{
    public string Type { get; init; } = type;

    public T Data { get; init; } = data;
}

public class AGUIStatusUpdate(string status, string message)
{
    public string Type { get; init; } = "StatusUpdate";
    
    public string Status { get; init; } = status;
        
    public string Message { get; init; } = message;
}