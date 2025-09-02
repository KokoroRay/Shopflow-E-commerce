using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.UseCases.Products;

public interface IGetProductUseCase
{
    Task<ProductResponse?> ExecuteAsync(long id, CancellationToken cancellationToken = default);
}
