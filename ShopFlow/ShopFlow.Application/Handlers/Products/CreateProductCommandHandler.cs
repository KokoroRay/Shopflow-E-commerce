using MediatR;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;
using ShopFlow.Application.Exceptions;

namespace ShopFlow.Application.Handlers.Products;

/// <summary>
/// Handler for creating a new product in the Vietnamese marketplace
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of CreateProductCommandHandler
    /// </summary>
    /// <param name="productRepository">Product repository</param>
    /// <param name="categoryRepository">Category repository</param>
    /// <param name="unitOfWork">Unit of work</param>
    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the create product command
    /// </summary>
    /// <param name="request">Create product command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product response</returns>
    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validate input
        await ValidateRequestAsync(request, cancellationToken).ConfigureAwait(false);

        // Create value objects for validation (even though current entity doesn't use them yet)
        var productName = ProductName.FromDisplayName(request.Name);
        _ = string.IsNullOrWhiteSpace(request.Slug)
            ? ProductSlug.FromProductName(productName)
            : ProductSlug.FromString(request.Slug);

        // Check for duplicates - Note: Placeholder for future slug validation logic
        // In Phase 2, this would check vendor-specific uniqueness and global slug uniqueness

        // Create the product entity
        var product = CreateProductEntity(request);

        // Add categories
        await AddCategoriesToProductAsync(product, request.CategoryIds, cancellationToken).ConfigureAwait(false);

        // Save to repository
        await _productRepository.AddAsync(product, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Return response
        return MapToResponse(product, request);
    }

    private async Task ValidateRequestAsync(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Validate categories exist
        foreach (var categoryId in request.CategoryIds)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken).ConfigureAwait(false);
            if (category == null)
            {
                throw new DomainException($"Category with ID {categoryId} not found.");
            }
        }

        // Validate language codes
        if (!IsValidLanguageCode(request.PrimaryLanguage))
        {
            throw new DomainException($"Invalid primary language code: {request.PrimaryLanguage}. Must be 'vi' or 'en'.");
        }

        // Validate currency codes in pricing
        foreach (var pricing in request.Pricing)
        {
            if (!IsValidCurrencyCode(pricing.CurrencyCode))
            {
                throw new DomainException($"Invalid currency code: {pricing.CurrencyCode}. Must be 'VND' or 'USD'.");
            }

            if (pricing.BasePrice <= 0)
            {
                throw new DomainException($"Base price must be greater than 0 for currency {pricing.CurrencyCode}.");
            }

            if (pricing.SalePrice.HasValue && pricing.SalePrice.Value >= pricing.BasePrice)
            {
                throw new DomainException($"Sale price must be less than base price for currency {pricing.CurrencyCode}.");
            }
        }

        // Validate variants if specified
        if (request.Variants?.Count > 0)
        {
            foreach (var variant in request.Variants)
            {
                if (variant.StockQuantity < 0)
                {
                    throw new DomainException($"Stock quantity cannot be negative for variant {variant.VariantType}:{variant.VariantValue}.");
                }
            }

            // Ensure at most one default variant
            var defaultVariants = request.Variants.Where(v => v.IsDefault).ToList();
            if (defaultVariants.Count > 1)
            {
                throw new DomainException("Only one variant can be marked as default.");
            }
        }

        // Validate Vietnamese tax compliance
        if (request.VatRate.HasValue && (request.VatRate.Value < 0 || request.VatRate.Value > 1))
        {
            throw new DomainException("VAT rate must be between 0 and 1 (0% to 100%).");
        }
    }

    private static CatProduct CreateProductEntity(CreateProductCommand request)
    {
        // Create value objects
        var productName = ProductName.FromDisplayName(request.Name);
        var productSlug = ProductSlug.FromString(request.Slug);

        // Create basic product using existing constructor
        var product = new CatProduct(
            productName,
            productSlug,
            request.ProductType,
            request.ReturnDays ?? 15 // Default 15 days for Vietnamese marketplace
        );

        // Note: The current CatProduct entity doesn't have all the marketplace-specific fields
        // In Phase 2, we would extend the entity or create a new Product entity that includes:
        // - Name/Slug value objects
        // - Vendor assignment
        // - Multi-language support
        // - Multi-currency pricing
        // - Vietnamese tax compliance
        // - Approval workflow
        // 
        // For now, we're creating a basic product that follows the existing pattern

        return product;
    }

    private async Task AddCategoriesToProductAsync(CatProduct product, IReadOnlyCollection<long> categoryIds, CancellationToken cancellationToken)
    {
        foreach (var categoryId in categoryIds)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken).ConfigureAwait(false);
            if (category != null)
            {
                product.AddCategory(category);
            }
        }
    }

    private static ProductResponse MapToResponse(CatProduct product, CreateProductCommand request)
    {
        // Map to response with available fields
        // Note: Many marketplace-specific fields are not yet available in the current ProductResponse
        // This mapping will be enhanced in Phase 2 when DTOs are updated
        return new ProductResponse
        {
            Id = product.Id,
            Name = request.Name, // Using request name since entity doesn't have name property yet
            Slug = request.Slug, // Using request slug since entity doesn't have slug property yet
            ShortDescription = request.ShortDescription,
            ProductType = product.ProductType,
            Status = (byte)product.Status,
            ReturnDays = product.ReturnDays,
            VendorId = request.VendorId,
            Language = request.PrimaryLanguage,
            MetaTitle = request.MetaTitle,
            MetaDescription = request.MetaDescription,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Categories = new List<CategoryResponse>(), // Will be populated when category mapping is available
            Skus = new List<SkuResponse>() // Will be populated when SKUs are created
        };
    }

    private static bool IsValidLanguageCode(string languageCode) =>
        languageCode is "vi" or "en";

    private static bool IsValidCurrencyCode(string currencyCode) =>
        currencyCode is "VND" or "USD";
}