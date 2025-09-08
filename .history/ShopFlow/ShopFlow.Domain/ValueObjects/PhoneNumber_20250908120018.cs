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

    public string Value { get; }

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

    public static implicit operator string(PhoneNumber? phone) 
    {
        ArgumentNullException.ThrowIfNull(phone);
        return phone.Value;
    }
    
    public static explicit operator PhoneNumber(string value) => new(value);

    /// <summary>
    /// Converts string to PhoneNumber - alternative to explicit operator
    /// </summary>
    public static PhoneNumber FromString(string value) => new(value);

    /// <summary>
    /// Converts PhoneNumber to string - alternative to implicit operator
    /// </summary>
    public string ToPhoneNumber() => Value;

    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PhoneNumber);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static bool operator ==(PhoneNumber? left, PhoneNumber? right)
    {
        return EqualityComparer<PhoneNumber>.Default.Equals(left, right);
    }

    public static bool operator !=(PhoneNumber? left, PhoneNumber? right)
    {
        return !(left == right);
    }
}
