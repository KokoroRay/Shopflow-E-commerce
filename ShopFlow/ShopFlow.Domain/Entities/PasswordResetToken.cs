using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Domain.Entities;

/// <summary>
/// Entity representing a password reset token with OTP verification
/// </summary>
public class PasswordResetToken
{
    /// <summary>
    /// Gets the unique identifier for the password reset token
    /// </summary>
    public long Id { get; private set; }

    /// <summary>
    /// Gets the ID of the user requesting password reset
    /// </summary>
    public long UserId { get; private set; }

    /// <summary>
    /// Gets the email address for which the reset was requested
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Gets the OTP code for verification
    /// </summary>
    public OtpCode OtpCode { get; private set; }

    /// <summary>
    /// Gets when the token was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets when the token expires
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Gets whether the token has been used
    /// </summary>
    public bool IsUsed { get; private set; }

    /// <summary>
    /// Gets when the token was used (if applicable)
    /// </summary>
    public DateTime? UsedAt { get; private set; }

    /// <summary>
    /// Gets the number of verification attempts
    /// </summary>
    public int AttemptCount { get; private set; }

    /// <summary>
    /// Gets the maximum allowed attempts before token is locked
    /// </summary>
    public static readonly int MaxAttempts = 5;

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private PasswordResetToken() { }

    /// <summary>
    /// Initializes a new instance of the PasswordResetToken class
    /// </summary>
    /// <param name="userId">The ID of the user requesting password reset</param>
    /// <param name="email">The email address for the reset request</param>
    /// <param name="otpCode">The OTP code for verification</param>
    public PasswordResetToken(long userId, Email email, OtpCode otpCode)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero", nameof(userId));

        UserId = userId;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        OtpCode = otpCode ?? throw new ArgumentNullException(nameof(otpCode));
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = otpCode.ExpiresAt;
        IsUsed = false;
        AttemptCount = 0;
    }

    /// <summary>
    /// Checks if the token is valid for use
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid()
    {
        return !IsExpired() && !IsUsed && !IsLocked() && OtpCode.IsValid();
    }

    /// <summary>
    /// Checks if the token is expired
    /// </summary>
    /// <returns>True if expired, false otherwise</returns>
    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }

    /// <summary>
    /// Checks if the token is locked due to too many attempts
    /// </summary>
    /// <returns>True if locked, false otherwise</returns>
    public bool IsLocked()
    {
        return AttemptCount >= MaxAttempts;
    }

    /// <summary>
    /// Verifies the provided OTP code against this token
    /// </summary>
    /// <param name="providedOtp">The OTP code to verify</param>
    /// <returns>True if the OTP matches and token is valid</returns>
    public bool VerifyOtp(string providedOtp)
    {
        if (string.IsNullOrWhiteSpace(providedOtp))
            return false;

        // Increment attempt count regardless of outcome for security
        AttemptCount++;

        if (!IsValid())
            return false;

        return OtpCode.Value == providedOtp.Trim();
    }

    /// <summary>
    /// Marks the token as used
    /// </summary>
    public void MarkAsUsed()
    {
        if (IsUsed)
            throw new InvalidOperationException("Token has already been used");

        if (!IsValid())
            throw new InvalidOperationException("Cannot mark invalid token as used");

        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the remaining time until expiration
    /// </summary>
    /// <returns>TimeSpan representing remaining time</returns>
    public TimeSpan GetRemainingTime()
    {
        var remaining = ExpiresAt - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Gets the remaining number of attempts
    /// </summary>
    /// <returns>Number of remaining attempts</returns>
    public int GetRemainingAttempts()
    {
        return Math.Max(0, MaxAttempts - AttemptCount);
    }

    /// <summary>
    /// Creates a new password reset token for the specified user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="email">The email address</param>
    /// <param name="expirationMinutes">The expiration time in minutes (default: 15)</param>
    /// <returns>A new PasswordResetToken instance</returns>
    public static PasswordResetToken Create(long userId, Email email, int expirationMinutes = 15)
    {
        var otpCode = OtpCode.Generate(expirationMinutes);
        return new PasswordResetToken(userId, email, otpCode);
    }

    /// <summary>
    /// Returns a string representation of the token
    /// </summary>
    public override string ToString()
    {
        return $"PasswordResetToken: UserId={UserId}, Email={Email}, ExpiresAt={ExpiresAt:yyyy-MM-dd HH:mm:ss}, IsUsed={IsUsed}";
    }
}
