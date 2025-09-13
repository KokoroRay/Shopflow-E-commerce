using System;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Represents a product name value object with marketplace validation rules
/// Supports Vietnamese marketplace requirements with both VN and EN languages
/// </summary>
public record ProductName
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 255;

    public string Value { get; }

    private ProductName(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a ProductName from a display name with marketplace validation
    /// Supports Vietnamese and English characters for multi-language marketplace
    /// </summary>
    /// <param name="displayName">The product display name</param>
    /// <returns>A valid ProductName instance</returns>
    /// <exception cref="ArgumentException">When display name is invalid</exception>
    public static ProductName FromDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Product name cannot be empty", nameof(displayName));

        var trimmed = displayName.Trim();

        if (trimmed.Length < MIN_LENGTH)
            throw new ArgumentException($"Product name must be at least {MIN_LENGTH} characters long", nameof(displayName));

        if (trimmed.Length > MAX_LENGTH)
            throw new ArgumentException($"Product name cannot exceed {MAX_LENGTH} characters", nameof(displayName));

        // Validate for marketplace - allow Vietnamese characters, alphanumeric, spaces, and common symbols
        if (!IsValidMarketplaceName(trimmed))
            throw new ArgumentException("Product name contains invalid characters for marketplace", nameof(displayName));

        return new ProductName(trimmed);
    }

    /// <summary>
    /// Validates if a name is suitable for Vietnamese marketplace
    /// Allows Vietnamese characters, Latin characters, numbers, spaces, and common symbols
    /// </summary>
    private static bool IsValidMarketplaceName(string name)
    {
        // Allow Vietnamese characters (Unicode blocks), Latin characters, numbers, spaces, and marketplace-common symbols
        foreach (char c in name)
        {
            if (char.IsLetterOrDigit(c) ||
                char.IsWhiteSpace(c) ||
                c == '-' || c == '_' || c == '.' || c == ',' || c == '(' || c == ')' ||
                c == '[' || c == ']' || c == '/' || c == '+' || c == '&' ||
                IsVietnameseCharacter(c))
            {
                continue;
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if character is a Vietnamese character
    /// Covers Vietnamese Unicode ranges for accented characters
    /// </summary>
    private static bool IsVietnameseCharacter(char c)
    {
        // Vietnamese accented characters ranges
        return (c >= 0x00C0 && c <= 0x024F) ||  // Latin Extended-A and B
               (c >= 0x1E00 && c <= 0x1EFF);   // Latin Extended Additional (Vietnamese)
    }

    public override string ToString() => Value;

    public static implicit operator string(ProductName productName) => productName.Value;

    public static bool operator ==(ProductName? left, ProductName? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Value.Equals(right.Value, StringComparison.OrdinalIgnoreCase);
    }

    public static bool operator !=(ProductName? left, ProductName? right) => !(left == right);

    public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

    public virtual bool Equals(ProductName? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }
}