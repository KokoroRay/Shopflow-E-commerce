using System;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Represents a product description value object with Vietnamese marketplace support
/// Supports both Vietnamese and English content for multi-language marketplace
/// </summary>
public record ProductDescription
{
    public const int MIN_LENGTH = 10;
    public const int MAX_LENGTH = 5000;

    public string Value { get; }

    private ProductDescription(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a ProductDescription from content with marketplace validation
    /// Supports Vietnamese and English text for multi-language marketplace
    /// </summary>
    /// <param name="content">The product description content</param>
    /// <returns>A valid ProductDescription instance</returns>
    /// <exception cref="ArgumentException">When content is invalid</exception>
    public static ProductDescription FromContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Product description cannot be empty", nameof(content));

        var trimmed = content.Trim();

        if (trimmed.Length < MIN_LENGTH)
            throw new ArgumentException($"Product description must be at least {MIN_LENGTH} characters long", nameof(content));

        if (trimmed.Length > MAX_LENGTH)
            throw new ArgumentException($"Product description cannot exceed {MAX_LENGTH} characters", nameof(content));

        // Validate for marketplace - allow Vietnamese characters, alphanumeric, spaces, and common symbols
        if (!IsValidMarketplaceDescription(trimmed))
            throw new ArgumentException("Product description contains invalid characters for marketplace", nameof(content));

        return new ProductDescription(trimmed);
    }

    /// <summary>
    /// Validates if description content is suitable for Vietnamese marketplace
    /// Allows Vietnamese characters, Latin characters, numbers, spaces, and common symbols
    /// </summary>
    private static bool IsValidMarketplaceDescription(string content)
    {
        // Allow Vietnamese characters (Unicode blocks), Latin characters, numbers, spaces, and marketplace-common symbols
        foreach (char c in content)
        {
            if (char.IsLetterOrDigit(c) ||
                char.IsWhiteSpace(c) ||
                char.IsPunctuation(c) ||
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

    public static implicit operator string(ProductDescription productDescription) => productDescription.Value;

    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

    public virtual bool Equals(ProductDescription? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }
}