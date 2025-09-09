namespace ShopFlow.Application.Exceptions;

/// <summary>
/// Exception thrown when attempting to register with an email that already exists
/// </summary>
public sealed class EmailAlreadyExistsException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the EmailAlreadyExistsException class
    /// </summary>
    /// <param name="email">The email that already exists</param>
    public EmailAlreadyExistsException(string email)
        : base($"Email '{email}' is already registered")
    {
        Email = email;
    }

    /// <summary>
    /// Gets the email that already exists
    /// </summary>
    public string Email { get; }
}
