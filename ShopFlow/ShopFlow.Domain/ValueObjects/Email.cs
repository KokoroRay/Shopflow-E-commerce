using System.Text.RegularExpressions;

namespace ShopFlow.Domain.ValueObjects;

public class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));
        
        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format", nameof(value));
        
        Value = value.ToLowerInvariant();
    }

    public static implicit operator string(Email email) => email.Value;
    
    public static explicit operator Email(string value) => new(value);

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Email);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static bool operator ==(Email? left, Email? right)
    {
        return EqualityComparer<Email>.Default.Equals(left, right);
    }

    public static bool operator !=(Email? left, Email? right)
    {
        return !(left == right);
    }
}
