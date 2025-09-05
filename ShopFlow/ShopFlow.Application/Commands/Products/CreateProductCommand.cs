using MediatR;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Commands.Products;

public record CreateProductCommand(
    byte ProductType,
    int? ReturnDays,
    string Language,
    string Name,
    string? ShortDescription,
    string Slug,
    string? MetaTitle,
    string? MetaDescription,
    List<long> CategoryIds
) : IRequest<ProductResponse>;
