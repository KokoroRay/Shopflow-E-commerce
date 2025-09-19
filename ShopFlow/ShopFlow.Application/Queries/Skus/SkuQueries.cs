using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Skus;

/// <summary>
/// Query to get a single SKU by ID
/// </summary>
public class GetSkuByIdQuery : IRequest<GetSkuResponse>
{
    /// <summary>
    /// Gets or sets the SKU ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets whether to include related entities (Product, Media, etc.)
    /// </summary>
    public bool IncludeRelated { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the GetSkuByIdQuery class
    /// </summary>
    /// <param name="id">The SKU ID</param>
    public GetSkuByIdQuery(long id)
    {
        Id = id;
    }
}

/// <summary>
/// Query to get a SKU by its code
/// </summary>
public class GetSkuByCodeQuery : IRequest<GetSkuResponse>
{
    /// <summary>
    /// Gets or sets the SKU code
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Gets or sets whether to include related entities
    /// </summary>
    public bool IncludeRelated { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the GetSkuByCodeQuery class
    /// </summary>
    /// <param name="code">The SKU code</param>
    public GetSkuByCodeQuery(string code)
    {
        Code = code;
    }
}

/// <summary>
/// Query to get a SKU by its barcode
/// </summary>
public class GetSkuByBarcodeQuery : IRequest<GetSkuResponse>
{
    /// <summary>
    /// Gets or sets the barcode
    /// </summary>
    public string Barcode { get; set; } = null!;

    /// <summary>
    /// Gets or sets whether to include related entities
    /// </summary>
    public bool IncludeRelated { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the GetSkuByBarcodeQuery class
    /// </summary>
    /// <param name="barcode">The barcode</param>
    public GetSkuByBarcodeQuery(string barcode)
    {
        Barcode = barcode;
    }
}

/// <summary>
/// Query to get paginated list of SKUs with filtering
/// </summary>
public class GetSkusQuery : IRequest<GetSkusResponse>
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

    /// <summary>
    /// Gets or sets whether to include related entities
    /// </summary>
    public bool IncludeRelated { get; set; } = false;
}

/// <summary>
/// Query to get SKUs by product ID
/// </summary>
public class GetSkusByProductIdQuery : IRequest<GetSkusResponse>
{
    /// <summary>
    /// Gets or sets the product ID
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// Gets or sets whether to include only active SKUs
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets whether to include related entities
    /// </summary>
    public bool IncludeRelated { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the GetSkusByProductIdQuery class
    /// </summary>
    /// <param name="productId">The product ID</param>
    public GetSkusByProductIdQuery(long productId)
    {
        ProductId = productId;
    }
}

/// <summary>
/// Query to search SKUs by various criteria
/// </summary>
public class SearchSkusQuery : IRequest<GetSkusResponse>
{
    /// <summary>
    /// Gets or sets the search term
    /// </summary>
    public string SearchTerm { get; set; } = null!;

    /// <summary>
    /// Gets or sets the search fields to include
    /// </summary>
    public ICollection<string> SearchFields { get; set; } = new List<string> { "code", "name", "description", "barcode" };

    /// <summary>
    /// Gets or sets additional filters
    /// </summary>
    public GetSkusQuery Filters { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the SearchSkusQuery class
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    public SearchSkusQuery(string searchTerm)
    {
        SearchTerm = searchTerm;
    }
}

/// <summary>
/// Query to get SKU inventory information
/// </summary>
public class GetSkuInventoryQuery : IRequest<SkuInventoryResponse>
{
    /// <summary>
    /// Gets or sets the SKU ID
    /// </summary>
    public long SkuId { get; set; }

    /// <summary>
    /// Gets or sets the warehouse ID (optional)
    /// </summary>
    public long? WarehouseId { get; set; }

    /// <summary>
    /// Initializes a new instance of the GetSkuInventoryQuery class
    /// </summary>
    /// <param name="skuId">The SKU ID</param>
    public GetSkuInventoryQuery(long skuId)
    {
        SkuId = skuId;
    }
}

/// <summary>
/// Response for SKU inventory information
/// </summary>
public class SkuInventoryResponse
{
    /// <summary>
    /// Gets or sets the SKU ID
    /// </summary>
    public long SkuId { get; set; }

    /// <summary>
    /// Gets or sets the total available stock
    /// </summary>
    public decimal TotalStock { get; set; }

    /// <summary>
    /// Gets or sets the reserved stock
    /// </summary>
    public decimal ReservedStock { get; set; }

    /// <summary>
    /// Gets or sets the available stock for sale
    /// </summary>
    public decimal AvailableStock => TotalStock - ReservedStock;

    /// <summary>
    /// Gets or sets the stock per warehouse
    /// </summary>
    public ICollection<WarehouseStockInfo> WarehouseStocks { get; set; } = new List<WarehouseStockInfo>();
}

/// <summary>
/// Information about stock in a specific warehouse
/// </summary>
public class WarehouseStockInfo
{
    /// <summary>
    /// Gets or sets the warehouse ID
    /// </summary>
    public long WarehouseId { get; set; }

    /// <summary>
    /// Gets or sets the warehouse name
    /// </summary>
    public string WarehouseName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the stock quantity
    /// </summary>
    public decimal Stock { get; set; }

    /// <summary>
    /// Gets or sets the reserved quantity
    /// </summary>
    public decimal Reserved { get; set; }

    /// <summary>
    /// Gets the available quantity
    /// </summary>
    public decimal Available => Stock - Reserved;
}