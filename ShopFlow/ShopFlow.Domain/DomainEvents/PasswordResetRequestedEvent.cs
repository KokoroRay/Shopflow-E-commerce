using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a user requests a password reset
/// </summary>
public sealed class PasswordResetRequestedEvent : DomainEvent
{
    /// <summary>
    /// Gets the ID of the user who requested password reset
    /// </summary>
    public long UserId { get; }

    /// <summary>
    /// Gets the email address of the user
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Gets the OTP code for password reset
    /// </summary>
    public string OtpCode { get; }

    /// <summary>
    /// Gets the expiration time for the OTP
    /// </summary>
    public DateTime ExpiresAt { get; }

    /// <summary>
    /// Initializes a new instance of the PasswordResetRequestedEvent
    /// </summary>
    /// <param name="userId">The ID of the user requesting password reset</param>
    /// <param name="email">The email address of the user</param>
    /// <param name="otpCode">The OTP code for verification</param>
    /// <param name="expiresAt">The expiration time for the OTP</param>
    public PasswordResetRequestedEvent(long userId, string email, string otpCode, DateTime expiresAt)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero", nameof(userId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
        if (string.IsNullOrWhiteSpace(otpCode))
            throw new ArgumentException("OTP code cannot be null or empty", nameof(otpCode));
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration time must be in the future", nameof(expiresAt));

        UserId = userId;
        Email = email;
        OtpCode = otpCode;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Initializes a new instance with Email value object
    /// </summary>
    /// <param name="userId">The ID of the user requesting password reset</param>
    /// <param name="email">The email value object of the user</param>
    /// <param name="otpCode">The OTP code for verification</param>
    /// <param name="expiresAt">The expiration time for the OTP</param>
    public PasswordResetRequestedEvent(long userId, Email email, string otpCode, DateTime expiresAt)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero", nameof(userId));
        if (email is null)
            throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrWhiteSpace(otpCode))
            throw new ArgumentException("OTP code cannot be null or empty", nameof(otpCode));
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration time must be in the future", nameof(expiresAt));

        UserId = userId;
        Email = email.Value;
        OtpCode = otpCode;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Returns a string representation of the event
    /// </summary>
    public override string ToString()
    {
        return $"PasswordResetRequestedEvent: UserId={UserId}, Email={Email}, ExpiresAt={ExpiresAt:yyyy-MM-dd HH:mm:ss}";
    }
}
