using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Domain.Entities; // (Optional explicit; GlobalUsings already covers it if present)

namespace ShopFlow.Application.UseCases.Products;

public class GetProductUseCase : IGetProductUseCase
{
    private readonly IRepository<cat_product> _productRepository;
    private readonly IProductMapper _productMapper;

    public GetProductUseCase(
        IRepository<cat_product> productRepository,
        IProductMapper productMapper)
    {
        _productRepository = productRepository;
        _productMapper = productMapper;
    }

    public async Task<ProductResponse?> ExecuteAsync(long id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return null;

        return await _productMapper.MapToProductResponseAsync(id, cancellationToken);
    }
}