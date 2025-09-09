namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a user successfully resets their password
/// </summary>
public sealed class PasswordResetConfirmedEvent : DomainEvent
{
    /// <summary>
    /// Gets the ID of the user who reset their password
    /// </summary>
    public long UserId { get; }

    /// <summary>
    /// Gets the email address of the user
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Gets the timestamp when the password was reset
    /// </summary>
    public DateTime ResetAt { get; }

    /// <summary>
    /// Initializes a new instance of the PasswordResetConfirmedEvent
    /// </summary>
    /// <param name="userId">The ID of the user who reset their password</param>
    /// <param name="email">The email address of the user</param>
    /// <param name="resetAt">The timestamp when the password was reset</param>
    public PasswordResetConfirmedEvent(long userId, string email, DateTime resetAt)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero", nameof(userId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        UserId = userId;
        Email = email;
        ResetAt = resetAt;
    }

    /// <summary>
    /// Returns a string representation of the event
    /// </summary>
    public override string ToString()
    {
        return $"PasswordResetConfirmedEvent: UserId={UserId}, Email={Email}, ResetAt={ResetAt:yyyy-MM-dd HH:mm:ss}";
    }
}
