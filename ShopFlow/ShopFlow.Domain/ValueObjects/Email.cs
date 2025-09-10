using System.Text.RegularExpressions;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Value object representing an email address with validation
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the normalized email value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the Email class
    /// </summary>
    /// <param name="value">The email string to validate and normalize</param>
    /// <exception cref="ArgumentException">Thrown when email is invalid</exception>
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        var trimmedValue = value.Trim();
        if (trimmedValue.Length > 254) // RFC 5321 limit
            throw new ArgumentException("Email address is too long", nameof(value));

        if (!EmailRegex.IsMatch(trimmedValue))
            throw new ArgumentException("Invalid email format", nameof(value));

        Value = trimmedValue.ToLowerInvariant();
    }

    /// <summary>
    /// Validates if a string is a valid email format
    /// </summary>
    /// <param name="email">Email string to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var trimmedEmail = email.Trim();
        return trimmedEmail.Length <= 254 && EmailRegex.IsMatch(trimmedEmail);
    }

    /// <summary>
    /// Creates an Email from a string value
    /// </summary>
    /// <param name="value">The email string</param>
    /// <returns>Email instance</returns>
    public static Email FromString(string value) => new(value);

    /// <summary>
    /// Converts Email to string implicitly
    /// </summary>
    /// <param name="email">Email instance</param>
    /// <returns>String representation</returns>
    public static implicit operator string(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);
        return email.Value;
    }

    /// <summary>
    /// Converts string to Email explicitly
    /// </summary>
    /// <param name="value">String value</param>
    /// <returns>Email instance</returns>
    public static explicit operator Email(string value) => new(value);

    /// <summary>
    /// Determines whether this instance and another Email are equal
    /// </summary>
    /// <param name="other">Email to compare with</param>
    /// <returns>True if equal, false otherwise</returns>
    public bool Equals(Email? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    /// <summary>
    /// Determines whether this instance and the specified object are equal
    /// </summary>
    /// <param name="obj">Object to compare with</param>
    /// <returns>True if equal, false otherwise</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Email);
    }

    /// <summary>
    /// Gets the hash code for this instance
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns the string representation of the email
    /// </summary>
    /// <returns>Email value as string</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Determines whether two Email instances are equal
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns>True if equal, false otherwise</returns>
    public static bool operator ==(Email? left, Email? right)
    {
        return EqualityComparer<Email>.Default.Equals(left, right);
    }

    /// <summary>
    /// Determines whether two Email instances are not equal
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns>True if not equal, false otherwise</returns>
    public static bool operator !=(Email? left, Email? right)
    {
        return !(left == right);
    }
}
