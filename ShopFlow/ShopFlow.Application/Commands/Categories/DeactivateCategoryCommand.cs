using MediatR;

namespace ShopFlow.Application.Commands.Categories;

/// <summary>
/// Command for deactivating a category
/// </summary>
/// <param name="Id">The category ID to deactivate</param>
public record DeactivateCategoryCommand(long Id) : IRequest;