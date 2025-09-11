using System.Text.RegularExpressions;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Value object representing a category URL slug with validation
/// </summary>
public sealed class CategorySlug : IEquatable<CategorySlug>
{
    private static readonly Regex SlugRegex = new(
        @"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.Compiled);

    /// <summary>
    /// Gets the normalized category slug value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the CategorySlug class
    /// </summary>
    /// <param name="value">The category slug string to validate and normalize</param>
    /// <exception cref="ArgumentException">Thrown when category slug is invalid</exception>
    public CategorySlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Category slug cannot be empty", nameof(value));

        var trimmedValue = value.Trim().ToLowerInvariant();

        if (trimmedValue.Length < 2)
            throw new ArgumentException("Category slug must be at least 2 characters long", nameof(value));

        if (trimmedValue.Length > 100)
            throw new ArgumentException("Category slug cannot exceed 100 characters", nameof(value));

        if (!SlugRegex.IsMatch(trimmedValue))
            throw new ArgumentException("Category slug must contain only lowercase letters, numbers, and hyphens", nameof(value));

        Value = trimmedValue;
    }

    /// <summary>
    /// Creates a CategorySlug from a name by converting it to a slug format
    /// </summary>
    /// <param name="name">The name to convert to slug</param>
    /// <returns>A new CategorySlug instance</returns>
    public static CategorySlug FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        // Convert to lowercase and replace spaces with hyphens
        var slug = name.Trim()
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");

        // Remove invalid characters
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

        // Remove multiple consecutive hyphens
        slug = Regex.Replace(slug, @"-+", "-");

        // Remove leading/trailing hyphens
        slug = slug.Trim('-');

        if (string.IsNullOrEmpty(slug))
            throw new ArgumentException("Cannot create valid slug from the provided name", nameof(name));

        return new CategorySlug(slug);
    }

    /// <summary>
    /// Creates a CategorySlug from a CategoryName
    /// </summary>
    /// <param name="categoryName">The category name to convert to slug</param>
    /// <returns>A new CategorySlug instance</returns>
    public static CategorySlug FromCategoryName(CategoryName categoryName)
    {
        if (categoryName == null)
            throw new ArgumentNullException(nameof(categoryName));

        return FromName(categoryName.Value);
    }

    /// <summary>
    /// Returns the string representation of the category slug
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Determines whether two CategorySlug instances are equal
    /// </summary>
    public bool Equals(CategorySlug? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether two CategorySlug instances are equal
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is CategorySlug other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this CategorySlug
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Determines whether two CategorySlug instances are equal
    /// </summary>
    public static bool operator ==(CategorySlug? left, CategorySlug? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Determines whether two CategorySlug instances are not equal
    /// </summary>
    public static bool operator !=(CategorySlug? left, CategorySlug? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// Validates if a string can be used as a category slug
    /// </summary>
    /// <param name="value">The string to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        var trimmed = value.Trim().ToLowerInvariant();
        return trimmed.Length >= 2 &&
               trimmed.Length <= 100 &&
               SlugRegex.IsMatch(trimmed);
    }
}