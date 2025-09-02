using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;
using ShopFlow.Application.Abstractions.Mappings;

namespace ShopFlow.Infrastructure.Mappings;

public class ProductMapper : IProductMapper
{
    private readonly IRepository<cat_product> _productRepository;
    private readonly IRepository<cat_product_i18n> _productI18nRepository;
    private readonly IRepository<cat_category> _categoryRepository;
    private readonly IRepository<cat_sku> _skuRepository;

    public ProductMapper(
        IRepository<cat_product> productRepository,
        IRepository<cat_product_i18n> productI18nRepository,
        IRepository<cat_category> categoryRepository,
        IRepository<cat_sku> skuRepository)
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
