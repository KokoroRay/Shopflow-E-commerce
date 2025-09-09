using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Value object representing an OTP (One-Time Password) code
/// </summary>
public sealed class OtpCode : IEquatable<OtpCode>
{
    private static readonly Regex OtpRegex = new(@"^\d{6}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the OTP code value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets when the OTP was generated
    /// </summary>
    public DateTime GeneratedAt { get; }

    /// <summary>
    /// Gets when the OTP expires
    /// </summary>
    public DateTime ExpiresAt { get; }

    /// <summary>
    /// Gets the default OTP expiration time in minutes
    /// </summary>
    public static readonly int DefaultExpirationMinutes = 15;

    /// <summary>
    /// Initializes a new instance of the OtpCode class
    /// </summary>
    /// <param name="value">The OTP code value</param>
    /// <param name="expirationMinutes">The expiration time in minutes (default: 15)</param>
    public OtpCode(string value, int expirationMinutes = 15)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("OTP code cannot be empty", nameof(value));

        var trimmedValue = value.Trim();
        if (!OtpRegex.IsMatch(trimmedValue))
            throw new ArgumentException("OTP code must be exactly 6 digits", nameof(value));

        if (expirationMinutes <= 0 || expirationMinutes > 60)
            throw new ArgumentException("Expiration minutes must be between 1 and 60", nameof(expirationMinutes));

        Value = trimmedValue;
        GeneratedAt = DateTime.UtcNow;
        ExpiresAt = GeneratedAt.AddMinutes(expirationMinutes);
    }

    /// <summary>
    /// Generates a new random 6-digit OTP code
    /// </summary>
    /// <param name="expirationMinutes">The expiration time in minutes (default: 15)</param>
    /// <returns>A new OtpCode instance</returns>
    public static OtpCode Generate(int expirationMinutes = 15)
    {
        var otpValue = GenerateRandomOtp();
        return new OtpCode(otpValue, expirationMinutes);
    }

    /// <summary>
    /// Checks if the OTP code is expired
    /// </summary>
    /// <returns>True if expired, false otherwise</returns>
    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }

    /// <summary>
    /// Checks if the OTP code is valid (not expired)
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid()
    {
        return !IsExpired();
    }

    /// <summary>
    /// Gets the remaining time until expiration
    /// </summary>
    /// <returns>TimeSpan representing remaining time, or TimeSpan.Zero if expired</returns>
    public TimeSpan GetRemainingTime()
    {
        var remaining = ExpiresAt - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Validates if a string is a valid OTP format
    /// </summary>
    /// <param name="otp">OTP string to validate</param>
    /// <returns>True if valid format, false otherwise</returns>
    public static bool IsValidFormat(string otp)
    {
        if (string.IsNullOrWhiteSpace(otp))
            return false;

        return OtpRegex.IsMatch(otp.Trim());
    }

    /// <summary>
    /// Generates a cryptographically secure random 6-digit OTP
    /// </summary>
    /// <returns>6-digit OTP string</returns>
    private static string GenerateRandomOtp()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);

        var number = Math.Abs(BitConverter.ToInt32(bytes, 0));
        var otp = (number % 1000000).ToString("D6");

        return otp;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object
    /// </summary>
    public bool Equals(OtpCode? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && GeneratedAt == other.GeneratedAt;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as OtpCode);
    }

    /// <summary>
    /// Returns the hash code for this instance
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, GeneratedAt);
    }

    /// <summary>
    /// Returns the string representation of the OTP code
    /// </summary>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Implicit conversion from OtpCode to string
    /// </summary>
    public static implicit operator string(OtpCode otpCode)
    {
        return otpCode.Value;
    }

    /// <summary>
    /// Explicit conversion from string to OtpCode
    /// </summary>
    public static explicit operator OtpCode(string value)
    {
        return new OtpCode(value);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(OtpCode? left, OtpCode? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(OtpCode? left, OtpCode? right)
    {
        return !Equals(left, right);
    }
}
