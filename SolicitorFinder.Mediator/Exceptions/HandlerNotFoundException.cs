namespace SolicitorFinder.Mediator.Exceptions;

public sealed class HandlerNotFoundException : Exception
{
    public Type RequestType { get; }
    public Type? ResponseType { get; }

    public HandlerNotFoundException(Type requestType)
        : base($"Handler for request '{requestType.Name}' not found")
    {
        RequestType = requestType;
    }

    public HandlerNotFoundException(Type requestType, Type responseType)
        : base($"Handler for request '{requestType.Name}' with response '{responseType.Name}' not found")
    {
        RequestType = requestType;
        ResponseType = responseType;
    }
}
