namespace Core.Domain;

/// <summary>
/// 
/// </summary>
public class ResponseObject<T>(ResponseEnum responseEnum, T t)
{
    private T? _value = t;
    private ResponseEnum _responseEnum = responseEnum;

    public ResponseEnum Response => _responseEnum;

    public T? GetResponse => _value ?? default;
}