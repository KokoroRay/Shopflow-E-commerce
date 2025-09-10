namespace ShopFlow.Application.Exceptions;

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message) { }
    public AuthenticationException(string message, Exception innerException) : base(message, innerException) { }
}
