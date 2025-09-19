namespace ShopFlow.Application.Contracts.Requests;

/// <summary>
/// Request to create a new SKU with Vietnamese marketplace validation
/// </summary>
public class CreateSkuRequest
{
    /// <summary>
    /// Gets or sets the SKU code (optional, will be auto-generated if not provided)
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the barcode (optional, will be auto-generated if not provided)
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Gets or sets the barcode type (EAN13, UPC-A, etc.)
    /// </summary>
    public string? BarcodeType { get; set; }

    /// <summary>
    /// Gets or sets the display name for this SKU variant
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
    /// Gets or sets whether this SKU should be active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the product ID this SKU belongs to
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// Gets or sets the physical dimensions
    /// </summary>
    public CreateDimensionsRequest Dimensions { get; set; } = null!;

    /// <summary>
    /// Gets or sets the weight information
    /// </summary>
    public CreateWeightRequest Weight { get; set; } = null!;

    /// <summary>
    /// Gets or sets the option values that define this variant
    /// </summary>
    public ICollection<CreateSkuOptionValueRequest> OptionValues { get; set; } = new List<CreateSkuOptionValueRequest>();
}

/// <summary>
/// Request to update an existing SKU
/// </summary>
public class UpdateSkuRequest
{
    /// <summary>
    /// Gets or sets the SKU ID to update
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the display name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the retail price in VND
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Gets or sets the compare-at price
    /// </summary>
    public decimal? CompareAtPrice { get; set; }

    /// <summary>
    /// Gets or sets the cost price
    /// </summary>
    public decimal? CostPrice { get; set; }

    /// <summary>
    /// Gets or sets whether this SKU is active
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the dimensions to update
    /// </summary>
    public UpdateDimensionsRequest? Dimensions { get; set; }

    /// <summary>
    /// Gets or sets the weight to update
    /// </summary>
    public UpdateWeightRequest? Weight { get; set; }
}

/// <summary>
/// Request to create dimensions information
/// </summary>
public class CreateDimensionsRequest
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
}

/// <summary>
/// Request to update dimensions information
/// </summary>
public class UpdateDimensionsRequest
{
    /// <summary>
    /// Gets or sets the length in millimeters
    /// </summary>
    public decimal? LengthMm { get; set; }

    /// <summary>
    /// Gets or sets the width in millimeters
    /// </summary>
    public decimal? WidthMm { get; set; }

    /// <summary>
    /// Gets or sets the height in millimeters
    /// </summary>
    public decimal? HeightMm { get; set; }
}

/// <summary>
/// Request to create weight information
/// </summary>
public class CreateWeightRequest
{
    /// <summary>
    /// Gets or sets the weight in grams
    /// </summary>
    public decimal Grams { get; set; }
}

/// <summary>
/// Request to update weight information
/// </summary>
public class UpdateWeightRequest
{
    /// <summary>
    /// Gets or sets the weight in grams
    /// </summary>
    public decimal? Grams { get; set; }
}

/// <summary>
/// Request to create SKU option value
/// </summary>
public class CreateSkuOptionValueRequest
{
    /// <summary>
    /// Gets or sets the attribute ID
    /// </summary>
    public long AttributeId { get; set; }

    /// <summary>
    /// Gets or sets the option ID
    /// </summary>
    public long OptionId { get; set; }
}

/// <summary>
/// Request to get SKUs with filtering and pagination
/// </summary>
public class GetSkusRequest
{
    /// <summary>
    /// Gets or sets the product ID to filter by
    /// </summary>
    public long? ProductId { get; set; }

    /// <summary>
    /// Gets or sets whether to include only active SKUs
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the minimum price filter
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Gets or sets the maximum price filter
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Gets or sets the search term for SKU code or name
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Gets or sets the sort field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets the sort direction
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}