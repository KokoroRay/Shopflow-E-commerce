using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.UseCases.Products;

public class CreateProductUseCase : ICreateProductUseCase
{
    private readonly IRepository<cat_product> _productRepository;
    private readonly IRepository<cat_product_i18n> _productI18nRepository;
    private readonly IRepository<cat_category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductMapper _productMapper;

    public CreateProductUseCase(
        IRepository<cat_product> productRepository,
        IRepository<cat_product_i18n> productI18nRepository,
        IRepository<cat_category> categoryRepository,
        IUnitOfWork unitOfWork,
        IProductMapper productMapper)
    {
        _productRepository = productRepository;
        _productI18nRepository = productI18nRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _productMapper = productMapper;
    }

    public async Task<ProductResponse> ExecuteAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var product = new cat_product
            {
                product_type = request.ProductType,   // fixed: assign byte directly
                status = 1,
                return_days = request.ReturnDays,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.AddAsync(product, cancellationToken);

            var productI18n = new cat_product_i18n
            {
                product_id = createdProduct.id,
                lang = request.Language,
                name = request.Name,
                short_desc = request.ShortDescription,
                slug = request.Slug,
                meta_title = request.MetaTitle,
                meta_desc = request.MetaDescription
            };

            await _productI18nRepository.AddAsync(productI18n, cancellationToken);

            if (request.CategoryIds.Any())
            {
                // You may later implement relation insert (join table) here.
                var _ = await _categoryRepository.FindAsync(c => request.CategoryIds.Contains(c.id), cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return await _productMapper.MapToProductResponseAsync(createdProduct.id, cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}


