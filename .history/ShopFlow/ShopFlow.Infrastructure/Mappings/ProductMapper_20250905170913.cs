using ShopFlow.Application.Abstractions.Mappings;
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
    {
        _productRepository = productRepository;
        _productI18nRepository = productI18nRepository;
        _categoryRepository = categoryRepository;
        _skuRepository = skuRepository;
    }

    public async Task<ProductResponse> MapToProductResponseAsync(long productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
        {
            throw new ArgumentException($"Product with ID {productId} not found");
        }

        var productI18n = await _productI18nRepository.FindAsync(p => p.product_id == productId, cancellationToken);
        var skus = await _skuRepository.FindAsync(s => s.product_id == productId, cancellationToken);

        var i18n = productI18n.FirstOrDefault();
        if (i18n == null)
        {
            throw new InvalidOperationException($"Product i18n not found for product ID {productId}");
        }

        return new ProductResponse
        {
            Id = product.id,
            Name = i18n.name,
            ShortDescription = i18n.short_desc,
            MetaTitle = i18n.meta_title,
            MetaDescription = i18n.meta_desc,
            Slug = i18n.slug,
            ProductType = product.product_type,
            Status = product.status,
            ReturnDays = product.return_days,
            CreatedAt = product.created_at,
            UpdatedAt = product.updated_at,
            Language = i18n.lang,
            Categories = new List<CategoryResponse>(),
            Skus = skus.Select(s => new SkuResponse
            {
                Id = s.id,
                SkuCode = s.sku_code,
                Barcode = s.barcode,
                IsActive = s.is_active,
                OptionsJson = s.options_json
            }).ToList()
        };
    }
}
