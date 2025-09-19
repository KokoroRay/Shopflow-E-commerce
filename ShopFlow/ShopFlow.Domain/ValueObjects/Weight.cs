using System.Globalization;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Value object representing product weight with Vietnamese marketplace validation
/// </summary>
public sealed class Weight : IEquatable<Weight>
{
    private const int MinWeightGrams = 1; // 1 gram minimum
    private const int MaxWeightGrams = 50_000_000; // 50 tons maximum

    /// <summary>
    /// Gets the weight in grams
    /// </summary>
    public int Grams { get; }

    /// <summary>
    /// Gets the weight in kilograms
    /// </summary>
    public decimal Kilograms => Grams / 1000m;

    /// <summary>
    /// Gets the weight in pounds (for international compatibility)
    /// </summary>
    public decimal Pounds => Grams / 453.592m;

    private Weight(int grams)
    {
        Grams = grams;
    }

    /// <summary>
    /// Creates weight with validation for Vietnamese marketplace standards
    /// </summary>
    /// <param name="grams">Weight in grams</param>
    /// <returns>Weight instance</returns>
    /// <exception cref="ArgumentException">When weight is invalid</exception>
    public static Weight Create(int grams)
    {
        if (grams < MinWeightGrams)
            throw new ArgumentException($"Weight must be at least {MinWeightGrams} gram", nameof(grams));

        if (grams > MaxWeightGrams)
            throw new ArgumentException($"Weight cannot exceed {MaxWeightGrams} grams (50 tons)", nameof(grams));

        return new Weight(grams);
    }

    /// <summary>
    /// Creates weight from kilograms (common input format)
    /// </summary>
    /// <param name="kilograms">Weight in kilograms</param>
    /// <returns>Weight instance</returns>
    public static Weight FromKilograms(decimal kilograms)
    {
        var grams = (int)Math.Round(kilograms * 1000);
        return Create(grams);
    }

    /// <summary>
    /// Creates weight from pounds (for international compatibility)
    /// </summary>
    /// <param name="pounds">Weight in pounds</param>
    /// <returns>Weight instance</returns>
    public static Weight FromPounds(decimal pounds)
    {
        var grams = (int)Math.Round(pounds * 453.592m);
        return Create(grams);
    }

    /// <summary>
    /// Checks if weight is within Vietnamese postal service limits
    /// Vietnam Post: max 30kg for standard delivery
    /// </summary>
    /// <returns>true if weight is within postal limits</returns>
    public bool IsWithinPostalLimits()
    {
        const int MaxPostalGrams = 30_000; // 30kg
        return Grams <= MaxPostalGrams;
    }

    /// <summary>
    /// Checks if weight qualifies for express shipping
    /// Express shipping typically limited to lighter packages
    /// </summary>
    /// <returns>true if weight qualifies for express shipping</returns>
    public bool IsExpressShippingEligible()
    {
        const int MaxExpressGrams = 5_000; // 5kg
        return Grams <= MaxExpressGrams;
    }

    /// <summary>
    /// Gets the shipping category based on weight for Vietnamese logistics
    /// </summary>
    /// <returns>Shipping category</returns>
    public WeightCategory GetShippingCategory()
    {
        return Grams switch
        {
            <= 500 => WeightCategory.Light,        // Up to 500g
            <= 2_000 => WeightCategory.Medium,     // Up to 2kg
            <= 10_000 => WeightCategory.Heavy,     // Up to 10kg
            <= 30_000 => WeightCategory.BulkStandard, // Up to 30kg
            _ => WeightCategory.BulkOversized       // Over 30kg
        };
    }

    /// <summary>
    /// Calculates estimated shipping cost category for Vietnamese market
    /// </summary>
    /// <returns>Cost multiplier category</returns>
    public decimal GetShippingCostMultiplier()
    {
        return GetShippingCategory() switch
        {
            WeightCategory.Light => 1.0m,
            WeightCategory.Medium => 1.5m,
            WeightCategory.Heavy => 2.5m,
            WeightCategory.BulkStandard => 4.0m,
            WeightCategory.BulkOversized => 6.0m,
            _ => 1.0m
        };
    }

    /// <summary>
    /// Determines whether the specified Weight is equal to the current Weight
    /// </summary>
    /// <param name="other">The Weight to compare with the current Weight</param>
    /// <returns>true if the specified Weight is equal to the current Weight; otherwise, false</returns>
    public bool Equals(Weight? other)
    {
        return other is not null && Grams == other.Grams;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Weight
    /// </summary>
    /// <param name="obj">The object to compare with the current Weight</param>
    /// <returns>true if the specified object is equal to the current Weight; otherwise, false</returns>
    public override bool Equals(object? obj)
    {
        return obj is Weight other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this Weight
    /// </summary>
    /// <returns>A hash code for the current Weight</returns>
    public override int GetHashCode()
    {
        return Grams.GetHashCode();
    }

    /// <summary>
    /// Returns the weight as a formatted string
    /// </summary>
    /// <returns>The weight formatted based on magnitude</returns>
    public override string ToString()
    {
        if (Grams >= 1000)
        {
            return $"{Kilograms.ToString("0.###", CultureInfo.InvariantCulture)} kg";
        }
        else
        {
            return $"{Grams.ToString(CultureInfo.InvariantCulture)} g";
        }
    }

    /// <summary>
    /// Determines whether two Weight instances are equal
    /// </summary>
    /// <param name="left">The first Weight to compare</param>
    /// <param name="right">The second Weight to compare</param>
    /// <returns>true if the Weight instances are equal; otherwise, false</returns>
    public static bool operator ==(Weight? left, Weight? right)
    {
        return EqualityComparer<Weight>.Default.Equals(left, right);
    }

    /// <summary>
    /// Determines whether two Weight instances are not equal
    /// </summary>
    /// <param name="left">The first Weight to compare</param>
    /// <param name="right">The second Weight to compare</param>
    /// <returns>true if the Weight instances are not equal; otherwise, false</returns>
    public static bool operator !=(Weight? left, Weight? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Adds two weights together
    /// </summary>
    /// <param name="left">The first weight</param>
    /// <param name="right">The second weight</param>
    /// <returns>The sum of the two weights</returns>
    public static Weight operator +(Weight left, Weight right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return Create(left.Grams + right.Grams);
    }

    /// <summary>
    /// Subtracts one weight from another
    /// </summary>
    /// <param name="left">The weight to subtract from</param>
    /// <param name="right">The weight to subtract</param>
    /// <returns>The difference between the two weights</returns>
    public static Weight operator -(Weight left, Weight right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return Create(left.Grams - right.Grams);
    }

    /// <summary>
    /// Compares two weights
    /// </summary>
    /// <param name="left">The first weight</param>
    /// <param name="right">The second weight</param>
    /// <returns>true if left is greater than right</returns>
    public static bool operator >(Weight left, Weight right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.Grams > right.Grams;
    }

    /// <summary>
    /// Compares two weights
    /// </summary>
    /// <param name="left">The first weight</param>
    /// <param name="right">The second weight</param>
    /// <returns>true if left is less than right</returns>
    public static bool operator <(Weight left, Weight right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.Grams < right.Grams;
    }

    /// <summary>
    /// Compares two weights
    /// </summary>
    /// <param name="left">The first weight</param>
    /// <param name="right">The second weight</param>
    /// <returns>true if left is greater than or equal to right</returns>
    public static bool operator >=(Weight left, Weight right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.Grams >= right.Grams;
    }

    /// <summary>
    /// Compares two weights
    /// </summary>
    /// <param name="left">The first weight</param>
    /// <param name="right">The second weight</param>
    /// <returns>true if left is less than or equal to right</returns>
    public static bool operator <=(Weight left, Weight right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.Grams <= right.Grams;
    }
}

/// <summary>
/// Weight categories for shipping and logistics in Vietnamese marketplace
/// </summary>
public enum WeightCategory
{
    /// <summary>
    /// Light weight items (up to 500g) - letters, small accessories
    /// </summary>
    Light,
    
    /// <summary>
    /// Medium weight items (500g - 2kg) - books, electronics, clothing
    /// </summary>
    Medium,
    
    /// <summary>
    /// Heavy items (2kg - 10kg) - appliances, multiple items
    /// </summary>
    Heavy,
    
    /// <summary>
    /// Bulk standard items (10kg - 30kg) - furniture, large appliances
    /// </summary>
    BulkStandard,
    
    /// <summary>
    /// Oversized bulk items (over 30kg) - industrial equipment, vehicles
    /// </summary>
    BulkOversized
}