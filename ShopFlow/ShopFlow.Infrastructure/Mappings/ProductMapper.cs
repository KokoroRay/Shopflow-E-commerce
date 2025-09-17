using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using PersistenceEntities = ShopFlow.Infrastructure.Persistence.Entities;

namespace ShopFlow.Infrastructure.Mappings;

public class ProductMapper : IDomainPersistenceMapper<CatProduct, PersistenceEntities.CatProduct>, IProductMapper
{
    public CatProduct ToDomain(PersistenceEntities.CatProduct dataEntity)
    {
        if (dataEntity == null) return null;

        var domain = new CatProduct(
            ProductName.FromDisplayName(dataEntity.Name),
            ProductSlug.FromString(dataEntity.Slug),
            dataEntity.ProductType,
            dataEntity.ReturnDays
        );

        // Use reflection to set private properties
        SetPrivateProperty(domain, "Id", dataEntity.Id);
        SetPrivateProperty(domain, "Status", (ProductStatus)dataEntity.Status);
        SetPrivateProperty(domain, "CreatedAt", dataEntity.CreatedAt);
        SetPrivateProperty(domain, "UpdatedAt", dataEntity.UpdatedAt);

        return domain;
    }

    public PersistenceEntities.CatProduct ToPersistence(CatProduct domainEntity)
    {
        if (domainEntity == null) return null;

        return new PersistenceEntities.CatProduct
        {
            Id = domainEntity.Id,
            Name = domainEntity.Name.Value,
            Slug = domainEntity.Slug.Value,
            ProductType = domainEntity.ProductType,
            Status = (byte)domainEntity.Status,
            ReturnDays = domainEntity.ReturnDays,
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt
        };
    }

    public void UpdatePersistence(CatProduct domainEntity, PersistenceEntities.CatProduct persistenceEntity)
    {
        if (domainEntity == null || persistenceEntity == null) return;

        persistenceEntity.Name = domainEntity.Name.Value;
        persistenceEntity.Slug = domainEntity.Slug.Value;
        persistenceEntity.ProductType = domainEntity.ProductType;
        persistenceEntity.Status = (byte)domainEntity.Status;
        persistenceEntity.ReturnDays = domainEntity.ReturnDays;
        persistenceEntity.UpdatedAt = domainEntity.UpdatedAt;
    }

    public async Task<ProductResponse> MapToProductResponseAsync(long productId, CancellationToken cancellationToken = default)
    {
        // Temporary implementation
        return new ProductResponse
        {
            Id = productId,
            Name = "Sample Product",
            Categories = new List<CategoryResponse>(),
            Skus = new List<SkuResponse>()
        };
    }

    // Keep this method as it might be used elsewhere or for complex scenarios
    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value, null);
        }
    }
}
