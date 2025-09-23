using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Infrastructure.Persistence;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using PersistenceEntities = ShopFlow.Infrastructure.Persistence.Entities;

namespace ShopFlow.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for SKU management with Vietnamese marketplace support
/// Provides comprehensive data access for Vietnamese e-commerce operations
/// </summary>
public class SkuRepository : ISkuRepository
{
    private readonly ILogger<SkuRepository> _logger;
    private readonly ShopFlowDbContext _context;

    /// <summary>
    /// Initializes a new instance of the SkuRepository
    /// </summary>
    /// <param name="context">Database context for Entity Framework operations</param>
    /// <param name="logger">Logger for repository operations and Vietnamese marketplace events</param>
    public SkuRepository(ShopFlowDbContext context, ILogger<SkuRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Core CRUD Operations

    /// <inheritdoc />
    public async Task<Sku> CreateAsync(Sku sku, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sku);
        
        _logger.LogDebug("Creating new SKU: {SkuCode}", sku.Code.Value);
        
        var persistenceEntity = MapToPersistenceEntity(sku);
        
        _context.CatSkus.Add(persistenceEntity);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Reload with all navigation properties
        var addedEntity = await GetByIdAsync(persistenceEntity.Id, true, cancellationToken);
        
        _logger.LogInformation("Successfully created SKU: {SkuCode} with ID: {SkuId}", 
            sku.Code.Value, persistenceEntity.Id);
        
        return addedEntity!;
    }

    /// <inheritdoc />
    public async Task<Sku> UpdateAsync(Sku sku, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sku);
        
        _logger.LogDebug("Updating SKU: {SkuCode}", sku.Code.Value);
        
        var existingEntity = await _context.CatSkus
            .FirstOrDefaultAsync(s => s.Id == sku.Id, cancellationToken);

        if (existingEntity == null)
        {
            throw new InvalidOperationException($"SKU with ID {sku.Id} not found for update");
        }

        UpdatePersistenceEntity(existingEntity, sku);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        // Reload with all navigation properties
        var updatedEntity = await GetByIdAsync(sku.Id, true, cancellationToken);
        
        _logger.LogInformation("Successfully updated SKU: {SkuCode}", sku.Code.Value);
        
        return updatedEntity!;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting SKU with ID: {SkuId}", id);
        
        var entity = await _context.CatSkus.FindAsync([id], cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("SKU with ID {SkuId} not found for deletion", id);
            return false;
        }

        // Check for dependencies before deletion
        var hasOrders = await _context.OrdOrderItems
            .AnyAsync(oi => oi.SkuId == id, cancellationToken);
        
        if (hasOrders)
        {
            throw new InvalidOperationException($"Cannot delete SKU {id} as it has associated orders");
        }

        _context.CatSkus.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Successfully deleted SKU with ID: {SkuId}", id);
        return true;
    }

    /// <inheritdoc />
    public async Task<Sku?> GetByIdAsync(long id, bool includeRelated = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving SKU with ID: {SkuId}", id);
        
        var query = _context.CatSkus.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(s => s.CatSkuMedia)
                .Include(s => s.CatSkuPricings)
                .Include(s => s.CatSkuOptionValues)
                .Include(s => s.InvStocks)
                .AsSplitQuery();
        }

        var persistenceEntity = await query
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (persistenceEntity == null)
        {
            _logger.LogWarning("SKU with ID {SkuId} not found", id);
            return null;
        }

        return MapToDomainEntity(persistenceEntity);
    }

    /// <inheritdoc />
    public async Task<Sku?> GetByCodeAsync(string code, bool includeRelated = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        
        _logger.LogDebug("Retrieving SKU with code: {SkuCode}", code);
        
        var query = _context.CatSkus.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(s => s.CatSkuMedia)
                .Include(s => s.CatSkuPricings)
                .Include(s => s.CatSkuOptionValues)
                .Include(s => s.InvStocks)
                .AsSplitQuery();
        }

        var persistenceEntity = await query
            .FirstOrDefaultAsync(s => s.SkuCode == code, cancellationToken);

        if (persistenceEntity == null)
        {
            _logger.LogWarning("SKU with code {SkuCode} not found", code);
            return null;
        }

        return MapToDomainEntity(persistenceEntity);
    }

    /// <inheritdoc />
    public async Task<Sku?> GetByBarcodeAsync(string barcode, bool includeRelated = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(barcode);
        
        _logger.LogDebug("Retrieving SKU with barcode: {Barcode}", barcode);
        
        var query = _context.CatSkus.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(s => s.CatSkuMedia)
                .Include(s => s.CatSkuPricings)
                .Include(s => s.CatSkuOptionValues)
                .Include(s => s.InvStocks)
                .AsSplitQuery();
        }

        var persistenceEntity = await query
            .FirstOrDefaultAsync(s => s.Barcode == barcode, cancellationToken);

        if (persistenceEntity == null)
        {
            _logger.LogWarning("SKU with barcode {Barcode} not found", barcode);
            return null;
        }

        return MapToDomainEntity(persistenceEntity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Sku>> GetByProductIdAsync(long productId, bool? isActive = null, bool includeRelated = false, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting SKUs for product: {ProductId}", productId);
        
        var query = _context.CatSkus.Where(s => s.ProductId == productId);

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }

        if (includeRelated)
        {
            query = query
                .Include(s => s.CatSkuMedia)
                .Include(s => s.CatSkuPricings)
                .Include(s => s.CatSkuOptionValues)
                .Include(s => s.InvStocks)
                .AsSplitQuery();
        }

        var entities = await query.ToListAsync(cancellationToken);
        return entities.Select(MapToDomainEntity);
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Sku> Items, int TotalCount)> GetPaginatedAsync(
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
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting paginated SKUs with filters");
        
        var query = _context.CatSkus.AsQueryable();

        // Apply filters
        if (productId.HasValue)
        {
            query = query.Where(s => s.ProductId == productId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s => 
                s.SkuCode.Contains(searchTerm) ||
                (s.Barcode != null && s.Barcode.Contains(searchTerm)));
        }

        if (minPrice.HasValue || maxPrice.HasValue)
        {
            query = query.Where(s => s.CatSkuPricings.Any(p => 
                (!minPrice.HasValue || (p.BasePriceVnd ?? 0) >= minPrice.Value) &&
                (!maxPrice.HasValue || (p.BasePriceVnd ?? 0) <= maxPrice.Value)));
        }

        // Count total before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy.ToLowerInvariant() switch
            {
                "code" => sortDirection.ToLowerInvariant() == "desc" 
                    ? query.OrderByDescending(s => s.SkuCode)
                    : query.OrderBy(s => s.SkuCode),
                "created" => sortDirection.ToLowerInvariant() == "desc"
                    ? query.OrderByDescending(s => s.CreatedAt)
                    : query.OrderBy(s => s.CreatedAt),
                _ => query.OrderBy(s => s.SkuCode)
            };
        }
        else
        {
            query = query.OrderBy(s => s.SkuCode);
        }

        if (includeRelated)
        {
            query = query
                .Include(s => s.CatSkuMedia)
                .Include(s => s.CatSkuPricings)
                .Include(s => s.CatSkuOptionValues)
                .Include(s => s.InvStocks)
                .AsSplitQuery();
        }

        // Apply pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var domainEntities = items.Select(MapToDomainEntity);

        return (domainEntities, totalCount);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Sku>> SearchAsync(
        string searchTerm, 
        IEnumerable<string> searchFields, 
        object? filters = null, 
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);
        ArgumentNullException.ThrowIfNull(searchFields);

        _logger.LogDebug("Searching SKUs with term: {SearchTerm}", searchTerm);
        
        var query = _context.CatSkus.AsQueryable();
        var fieldsArray = searchFields.ToArray();

        // Build search conditions based on specified fields
        var searchConditions = new List<System.Linq.Expressions.Expression<Func<PersistenceEntities.CatSku, bool>>>();

        if (fieldsArray.Contains("code", StringComparer.OrdinalIgnoreCase))
        {
            searchConditions.Add(s => s.SkuCode.Contains(searchTerm));
        }

        if (fieldsArray.Contains("barcode", StringComparer.OrdinalIgnoreCase))
        {
            searchConditions.Add(s => s.Barcode != null && s.Barcode.Contains(searchTerm));
        }

        // Combine search conditions with OR logic
        if (searchConditions.Count > 0)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(PersistenceEntities.CatSku), "s");
            System.Linq.Expressions.Expression? combinedExpression = null;

            foreach (var condition in searchConditions)
            {
                var body = System.Linq.Expressions.Expression.Invoke(condition, parameter);
                combinedExpression = combinedExpression == null ? body : System.Linq.Expressions.Expression.OrElse(combinedExpression, body);
            }

            if (combinedExpression != null)
            {
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<PersistenceEntities.CatSku, bool>>(combinedExpression, parameter);
                query = query.Where(lambda);
            }
        }

        var entities = await query.ToListAsync(cancellationToken);
        return entities.Select(MapToDomainEntity);
    }

    #endregion

    #region Validation Operations

    /// <inheritdoc />
    public async Task<bool> CodeExistsAsync(string code, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var query = _context.CatSkus.Where(s => s.SkuCode == code);
        
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> BarcodeExistsAsync(string barcode, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(barcode);

        var query = _context.CatSkus.Where(s => s.Barcode == barcode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> GetNextSequenceNumberAsync(long productId, CancellationToken cancellationToken = default)
    {
        var maxSequence = await _context.CatSkus
            .Where(s => s.ProductId == productId)
            .CountAsync(cancellationToken);

        return maxSequence + 1;
    }

    /// <inheritdoc />
    public async Task<int> BulkUpdateStatusAsync(IEnumerable<long> skuIds, bool isActive, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(skuIds);
        
        var idList = skuIds.ToList();
        if (!idList.Any())
        {
            return 0;
        }

        _logger.LogDebug("Bulk updating status for {Count} SKUs to {Status}", 
            idList.Count, isActive ? "active" : "inactive");
        
        var affectedRows = await _context.CatSkus
            .Where(s => idList.Contains(s.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(sku => sku.IsActive, isActive)
                .SetProperty(sku => sku.UpdatedAt, DateTime.UtcNow), 
                cancellationToken);

        _logger.LogInformation("Updated status for {Count} SKUs", affectedRows);
        return affectedRows;
    }

    /// <inheritdoc />
    public async Task<(decimal TotalStock, decimal ReservedStock, IEnumerable<(long WarehouseId, string WarehouseName, decimal Stock, decimal Reserved)> WarehouseStocks)> GetInventoryAsync(
        long skuId, 
        long? warehouseId = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting inventory for SKU: {SkuId}", skuId);
        
        var query = _context.InvStocks
            .Include(s => s.Warehouse)
            .Where(s => s.SkuId == skuId);

        if (warehouseId.HasValue)
        {
            query = query.Where(s => s.WarehouseId == warehouseId.Value);
        }

        var stocks = await query.ToListAsync(cancellationToken);

        var totalStock = stocks.Sum(s => s.OnHand);
        var reservedStock = stocks.Sum(s => s.Reserved);

        var warehouseStocks = stocks.Select(s => (
            WarehouseId: s.WarehouseId,
            WarehouseName: s.Warehouse?.Name ?? "Unknown",
            Stock: (decimal)s.OnHand,
            Reserved: (decimal)s.Reserved
        ));

        return (totalStock, reservedStock, warehouseStocks);
    }

    /// <inheritdoc />
    public async Task<(bool CanDelete, IEnumerable<string> BlockingReasons)> ValidateDeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var reasons = new List<string>();

        // Check for orders
        var hasOrders = await _context.OrdOrderItems
            .AnyAsync(oi => oi.SkuId == id, cancellationToken);
        if (hasOrders)
        {
            reasons.Add("SKU has associated orders");
        }

        // Check for inventory reservations
        var hasReservations = await _context.InvReservations
            .AnyAsync(r => r.SkuId == id, cancellationToken);
        if (hasReservations)
        {
            reasons.Add("SKU has inventory reservations");
        }

        // Check for cart items
        var hasCartItems = await _context.CartItems
            .AnyAsync(ci => ci.SkuId == id, cancellationToken);
        if (hasCartItems)
        {
            reasons.Add("SKU is in shopping carts");
        }

        return (!reasons.Any(), reasons);
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Maps persistence entity to domain entity
    /// </summary>
    /// <param name="entity">Persistence entity</param>
    /// <returns>Domain entity</returns>
    private static Sku MapToDomainEntity(PersistenceEntities.CatSku entity)
    {
        // Create Value Objects
        var skuCode = SkuCode.Create(entity.SkuCode);
        var barcode = !string.IsNullOrWhiteSpace(entity.Barcode) ? 
            Barcode.Create(entity.Barcode) : null;
        
        var dimensions = entity.LengthMm.HasValue && entity.WidthMm.HasValue && entity.HeightMm.HasValue ?
            Dimensions.Create(entity.LengthMm.Value, entity.WidthMm.Value, entity.HeightMm.Value) : null;
        
        var weight = entity.WeightG.HasValue ? 
            Weight.Create(entity.WeightG.Value) : null;

        // Create domain entity using factory method
        var sku = Sku.CreateWithManualCode(
            productId: entity.ProductId,
            skuCode: skuCode.Value);

        // Set additional properties
        if (barcode != null)
            sku.SetBarcode(barcode.Value);
        
        if (dimensions != null)
            sku.SetDimensions(dimensions.LengthMm, dimensions.WidthMm, dimensions.HeightMm);
            
        if (weight != null)
            sku.SetWeight(weight.Grams);
            
        if (entity.IsActive)
            sku.Activate();
        else
            sku.Deactivate();

        return sku;
    }

    /// <summary>
    /// Maps domain entity to persistence entity
    /// </summary>
    /// <param name="domainEntity">Domain entity</param>
    /// <returns>Persistence entity</returns>
    private static PersistenceEntities.CatSku MapToPersistenceEntity(Sku domainEntity)
    {
        return new PersistenceEntities.CatSku
        {
            Id = domainEntity.Id,
            SkuCode = domainEntity.Code.Value,
            ProductId = domainEntity.ProductId,
            Barcode = domainEntity.BarcodeValue?.Value,
            LengthMm = domainEntity.DimensionsValue?.LengthMm,
            WidthMm = domainEntity.DimensionsValue?.WidthMm,
            HeightMm = domainEntity.DimensionsValue?.HeightMm,
            WeightG = domainEntity.WeightValue?.Grams,
            IsActive = domainEntity.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates persistence entity with domain entity values
    /// </summary>
    /// <param name="persistenceEntity">Persistence entity to update</param>
    /// <param name="domainEntity">Domain entity with new values</param>
    private static void UpdatePersistenceEntity(
        PersistenceEntities.CatSku persistenceEntity, 
        Sku domainEntity)
    {
        persistenceEntity.SkuCode = domainEntity.Code.Value;
        persistenceEntity.ProductId = domainEntity.ProductId;
        persistenceEntity.Barcode = domainEntity.BarcodeValue?.Value;
        persistenceEntity.LengthMm = domainEntity.DimensionsValue?.LengthMm;
        persistenceEntity.WidthMm = domainEntity.DimensionsValue?.WidthMm;
        persistenceEntity.HeightMm = domainEntity.DimensionsValue?.HeightMm;
        persistenceEntity.WeightG = domainEntity.WeightValue?.Grams;
        persistenceEntity.IsActive = domainEntity.IsActive;
        persistenceEntity.UpdatedAt = DateTime.UtcNow;
    }

    #endregion
}