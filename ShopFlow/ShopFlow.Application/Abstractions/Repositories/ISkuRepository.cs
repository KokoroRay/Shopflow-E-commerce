using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.Abstractions.Repositories;

/// <summary>
/// Repository interface for SKU entity operations with Vietnamese marketplace support
/// </summary>
public interface ISkuRepository
{
    /// <summary>
    /// Creates a new SKU in the database
    /// </summary>
    /// <param name="sku">The SKU entity to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created SKU with assigned ID</returns>
    Task<Sku> CreateAsync(Sku sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing SKU in the database
    /// </summary>
    /// <param name="sku">The SKU entity to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated SKU</returns>
    Task<Sku> UpdateAsync(Sku sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a SKU from the database
    /// </summary>
    /// <param name="id">The SKU ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a SKU by its ID
    /// </summary>
    /// <param name="id">The SKU ID</param>
    /// <param name="includeRelated">Whether to include related entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The SKU if found, null otherwise</returns>
    Task<Sku?> GetByIdAsync(long id, bool includeRelated = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a SKU by its code
    /// </summary>
    /// <param name="code">The SKU code</param>
    /// <param name="includeRelated">Whether to include related entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The SKU if found, null otherwise</returns>
    Task<Sku?> GetByCodeAsync(string code, bool includeRelated = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a SKU by its barcode
    /// </summary>
    /// <param name="barcode">The barcode</param>
    /// <param name="includeRelated">Whether to include related entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The SKU if found, null otherwise</returns>
    Task<Sku?> GetByBarcodeAsync(string barcode, bool includeRelated = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets SKUs by product ID
    /// </summary>
    /// <param name="productId">The product ID</param>
    /// <param name="isActive">Filter by active status (null for all)</param>
    /// <param name="includeRelated">Whether to include related entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of SKUs for the product</returns>
    Task<IEnumerable<Sku>> GetByProductIdAsync(long productId, bool? isActive = null, bool includeRelated = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated list of SKUs with filtering and sorting
    /// </summary>
    /// <param name="productId">Filter by product ID (optional)</param>
    /// <param name="isActive">Filter by active status (optional)</param>
    /// <param name="minPrice">Minimum price filter (optional)</param>
    /// <param name="maxPrice">Maximum price filter (optional)</param>
    /// <param name="searchTerm">Search term for code/name (optional)</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="sortBy">Sort field (optional)</param>
    /// <param name="sortDirection">Sort direction (asc/desc)</param>
    /// <param name="includeRelated">Whether to include related entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated result with SKUs and total count</returns>
    Task<(IEnumerable<Sku> Items, int TotalCount)> GetPaginatedAsync(
        long? productId = null,
        bool? isActive = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 20,
        string? sortBy = null,
        string sortDirection = "asc",
        bool includeRelated = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches SKUs by multiple criteria
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="searchFields">Fields to search in</param>
    /// <param name="filters">Additional filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching SKUs</returns>
    Task<IEnumerable<Sku>> SearchAsync(
        string searchTerm,
        IEnumerable<string> searchFields,
        object? filters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a SKU code already exists
    /// </summary>
    /// <param name="code">The SKU code to check</param>
    /// <param name="excludeId">SKU ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if code exists</returns>
    Task<bool> CodeExistsAsync(string code, long? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a barcode already exists
    /// </summary>
    /// <param name="barcode">The barcode to check</param>
    /// <param name="excludeId">SKU ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if barcode exists</returns>
    Task<bool> BarcodeExistsAsync(string barcode, long? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next available sequence number for SKU code generation
    /// </summary>
    /// <param name="productId">The product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Next sequence number</returns>
    Task<int> GetNextSequenceNumberAsync(long productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk updates SKU status (active/inactive)
    /// </summary>
    /// <param name="skuIds">List of SKU IDs to update</param>
    /// <param name="isActive">New active status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of SKUs updated</returns>
    Task<int> BulkUpdateStatusAsync(IEnumerable<long> skuIds, bool isActive, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets SKU inventory information
    /// </summary>
    /// <param name="skuId">The SKU ID</param>
    /// <param name="warehouseId">Optional warehouse ID filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Inventory information</returns>
    Task<(decimal TotalStock, decimal ReservedStock, IEnumerable<(long WarehouseId, string WarehouseName, decimal Stock, decimal Reserved)> WarehouseStocks)> GetInventoryAsync(
        long skuId,
        long? warehouseId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that SKU can be deleted (no dependencies)
    /// </summary>
    /// <param name="id">The SKU ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with any blocking dependencies</returns>
    Task<(bool CanDelete, IEnumerable<string> BlockingReasons)> ValidateDeleteAsync(long id, CancellationToken cancellationToken = default);
}