namespace ShopFlow.Domain.Exceptions;

/// <summary>
/// Base domain exception for business rule violations
/// </summary>
public class DomainException : Exception
{
    public DomainException() { }
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}