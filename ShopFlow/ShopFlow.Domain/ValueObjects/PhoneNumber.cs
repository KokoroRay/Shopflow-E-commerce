using System.Text.RegularExpressions;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Value object representing a Vietnamese phone number
/// </summary>
public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    private static readonly Regex PhoneRegex = new(
        @"^(\+84|84|0)?[1-9][0-9]{8}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Gets the normalized phone number value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the PhoneNumber class
    /// </summary>
    /// <param name="value">The phone number string to validate and normalize</param>
    /// <exception cref="ArgumentException">Thrown when phone number is invalid</exception>
    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));

        if (!PhoneRegex.IsMatch(value))
            throw new ArgumentException("Invalid phone number format", nameof(value));

        Value = NormalizePhoneNumber(value);
    }

    private static string NormalizePhoneNumber(string phone)
    {
        // Remove all non-digit characters
        var digits = new string(phone.Where(char.IsDigit).ToArray());

        // If starts with 84, remove it
        if (digits.StartsWith("84", StringComparison.Ordinal) && digits.Length == 11)
        {
            digits = digits.Substring(2);
        }

        // If starts with 0, remove it
        if (digits.StartsWith('0'))
        {
            digits = digits.Substring(1);
        }

        return digits;
    }

    /// <summary>
    /// Implicitly converts PhoneNumber to string
    /// </summary>
    /// <param name="phone">The phone number to convert</param>
    /// <returns>The phone number string value</returns>
    public static implicit operator string(PhoneNumber? phone)
    {
        ArgumentNullException.ThrowIfNull(phone);
        return phone.Value;
    }

    /// <summary>
    /// Explicitly converts string to PhoneNumber
    /// </summary>
    /// <param name="value">The string value to convert</param>
    /// <returns>A new PhoneNumber instance</returns>
    public static explicit operator PhoneNumber(string value) => new(value);

    /// <summary>
    /// Converts string to PhoneNumber - alternative to explicit operator
    /// </summary>
    /// <param name="value">The string value to convert</param>
    /// <returns>A new PhoneNumber instance</returns>
    public static PhoneNumber FromString(string value) => new(value);

    /// <summary>
    /// Converts PhoneNumber to string - alternative to implicit operator
    /// </summary>
    /// <returns>The phone number string value</returns>
    public string ToPhoneNumber() => Value;

    /// <summary>
    /// Determines whether the specified PhoneNumber is equal to the current PhoneNumber
    /// </summary>
    /// <param name="other">The PhoneNumber to compare with the current PhoneNumber</param>
    /// <returns>true if the specified PhoneNumber is equal to the current PhoneNumber; otherwise, false</returns>
    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current PhoneNumber
    /// </summary>
    /// <param name="obj">The object to compare with the current PhoneNumber</param>
    /// <returns>true if the specified object is equal to the current PhoneNumber; otherwise, false</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as PhoneNumber);
    }

    /// <summary>
    /// Serves as the hash function for PhoneNumber
    /// </summary>
    /// <returns>A hash code for the current PhoneNumber</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns a string representation of the phone number
    /// </summary>
    /// <returns>The phone number string value</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Determines whether two PhoneNumber instances are equal
    /// </summary>
    /// <param name="left">The first PhoneNumber to compare</param>
    /// <param name="right">The second PhoneNumber to compare</param>
    /// <returns>true if the PhoneNumbers are equal; otherwise, false</returns>
    public static bool operator ==(PhoneNumber? left, PhoneNumber? right)
    {
        return EqualityComparer<PhoneNumber>.Default.Equals(left, right);
    }

    /// <summary>
    /// Determines whether two PhoneNumber instances are not equal
    /// </summary>
    /// <param name="left">The first PhoneNumber to compare</param>
    /// <param name="right">The second PhoneNumber to compare</param>
    /// <returns>true if the PhoneNumbers are not equal; otherwise, false</returns>
    public static bool operator !=(PhoneNumber? left, PhoneNumber? right)
    {
        return !(left == right);
    }
}
