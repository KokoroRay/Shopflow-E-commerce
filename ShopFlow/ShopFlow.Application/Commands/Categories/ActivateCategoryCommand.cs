using MediatR;

namespace ShopFlow.Application.Commands.Categories;

/// <summary>
/// Command for activating a category
/// </summary>
/// <param name="Id">The category ID to activate</param>
public record ActivateCategoryCommand(long Id) : IRequest;