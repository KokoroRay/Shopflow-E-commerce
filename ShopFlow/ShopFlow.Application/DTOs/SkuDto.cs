using ShopFlow.Application.DTOs;

namespace ShopFlow.Application.DTOs;

/// <summary>
/// Data Transfer Object for SKU information
/// </summary>
public class SkuDto
{
    /// <summary>
    /// Gets or sets the SKU ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the auto-generated SKU code
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product barcode
    /// </summary>
    public string Barcode { get; set; } = null!;

    /// <summary>
    /// Gets or sets the barcode type (EAN13, UPC, etc.)
    /// </summary>
    public string BarcodeType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the SKU display name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the detailed description
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets the retail price in VND
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the compare-at price for discount display
    /// </summary>
    public decimal? CompareAtPrice { get; set; }

    /// <summary>
    /// Gets or sets the vendor cost price
    /// </summary>
    public decimal CostPrice { get; set; }

    /// <summary>
    /// Gets or sets whether this SKU is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the dimensions information
    /// </summary>
    public DimensionsDto Dimensions { get; set; } = null!;

    /// <summary>
    /// Gets or sets the weight information
    /// </summary>
    public WeightDto Weight { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product ID this SKU belongs to
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name (optional, for display purposes)
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// Gets or sets the media items for this SKU
    /// </summary>
    public ICollection<SkuMediaDto> Media { get; set; } = new List<SkuMediaDto>();

    /// <summary>
    /// Gets or sets the option values that define this variant
    /// </summary>
    public ICollection<SkuOptionValueDto> OptionValues { get; set; } = new List<SkuOptionValueDto>();

    /// <summary>
    /// Gets or sets the creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data Transfer Object for SKU dimensions
/// </summary>
public class DimensionsDto
{
    /// <summary>
    /// Gets or sets the length in millimeters
    /// </summary>
    public decimal LengthMm { get; set; }

    /// <summary>
    /// Gets or sets the width in millimeters
    /// </summary>
    public decimal WidthMm { get; set; }

    /// <summary>
    /// Gets or sets the height in millimeters
    /// </summary>
    public decimal HeightMm { get; set; }

    /// <summary>
    /// Gets the volume in cubic millimeters
    /// </summary>
    public decimal VolumeMm3 => LengthMm * WidthMm * HeightMm;

    /// <summary>
    /// Gets whether dimensions are within Vietnamese postal limits (60cm max)
    /// </summary>
    public bool IsWithinPostalLimits => LengthMm <= 600 && WidthMm <= 600 && HeightMm <= 600;
}

/// <summary>
/// Data Transfer Object for SKU weight
/// </summary>
public class WeightDto
{
    /// <summary>
    /// Gets or sets the weight in grams
    /// </summary>
    public decimal Grams { get; set; }

    /// <summary>
    /// Gets the weight in kilograms
    /// </summary>
    public decimal Kilograms => Grams / 1000m;

    /// <summary>
    /// Gets the shipping category based on weight
    /// </summary>
    public string ShippingCategory => Grams switch
    {
        <= 500 => "Light",
        <= 2000 => "Standard",
        <= 10000 => "Heavy",
        <= 30000 => "Bulky",
        _ => "Oversized"
    };

    /// <summary>
    /// Gets whether weight is within Vietnamese postal limits (30kg)
    /// </summary>
    public bool IsWithinPostalLimits => Grams <= 30000;
}

/// <summary>
/// Data Transfer Object for SKU media
/// </summary>
public class SkuMediaDto
{
    /// <summary>
    /// Gets or sets the media ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the SKU ID
    /// </summary>
    public long SkuId { get; set; }

    /// <summary>
    /// Gets or sets the media type
    /// </summary>
    public string MediaType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the media URL
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets whether this is the default media
    /// </summary>
    public bool IsDefault { get; set; }
}

/// <summary>
/// Data Transfer Object for SKU option values
/// </summary>
public class SkuOptionValueDto
{
    /// <summary>
    /// Gets or sets the SKU ID
    /// </summary>
    public long SkuId { get; set; }

    /// <summary>
    /// Gets or sets the attribute ID
    /// </summary>
    public long AttributeId { get; set; }

    /// <summary>
    /// Gets or sets the option ID
    /// </summary>
    public long OptionId { get; set; }

    /// <summary>
    /// Gets or sets the attribute name
    /// </summary>
    public string AttributeName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the option value
    /// </summary>
    public string OptionValue { get; set; } = null!;
}