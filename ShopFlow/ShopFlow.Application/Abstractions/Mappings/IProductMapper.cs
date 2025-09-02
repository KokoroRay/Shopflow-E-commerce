using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Abstractions.Mappings;

public interface IProductMapper
{
    Task<ProductResponse> MapToProductResponseAsync(long productId, CancellationToken cancellationToken = default);
}


