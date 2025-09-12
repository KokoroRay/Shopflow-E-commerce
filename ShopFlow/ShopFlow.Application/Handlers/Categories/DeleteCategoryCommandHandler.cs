using MediatR;
using ShopFlow.Application.Commands.Categories;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Exceptions;

namespace ShopFlow.Application.Handlers.Categories;

/// <summary>
/// Handler for deleting a category
/// </summary>
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new DomainException($"Category with ID {request.Id} not found.");
        }

        // Check if category has children
        var children = await _categoryRepository.GetChildrenAsync(request.Id, cancellationToken);
        if (children.Any())
        {
            throw new DomainException("Cannot delete a category that has child categories.");
        }

        // Soft delete by marking as deleted
        category.Delete();

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}