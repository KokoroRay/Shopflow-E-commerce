using System.Text.RegularExpressions;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Value object representing a category name with validation
/// </summary>
public sealed class CategoryName : IEquatable<CategoryName>
{
    private static readonly Regex NameRegex = new(
        @"^[a-zA-Z0-9\s\-_&\.\/\(\)]{1,100}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the normalized category name value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the CategoryName class
    /// </summary>
    /// <param name="value">The category name string to validate and normalize</param>
    /// <exception cref="ArgumentException">Thrown when category name is invalid</exception>
    public CategoryName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Category name cannot be empty", nameof(value));

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < 2)
            throw new ArgumentException("Category name must be at least 2 characters long", nameof(value));

        if (trimmedValue.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters", nameof(value));

        if (!NameRegex.IsMatch(trimmedValue))
            throw new ArgumentException("Category name contains invalid characters", nameof(value));

        Value = trimmedValue;
    }

    /// <summary>
    /// Implicitly converts a string to CategoryName
    /// </summary>
    public static implicit operator CategoryName(string value) => new(value);

    /// <summary>
    /// Implicitly converts CategoryName to string
    /// </summary>
    public static implicit operator string(CategoryName categoryName) => categoryName.Value;

    /// <summary>
    /// Returns the string representation of the category name
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Determines whether two CategoryName instances are equal
    /// </summary>
    public bool Equals(CategoryName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether two CategoryName instances are equal
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is CategoryName other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this CategoryName
    /// </summary>
    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    }

    /// <summary>
    /// Determines whether two CategoryName instances are equal
    /// </summary>
    public static bool operator ==(CategoryName? left, CategoryName? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Determines whether two CategoryName instances are not equal
    /// </summary>
    public static bool operator !=(CategoryName? left, CategoryName? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// Creates a CategoryName from a display name by normalizing it
    /// </summary>
    /// <param name="displayName">The display name to normalize</param>
    /// <returns>A new CategoryName instance</returns>
    public static CategoryName FromDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));

        // Normalize the display name: trim, collapse multiple spaces
        var normalized = Regex.Replace(displayName.Trim(), @"\s+", " ");
        return new CategoryName(normalized);
    }

    /// <summary>
    /// Validates if a string can be used as a category name
    /// </summary>
    /// <param name="value">The string to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        var trimmed = value.Trim();
        return trimmed.Length >= 2 &&
               trimmed.Length <= 100 &&
               NameRegex.IsMatch(trimmed);
    }
}