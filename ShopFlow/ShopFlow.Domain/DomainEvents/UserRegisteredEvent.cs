using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a user registers successfully
/// </summary>
public sealed class UserRegisteredEvent : DomainEvent
{
    /// <summary>
    /// Gets the ID of the newly registered user
    /// </summary>
    public long UserId { get; }

    /// <summary>
    /// Gets the email address of the registered user
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Initializes a new instance of the UserRegisteredEvent
    /// </summary>
    /// <param name="userId">The ID of the newly registered user</param>
    /// <param name="email">The email address of the registered user as string</param>
    public UserRegisteredEvent(long userId, string email)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero", nameof(userId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        UserId = userId;
        Email = email;
    }

    /// <summary>
    /// Initializes a new instance of the UserRegisteredEvent with Email value object
    /// </summary>
    /// <param name="userId">The ID of the newly registered user</param>
    /// <param name="email">The email value object of the registered user</param>
    public UserRegisteredEvent(long userId, Email email)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero", nameof(userId));
        if (email is null)
            throw new ArgumentNullException(nameof(email));

        UserId = userId;
        Email = email.Value;
    }

    /// <summary>
    /// Returns a string representation of the event
    /// </summary>
    public override string ToString()
    {
        return $"UserRegisteredEvent: UserId={UserId}, Email={Email}";
    }
}
