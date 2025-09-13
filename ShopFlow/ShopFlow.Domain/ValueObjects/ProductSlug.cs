using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Represents a product slug value object for marketplace SEO-friendly URLs
/// Supports Vietnamese marketplace with proper URL slug generation and validation
/// </summary>
public record ProductSlug
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 255;

    public string Value { get; }

    private ProductSlug(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a ProductSlug from a string with marketplace validation
    /// Ensures SEO-friendly URLs for Vietnamese marketplace
    /// </summary>
    /// <param name="slug">The product slug string</param>
    /// <returns>A valid ProductSlug instance</returns>
    /// <exception cref="ArgumentException">When slug is invalid</exception>
    public static ProductSlug FromString(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Product slug cannot be empty", nameof(slug));

        var trimmed = slug.Trim().ToLowerInvariant();

        if (trimmed.Length < MIN_LENGTH)
            throw new ArgumentException($"Product slug must be at least {MIN_LENGTH} characters long", nameof(slug));

        if (trimmed.Length > MAX_LENGTH)
            throw new ArgumentException($"Product slug cannot exceed {MAX_LENGTH} characters", nameof(slug));

        if (!IsValidSlugFormat(trimmed))
            throw new ArgumentException("Product slug must contain only lowercase letters, numbers, and hyphens, and cannot start or end with hyphen", nameof(slug));

        return new ProductSlug(trimmed);
    }

    /// <summary>
    /// Creates a ProductSlug from a ProductName by generating SEO-friendly slug
    /// Converts Vietnamese characters to URL-safe equivalents for marketplace SEO
    /// </summary>
    /// <param name="productName">The product name to convert</param>
    /// <returns>A SEO-friendly ProductSlug</returns>
    public static ProductSlug FromProductName(ProductName productName)
    {
        if (productName == null)
            throw new ArgumentNullException(nameof(productName));

        var slug = GenerateSlugFromName(productName.Value);
        return new ProductSlug(slug);
    }

    /// <summary>
    /// Generates a SEO-friendly slug from product name
    /// Handles Vietnamese character conversion for marketplace URLs
    /// </summary>
    private static string GenerateSlugFromName(string name)
    {
        // Convert to lowercase
        var slug = name.ToLowerInvariant();

        // Convert Vietnamese characters to ASCII equivalents for SEO
        slug = RemoveVietnameseAccents(slug);

        // Replace spaces and invalid characters with hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");

        // Remove leading and trailing hyphens
        slug = slug.Trim('-');

        // Ensure minimum length
        if (slug.Length < MIN_LENGTH)
        {
            slug = $"product-{DateTime.UtcNow.Ticks}";
        }

        // Ensure maximum length
        if (slug.Length > MAX_LENGTH)
        {
            slug = slug.Substring(0, MAX_LENGTH).TrimEnd('-');
        }

        return slug;
    }

    /// <summary>
    /// Removes Vietnamese accents and converts to ASCII for SEO-friendly URLs
    /// Essential for Vietnamese marketplace SEO optimization
    /// </summary>
    private static string RemoveVietnameseAccents(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                // Handle specific Vietnamese characters
                var replacement = c switch
                {
                    'đ' => 'd',
                    'Đ' => 'D',
                    _ => c
                };
                stringBuilder.Append(replacement);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Validates slug format for marketplace URLs
    /// Ensures SEO-friendly format: lowercase, alphanumeric, hyphens only
    /// </summary>
    private static bool IsValidSlugFormat(string slug)
    {
        // Must not start or end with hyphen
        if (slug.StartsWith('-') || slug.EndsWith('-'))
            return false;

        // Must contain only lowercase letters, numbers, and hyphens
        return Regex.IsMatch(slug, @"^[a-z0-9-]+$");
    }

    public override string ToString() => Value;

    public static implicit operator string(ProductSlug productSlug) => productSlug.Value;

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public virtual bool Equals(ProductSlug? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.Ordinal);
    }
}