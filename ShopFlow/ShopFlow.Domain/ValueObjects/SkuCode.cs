using System.Globalization;
using System.Text;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Value object representing SKU code with Vietnamese marketplace validation
/// </summary>
public sealed class SkuCode : IEquatable<SkuCode>
{
    private const int MinLength = 3;
    private const int MaxLength = 50;

    /// <summary>
    /// Gets the SKU code value
    /// </summary>
    public string Value { get; }

    private SkuCode(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates SKU code with validation for Vietnamese marketplace standards
    /// </summary>
    /// <param name="value">SKU code value</param>
    /// <returns>SkuCode instance</returns>
    /// <exception cref="ArgumentException">When SKU code is invalid</exception>
    public static SkuCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SKU code cannot be empty", nameof(value));

        if (value.Length < MinLength)
            throw new ArgumentException($"SKU code must be at least {MinLength} characters", nameof(value));

        if (value.Length > MaxLength)
            throw new ArgumentException($"SKU code cannot exceed {MaxLength} characters", nameof(value));

        // Vietnamese marketplace: Allow alphanumeric, hyphens, underscores
        if (!IsValidFormat(value))
            throw new ArgumentException("SKU code can only contain letters, numbers, hyphens, and underscores", nameof(value));

        return new SkuCode(value.ToUpperInvariant());
    }

    /// <summary>
    /// Generates automatic SKU code based on product name and variant options
    /// </summary>
    /// <param name="productName">Product name in Vietnamese/English</param>
    /// <param name="variantOptions">Variant options (color, size, etc.)</param>
    /// <param name="sequence">Sequence number for uniqueness</param>
    /// <returns>Auto-generated SkuCode</returns>
    public static SkuCode GenerateAutomatic(string productName, Dictionary<string, string>? variantOptions = null, int sequence = 1)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required for auto-generation", nameof(productName));

        var sanitized = SanitizeForSku(productName);
        var prefix = sanitized.Length > 8 ? sanitized[..8] : sanitized;

        var variantPart = string.Empty;
        if (variantOptions?.Count > 0)
        {
            variantPart = string.Join("-", variantOptions.Values
                .Select(SanitizeForSku)
                .Select(v => v.Length > 4 ? v[..4] : v));
        }

        var sequencePart = sequence.ToString("D3", CultureInfo.InvariantCulture);
        var generated = $"{prefix}-{variantPart}-{sequencePart}".Replace("--", "-", StringComparison.Ordinal).Trim('-');

        return Create(generated);
    }

    private static string SanitizeForSku(string input)
    {
        // Remove Vietnamese diacritics and special characters
        var normalized = input.Normalize(NormalizationForm.FormD);
        var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
        var ascii = new string(chars.ToArray());

        // Keep only alphanumeric and convert to uppercase
        return new string(ascii.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToUpperInvariant();
    }

    private static bool IsValidFormat(string value)
    {
        return value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
    }

    /// <summary>
    /// Determines whether the specified SkuCode is equal to the current SkuCode
    /// </summary>
    /// <param name="other">The SkuCode to compare with the current SkuCode</param>
    /// <returns>true if the specified SkuCode is equal to the current SkuCode; otherwise, false</returns>
    public bool Equals(SkuCode? other)
    {
        return other is not null && Value == other.Value;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current SkuCode
    /// </summary>
    /// <param name="obj">The object to compare with the current SkuCode</param>
    /// <returns>true if the specified object is equal to the current SkuCode; otherwise, false</returns>
    public override bool Equals(object? obj)
    {
        return obj is SkuCode other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this SkuCode
    /// </summary>
    /// <returns>A hash code for the current SkuCode</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns the SKU code value as a string
    /// </summary>
    /// <returns>The SKU code value</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a SkuCode to a string
    /// </summary>
    /// <param name="skuCode">The SkuCode to convert</param>
    /// <returns>The SKU code value as a string</returns>
    /// <exception cref="ArgumentNullException">When skuCode is null</exception>
    public static implicit operator string(SkuCode skuCode)
    {
        ArgumentNullException.ThrowIfNull(skuCode);
        return skuCode.Value;
    }

    /// <summary>
    /// Determines whether two SkuCode instances are equal
    /// </summary>
    /// <param name="left">The first SkuCode to compare</param>
    /// <param name="right">The second SkuCode to compare</param>
    /// <returns>true if the SkuCode instances are equal; otherwise, false</returns>
    public static bool operator ==(SkuCode? left, SkuCode? right)
    {
        return EqualityComparer<SkuCode>.Default.Equals(left, right);
    }

    /// <summary>
    /// Determines whether two SkuCode instances are not equal
    /// </summary>
    /// <param name="left">The first SkuCode to compare</param>
    /// <param name="right">The second SkuCode to compare</param>
    /// <returns>true if the SkuCode instances are not equal; otherwise, false</returns>
    public static bool operator !=(SkuCode? left, SkuCode? right)
    {
        return !(left == right);
    }
}