namespace SolicitorFinder.Mediator.Exceptions;

public sealed class MediatorException : Exception
{
    public MediatorException() { }
    public MediatorException(string message) : base(message) { }
    public MediatorException(string message, Exception innerException) : base(message, innerException) { }
}
