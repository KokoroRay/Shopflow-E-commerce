using System.Globalization;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Value object representing product dimensions with Vietnamese marketplace validation
/// </summary>
public sealed class Dimensions : IEquatable<Dimensions>
{
    private const int MinDimension = 1; // 1mm minimum
    private const int MaxDimension = 10000; // 10 meters maximum

    /// <summary>
    /// Gets the length in millimeters
    /// </summary>
    public int LengthMm { get; }
    
    /// <summary>
    /// Gets the width in millimeters
    /// </summary>
    public int WidthMm { get; }
    
    /// <summary>
    /// Gets the height in millimeters
    /// </summary>
    public int HeightMm { get; }

    /// <summary>
    /// Gets the calculated volume in cubic millimeters
    /// </summary>
    public long VolumeMm3 => (long)LengthMm * WidthMm * HeightMm;

    /// <summary>
    /// Gets the calculated volume in cubic centimeters
    /// </summary>
    public decimal VolumeCm3 => VolumeMm3 / 1000m;

    private Dimensions(int lengthMm, int widthMm, int heightMm)
    {
        LengthMm = lengthMm;
        WidthMm = widthMm;
        HeightMm = heightMm;
    }

    /// <summary>
    /// Creates dimensions with validation for Vietnamese marketplace standards
    /// </summary>
    /// <param name="lengthMm">Length in millimeters</param>
    /// <param name="widthMm">Width in millimeters</param>
    /// <param name="heightMm">Height in millimeters</param>
    /// <returns>Dimensions instance</returns>
    /// <exception cref="ArgumentException">When dimensions are invalid</exception>
    public static Dimensions Create(int lengthMm, int widthMm, int heightMm)
    {
        ValidateDimension(lengthMm, nameof(lengthMm));
        ValidateDimension(widthMm, nameof(widthMm));
        ValidateDimension(heightMm, nameof(heightMm));

        return new Dimensions(lengthMm, widthMm, heightMm);
    }

    /// <summary>
    /// Creates dimensions from centimeters (common input format)
    /// </summary>
    /// <param name="lengthCm">Length in centimeters</param>
    /// <param name="widthCm">Width in centimeters</param>
    /// <param name="heightCm">Height in centimeters</param>
    /// <returns>Dimensions instance</returns>
    public static Dimensions FromCentimeters(decimal lengthCm, decimal widthCm, decimal heightCm)
    {
        var lengthMm = (int)Math.Round(lengthCm * 10);
        var widthMm = (int)Math.Round(widthCm * 10);
        var heightMm = (int)Math.Round(heightCm * 10);

        return Create(lengthMm, widthMm, heightMm);
    }

    /// <summary>
    /// Creates dimensions from inches (for international compatibility)
    /// </summary>
    /// <param name="lengthIn">Length in inches</param>
    /// <param name="widthIn">Width in inches</param>
    /// <param name="heightIn">Height in inches</param>
    /// <returns>Dimensions instance</returns>
    public static Dimensions FromInches(decimal lengthIn, decimal widthIn, decimal heightIn)
    {
        const decimal InchesToMm = 25.4m;
        
        var lengthMm = (int)Math.Round(lengthIn * InchesToMm);
        var widthMm = (int)Math.Round(widthIn * InchesToMm);
        var heightMm = (int)Math.Round(heightIn * InchesToMm);

        return Create(lengthMm, widthMm, heightMm);
    }

    /// <summary>
    /// Gets length in centimeters
    /// </summary>
    /// <returns>Length in centimeters</returns>
    public decimal GetLengthCm() => LengthMm / 10m;

    /// <summary>
    /// Gets width in centimeters
    /// </summary>
    /// <returns>Width in centimeters</returns>
    public decimal GetWidthCm() => WidthMm / 10m;

    /// <summary>
    /// Gets height in centimeters
    /// </summary>
    /// <returns>Height in centimeters</returns>
    public decimal GetHeightCm() => HeightMm / 10m;

    /// <summary>
    /// Checks if dimensions exceed Vietnamese postal service limits
    /// Vietnam Post: max 60cm length, 120cm total L+W+H
    /// </summary>
    /// <returns>true if dimensions are within postal limits</returns>
    public bool IsWithinPostalLimits()
    {
        const int MaxLengthMm = 600; // 60cm
        const int MaxTotalMm = 1200; // 120cm total

        var maxDimension = Math.Max(LengthMm, Math.Max(WidthMm, HeightMm));
        var totalDimension = LengthMm + WidthMm + HeightMm;

        return maxDimension <= MaxLengthMm && totalDimension <= MaxTotalMm;
    }

    /// <summary>
    /// Checks if dimensions are suitable for standard shipping boxes
    /// </summary>
    /// <returns>true if dimensions fit standard shipping sizes</returns>
    public bool IsStandardShippingSize()
    {
        // Common Vietnamese shipping box sizes in mm
        var standardSizes = new[]
        {
            (200, 200, 100),   // Small box
            (300, 200, 100),   // Medium box
            (400, 300, 200),   // Large box
            (500, 400, 300),   // Extra large box
        };

        return standardSizes.Any(size => 
            LengthMm <= size.Item1 && 
            WidthMm <= size.Item2 && 
            HeightMm <= size.Item3);
    }

    private static void ValidateDimension(int dimension, string paramName)
    {
        if (dimension < MinDimension)
            throw new ArgumentException($"Dimension must be at least {MinDimension}mm", paramName);

        if (dimension > MaxDimension)
            throw new ArgumentException($"Dimension cannot exceed {MaxDimension}mm (10m)", paramName);
    }

    /// <summary>
    /// Determines whether the specified Dimensions is equal to the current Dimensions
    /// </summary>
    /// <param name="other">The Dimensions to compare with the current Dimensions</param>
    /// <returns>true if the specified Dimensions is equal to the current Dimensions; otherwise, false</returns>
    public bool Equals(Dimensions? other)
    {
        return other is not null && 
               LengthMm == other.LengthMm && 
               WidthMm == other.WidthMm && 
               HeightMm == other.HeightMm;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Dimensions
    /// </summary>
    /// <param name="obj">The object to compare with the current Dimensions</param>
    /// <returns>true if the specified object is equal to the current Dimensions; otherwise, false</returns>
    public override bool Equals(object? obj)
    {
        return obj is Dimensions other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this Dimensions
    /// </summary>
    /// <returns>A hash code for the current Dimensions</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(LengthMm, WidthMm, HeightMm);
    }

    /// <summary>
    /// Returns the dimensions as a formatted string
    /// </summary>
    /// <returns>The dimensions formatted as "L×W×H cm"</returns>
    public override string ToString()
    {
        return $"{GetLengthCm().ToString("0.#", CultureInfo.InvariantCulture)}×{GetWidthCm().ToString("0.#", CultureInfo.InvariantCulture)}×{GetHeightCm().ToString("0.#", CultureInfo.InvariantCulture)} cm";
    }

    /// <summary>
    /// Determines whether two Dimensions instances are equal
    /// </summary>
    /// <param name="left">The first Dimensions to compare</param>
    /// <param name="right">The second Dimensions to compare</param>
    /// <returns>true if the Dimensions instances are equal; otherwise, false</returns>
    public static bool operator ==(Dimensions? left, Dimensions? right)
    {
        return EqualityComparer<Dimensions>.Default.Equals(left, right);
    }

    /// <summary>
    /// Determines whether two Dimensions instances are not equal
    /// </summary>
    /// <param name="left">The first Dimensions to compare</param>
    /// <param name="right">The second Dimensions to compare</param>
    /// <returns>true if the Dimensions instances are not equal; otherwise, false</returns>
    public static bool operator !=(Dimensions? left, Dimensions? right)
    {
        return !(left == right);
    }
}