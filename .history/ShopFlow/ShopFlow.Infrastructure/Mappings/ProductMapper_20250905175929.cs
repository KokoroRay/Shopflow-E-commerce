using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Infrastructure.Mappings;

public class ProductMapper : IProductMapper
{
    public CatProduct ToDomain(cat_product dataEntity)
    {
        if (dataEntity == null) throw new ArgumentNullException(nameof(dataEntity));

        // For now, create a simple mapping
        // This would need to be expanded based on your CatProduct domain entity structure
        var product = (CatProduct)Activator.CreateInstance(typeof(CatProduct), true)!;
        
        SetPrivateProperty(product, nameof(CatProduct.Id), dataEntity.Id);
        // Add other property mappings as needed
        
        return product;
    }

    public cat_product ToData(CatProduct domainEntity)
    {
        if (domainEntity == null) throw new ArgumentNullException(nameof(domainEntity));

        return new cat_product
        {
            Id = domainEntity.Id,
            // Add other property mappings as needed
        };
    }

    public void UpdateData(CatProduct domainEntity, cat_product dataEntity)
    {
        if (domainEntity == null) throw new ArgumentNullException(nameof(domainEntity));
        if (dataEntity == null) throw new ArgumentNullException(nameof(dataEntity));

        // Update data entity with domain entity values
        // Add property mappings as needed
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
