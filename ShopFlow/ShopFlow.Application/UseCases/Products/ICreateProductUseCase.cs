using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Contracts.Requests;

namespace ShopFlow.Application.UseCases.Products;

public interface ICreateProductUseCase
{
    Task<ProductResponse> ExecuteAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
}
