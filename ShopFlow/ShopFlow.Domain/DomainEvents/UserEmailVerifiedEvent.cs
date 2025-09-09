namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a user's email is verified
/// </summary>
public sealed class UserEmailVerifiedEvent : DomainEvent
{
    /// <summary>
    /// Gets the ID of the user whose email was verified
    /// </summary>
    public long UserId { get; }

    /// <summary>
    /// Gets the verified email address
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Initializes a new instance of the UserEmailVerifiedEvent
    /// </summary>
    /// <param name="userId">The ID of the user whose email was verified</param>
    /// <param name="email">The verified email address</param>
    public UserEmailVerifiedEvent(long userId, string email)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero", nameof(userId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        UserId = userId;
        Email = email;
    }

    /// <summary>
    /// Returns a string representation of the event
    /// </summary>
    public override string ToString()
    {
        return $"UserEmailVerifiedEvent: UserId={UserId}, Email={Email}";
    }
}
