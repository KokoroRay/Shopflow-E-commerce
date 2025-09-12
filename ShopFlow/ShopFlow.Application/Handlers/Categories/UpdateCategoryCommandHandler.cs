using MediatR;
using ShopFlow.Application.Commands.Categories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using ShopFlow.Application.Exceptions;

namespace ShopFlow.Application.Handlers.Categories;

/// <summary>
/// Handler for updating an existing category
/// </summary>
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new DomainException($"Category with ID {request.Id} not found.");
        }

        var categoryName = CategoryName.FromDisplayName(request.Name);

        // Generate slug if not provided
        var categorySlug = string.IsNullOrWhiteSpace(request.Slug)
            ? CategorySlug.FromCategoryName(categoryName)
            : new CategorySlug(request.Slug);

        // Check if name already exists (excluding current category)
        if (await _categoryRepository.ExistsByNameAsync(categoryName, request.Id, cancellationToken))
        {
            throw new DomainException($"Category with name '{request.Name}' already exists.");
        }

        // Check if slug already exists (excluding current category)
        if (await _categoryRepository.ExistsBySlugAsync(categorySlug, request.Id, cancellationToken))
        {
            throw new DomainException($"Category with slug '{categorySlug.Value}' already exists.");
        }

        // Validate parent category if specified
        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == request.Id)
            {
                throw new DomainException("A category cannot be its own parent.");
            }

            var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parentCategory == null)
            {
                throw new DomainException($"Parent category with ID {request.ParentId.Value} not found.");
            }
        }

        // Update category properties
        category.UpdateName(categoryName);
        category.UpdateSlug(categorySlug);
        category.UpdateDescription(request.Description);
        category.ChangeParent(request.ParentId);
        category.UpdateSortOrder(request.SortOrder);
        category.UpdateImages(request.ImageUrl, request.IconUrl);

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name.Value,
            Slug = category.Slug.Value,
            Description = category.Description,
            ParentId = category.ParentId,
            SortOrder = category.SortOrder,
            ImageUrl = category.ImageUrl,
            IconUrl = category.IconUrl,
            Status = (byte)category.Status,
            IsActive = category.Status == CategoryStatus.Active,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}