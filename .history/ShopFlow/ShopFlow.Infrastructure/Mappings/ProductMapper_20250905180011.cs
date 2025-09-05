using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Infrastructure.Mappings;

public class ProductMapper : IProductMapper
{
    public CatProduct ToDomain(CatProduct dataEntity) // Changed to use Domain entity
    {
        return dataEntity ?? throw new ArgumentNullException(nameof(dataEntity));
    }

    public CatProduct ToData(CatProduct domainEntity) // Changed to use Domain entity
    {
        return domainEntity ?? throw new ArgumentNullException(nameof(domainEntity));
    }

    public void UpdateData(CatProduct domainEntity, CatProduct dataEntity) // Changed to use Domain entity
    {
        // Update logic here
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

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(obj, value);
        }
    }
}
