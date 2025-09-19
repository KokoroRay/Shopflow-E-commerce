using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Commands.Skus;

/// <summary>
/// Command to create a new SKU with Vietnamese marketplace validation
/// </summary>
public class CreateSkuCommand : IRequest<CreateSkuResponse>
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
    /// Gets or sets the weight in grams
    /// </summary>
    public decimal WeightGrams { get; set; }

    /// <summary>
    /// Gets or sets the option values that define this variant
    /// </summary>
    public ICollection<CreateSkuOptionValueCommand> OptionValues { get; set; } = new List<CreateSkuOptionValueCommand>();
}

/// <summary>
/// Command to create SKU option value
/// </summary>
public class CreateSkuOptionValueCommand
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
/// Command to update an existing SKU
/// </summary>
public class UpdateSkuCommand : IRequest<UpdateSkuResponse>
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

    /// <summary>
    /// Gets or sets the weight in grams
    /// </summary>
    public decimal? WeightGrams { get; set; }
}

/// <summary>
/// Command to delete a SKU
/// </summary>
public class DeleteSkuCommand : IRequest<DeleteSkuResponse>
{
    /// <summary>
    /// Gets or sets the SKU ID to delete
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets whether to force delete even if SKU has dependencies
    /// </summary>
    public bool ForceDelete { get; set; } = false;
}

/// <summary>
/// Command to activate/deactivate multiple SKUs
/// </summary>
public class BulkUpdateSkuStatusCommand : IRequest<BulkSkuOperationResponse>
{
    /// <summary>
    /// Gets or sets the list of SKU IDs to update
    /// </summary>
    public ICollection<long> SkuIds { get; set; } = new List<long>();

    /// <summary>
    /// Gets or sets the new active status
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Command to generate a new barcode for Vietnamese marketplace
/// </summary>
public class GenerateBarcodeCommand : IRequest<GenerateBarcodeResponse>
{
    /// <summary>
    /// Gets or sets the barcode type to generate
    /// </summary>
    public string BarcodeType { get; set; } = "EAN13";

    /// <summary>
    /// Gets or sets whether to generate Vietnamese EAN-13 (country code 893)
    /// </summary>
    public bool UseVietnamesePrefix { get; set; } = true;
}

/// <summary>
/// Command to regenerate SKU code
/// </summary>
public class RegenerateSkuCodeCommand : IRequest<string>
{
    /// <summary>
    /// Gets or sets the product ID to generate code for
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// Gets or sets the variant identifier (optional)
    /// </summary>
    public string? VariantIdentifier { get; set; }
}