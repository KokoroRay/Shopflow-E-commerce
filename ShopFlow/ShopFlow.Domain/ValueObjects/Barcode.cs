using System.Globalization;

namespace ShopFlow.Domain.ValueObjects;

/// <summary>
/// Value object representing barcode with Vietnamese marketplace validation
/// </summary>
public sealed class Barcode : IEquatable<Barcode>
{
    /// <summary>
    /// Gets the barcode value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the barcode type
    /// </summary>
    public BarcodeType Type { get; }

    private Barcode(string value, BarcodeType type)
    {
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Creates barcode with validation for Vietnamese marketplace standards
    /// </summary>
    /// <param name="value">Barcode value</param>
    /// <param name="type">Barcode type (EAN13, UPC, etc.)</param>
    /// <returns>Barcode instance</returns>
    /// <exception cref="ArgumentException">When barcode is invalid</exception>
    public static Barcode Create(string value, BarcodeType type = BarcodeType.EAN13)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Barcode cannot be empty", nameof(value));

        if (!IsValidFormat(value, type))
            throw new ArgumentException($"Invalid {type} barcode format", nameof(value));

        if (!IsValidChecksum(value, type))
            throw new ArgumentException($"Invalid {type} barcode checksum", nameof(value));

        return new Barcode(value, type);
    }

    /// <summary>
    /// Generates automatic Vietnamese EAN13 barcode
    /// Country code 893 is assigned to Vietnam
    /// </summary>
    /// <param name="companyPrefix">Vietnamese company prefix (3-7 digits)</param>
    /// <param name="itemReference">Item reference number</param>
    /// <returns>Auto-generated Vietnamese EAN13 barcode</returns>
    public static Barcode GenerateVietnameseEan13(string companyPrefix, int itemReference)
    {
        if (string.IsNullOrWhiteSpace(companyPrefix))
            throw new ArgumentException("Company prefix is required", nameof(companyPrefix));

        if (companyPrefix.Length < 3 || companyPrefix.Length > 7)
            throw new ArgumentException("Company prefix must be 3-7 digits", nameof(companyPrefix));

        if (!companyPrefix.All(char.IsDigit))
            throw new ArgumentException("Company prefix must contain only digits", nameof(companyPrefix));

        // Vietnam country code: 893
        var countryCode = "893";

        // Pad company prefix to fit EAN13 format
        var maxItemDigits = 12 - countryCode.Length - companyPrefix.Length;
        var itemRefStr = itemReference.ToString(CultureInfo.InvariantCulture).PadLeft(maxItemDigits, '0');

        if (itemRefStr.Length > maxItemDigits)
            throw new ArgumentException($"Item reference too large for company prefix length", nameof(itemReference));

        // Build 12-digit code without checksum
        var code12 = countryCode + companyPrefix + itemRefStr;

        // Calculate and append checksum
        var checksum = CalculateEan13Checksum(code12);
        var ean13 = code12 + checksum;

        return new Barcode(ean13, BarcodeType.EAN13);
    }

    /// <summary>
    /// Generates automatic UPC-A barcode for Vietnamese marketplace
    /// </summary>
    /// <param name="manufacturerId">Vietnamese manufacturer ID (6 digits)</param>
    /// <param name="productId">Product ID (5 digits)</param>
    /// <returns>Auto-generated UPC-A barcode</returns>
    public static Barcode GenerateUpcA(string manufacturerId, int productId)
    {
        if (string.IsNullOrWhiteSpace(manufacturerId) || manufacturerId.Length != 6)
            throw new ArgumentException("Manufacturer ID must be exactly 6 digits", nameof(manufacturerId));

        if (!manufacturerId.All(char.IsDigit))
            throw new ArgumentException("Manufacturer ID must contain only digits", nameof(manufacturerId));

        var productIdStr = productId.ToString("D5", CultureInfo.InvariantCulture);
        var code11 = manufacturerId + productIdStr;
        var checksum = CalculateUpcChecksum(code11);
        var upcA = code11 + checksum;

        return new Barcode(upcA, BarcodeType.UPCA);
    }

    private static bool IsValidFormat(string value, BarcodeType type)
    {
        return type switch
        {
            BarcodeType.EAN13 => value.Length == 13 && value.All(char.IsDigit),
            BarcodeType.EAN8 => value.Length == 8 && value.All(char.IsDigit),
            BarcodeType.UPCA => value.Length == 12 && value.All(char.IsDigit),
            BarcodeType.UPCE => value.Length == 8 && value.All(char.IsDigit),
            BarcodeType.CODE128 => value.Length >= 1 && value.Length <= 80,
            BarcodeType.CODE39 => value.All(c => "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%".Contains(c, StringComparison.Ordinal)),
            _ => false
        };
    }

    private static bool IsValidChecksum(string value, BarcodeType type)
    {
        return type switch
        {
            BarcodeType.EAN13 => IsValidEan13Checksum(value),
            BarcodeType.EAN8 => IsValidEan8Checksum(value),
            BarcodeType.UPCA => IsValidUpcChecksum(value),
            BarcodeType.UPCE => IsValidUpceChecksum(value),
            _ => true // No checksum validation for other types
        };
    }

    private static bool IsValidEan13Checksum(string ean13)
    {
        if (ean13.Length != 13) return false;

        var code12 = ean13[..12];
        var expectedChecksum = CalculateEan13Checksum(code12);
        var actualChecksum = ean13[12].ToString();

        return expectedChecksum == actualChecksum;
    }

    private static string CalculateEan13Checksum(string code12)
    {
        var sum = 0;
        for (int i = 0; i < 12; i++)
        {
            var digit = int.Parse(code12[i].ToString(), CultureInfo.InvariantCulture);
            sum += i % 2 == 0 ? digit : digit * 3;
        }

        var checksum = (10 - (sum % 10)) % 10;
        return checksum.ToString(CultureInfo.InvariantCulture);
    }

    private static bool IsValidEan8Checksum(string ean8)
    {
        if (ean8.Length != 8) return false;

        var sum = 0;
        for (int i = 0; i < 7; i++)
        {
            var digit = int.Parse(ean8[i].ToString(), CultureInfo.InvariantCulture);
            sum += i % 2 == 0 ? digit * 3 : digit;
        }

        var expectedChecksum = (10 - (sum % 10)) % 10;
        var actualChecksum = int.Parse(ean8[7].ToString(), CultureInfo.InvariantCulture);

        return expectedChecksum == actualChecksum;
    }

    private static bool IsValidUpcChecksum(string upc)
    {
        if (upc.Length != 12) return false;

        var code11 = upc[..11];
        var expectedChecksum = CalculateUpcChecksum(code11);
        var actualChecksum = upc[11].ToString();

        return expectedChecksum == actualChecksum;
    }

    private static string CalculateUpcChecksum(string code11)
    {
        var sum = 0;
        for (int i = 0; i < 11; i++)
        {
            var digit = int.Parse(code11[i].ToString(), CultureInfo.InvariantCulture);
            sum += i % 2 == 0 ? digit * 3 : digit;
        }

        var checksum = (10 - (sum % 10)) % 10;
        return checksum.ToString(CultureInfo.InvariantCulture);
    }

    private static bool IsValidUpceChecksum(string upce)
    {
        // UPC-E validation is more complex, simplified here
        return upce.Length == 8 && upce.All(char.IsDigit);
    }

    /// <summary>
    /// Determines whether the specified Barcode is equal to the current Barcode
    /// </summary>
    /// <param name="other">The Barcode to compare with the current Barcode</param>
    /// <returns>true if the specified Barcode is equal to the current Barcode; otherwise, false</returns>
    public bool Equals(Barcode? other)
    {
        return other is not null && Value == other.Value && Type == other.Type;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Barcode
    /// </summary>
    /// <param name="obj">The object to compare with the current Barcode</param>
    /// <returns>true if the specified object is equal to the current Barcode; otherwise, false</returns>
    public override bool Equals(object? obj)
    {
        return obj is Barcode other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this Barcode
    /// </summary>
    /// <returns>A hash code for the current Barcode</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Type);
    }

    /// <summary>
    /// Returns the barcode value as a string
    /// </summary>
    /// <returns>The barcode value</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a Barcode to a string
    /// </summary>
    /// <param name="barcode">The Barcode to convert</param>
    /// <returns>The barcode value as a string</returns>
    /// <exception cref="ArgumentNullException">When barcode is null</exception>
    public static implicit operator string(Barcode barcode)
    {
        ArgumentNullException.ThrowIfNull(barcode);
        return barcode.Value;
    }

    /// <summary>
    /// Determines whether two Barcode instances are equal
    /// </summary>
    /// <param name="left">The first Barcode to compare</param>
    /// <param name="right">The second Barcode to compare</param>
    /// <returns>true if the Barcode instances are equal; otherwise, false</returns>
    public static bool operator ==(Barcode? left, Barcode? right)
    {
        return EqualityComparer<Barcode>.Default.Equals(left, right);
    }

    /// <summary>
    /// Determines whether two Barcode instances are not equal
    /// </summary>
    /// <param name="left">The first Barcode to compare</param>
    /// <param name="right">The second Barcode to compare</param>
    /// <returns>true if the Barcode instances are not equal; otherwise, false</returns>
    public static bool operator !=(Barcode? left, Barcode? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Supported barcode types for Vietnamese marketplace
/// </summary>
public enum BarcodeType
{
    /// <summary>
    /// EAN-13 barcode (most common in Vietnam)
    /// </summary>
    EAN13,

    /// <summary>
    /// EAN-8 compact barcode
    /// </summary>
    EAN8,

    /// <summary>
    /// Universal Product Code A
    /// </summary>
    UPCA,

    /// <summary>
    /// Universal Product Code E (compact)
    /// </summary>
    UPCE,

    /// <summary>
    /// Code 128 with alphanumeric support
    /// </summary>
    CODE128,

    /// <summary>
    /// Code 39 for legacy support
    /// </summary>
    CODE39
}