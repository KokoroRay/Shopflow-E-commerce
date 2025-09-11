using MediatR;

namespace ShopFlow.Application.Commands.Categories;

/// <summary>
/// Command for deleting a category
/// </summary>
/// <param name="Id">The category ID to delete</param>
public record DeleteCategoryCommand(long Id) : IRequest;