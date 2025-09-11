using MediatR;
using ShopFlow.Application.Commands.Categories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using ShopFlow.Application.Exceptions;

namespace ShopFlow.Application.Handlers.Categories;

/// <summary>
/// Handler for creating a new category
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryName = CategoryName.FromDisplayName(request.Name);

        // Generate slug if not provided
        var categorySlug = string.IsNullOrWhiteSpace(request.Slug)
            ? CategorySlug.FromCategoryName(categoryName)
            : new CategorySlug(request.Slug);

        // Check if name already exists
        if (await _categoryRepository.ExistsByNameAsync(categoryName, cancellationToken))
        {
            throw new DomainException($"Category with name '{request.Name}' already exists.");
        }

        // Check if slug already exists
        if (await _categoryRepository.ExistsBySlugAsync(categorySlug, cancellationToken))
        {
            throw new DomainException($"Category with slug '{categorySlug.Value}' already exists.");
        }

        // Validate parent category if specified
        if (request.ParentId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parentCategory == null)
            {
                throw new DomainException($"Parent category with ID {request.ParentId.Value} not found.");
            }
        }

        var category = new CatCategory(
            categoryName,
            categorySlug,
            request.Description,
            request.ParentId,
            request.SortOrder
        );

        // Update image URLs if provided
        if (!string.IsNullOrWhiteSpace(request.ImageUrl) || !string.IsNullOrWhiteSpace(request.IconUrl))
        {
            category.UpdateImages(request.ImageUrl, request.IconUrl);
        }

        // Set status based on IsActive flag
        if (!request.IsActive)
        {
            category.Deactivate();
        }

        await _categoryRepository.AddAsync(category, cancellationToken);
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