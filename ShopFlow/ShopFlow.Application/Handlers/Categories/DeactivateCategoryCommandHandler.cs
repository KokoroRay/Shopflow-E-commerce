using MediatR;
using ShopFlow.Application.Commands.Categories;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Exceptions;

namespace ShopFlow.Application.Handlers.Categories;

/// <summary>
/// Handler for deactivating a category
/// </summary>
public class DeactivateCategoryCommandHandler : IRequestHandler<DeactivateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeactivateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new DomainException($"Category with ID {request.Id} not found.");
        }

        category.Deactivate();

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}