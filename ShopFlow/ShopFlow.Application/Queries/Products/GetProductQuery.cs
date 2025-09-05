using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Queries.Products;

public record GetProductQuery(long ProductId) : IRequest<ProductResponse?>;
