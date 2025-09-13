using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Exceptions;

namespace ShopFlow.Application.Handlers.Products;

/// <summary>
/// Handler for UpdateProductPricingCommand - Vietnamese marketplace multi-currency pricing
/// </summary>
public class UpdateProductPricingCommandHandler : IRequestHandler<UpdateProductPricingCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductPricingCommandHandler> _logger;

    public UpdateProductPricingCommandHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductPricingCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductResponse> Handle(UpdateProductPricingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating pricing for product {ProductId} by vendor {VendorId}",
            request.ProductId, request.VendorId);

        try
        {
            // Get the product
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).ConfigureAwait(false);

            if (product == null)
            {
                throw new DomainException($"Product with ID {request.ProductId} not found");
            }

            // TODO: In Phase 3, validate vendor ownership
            _logger.LogInformation("Vendor ownership validation will be implemented in Phase 3");

            // Validate pricing data
            foreach (var pricing in request.Pricing)
            {
                if (pricing.BasePrice < 0)
                {
                    throw new DomainException($"Base price cannot be negative for currency {pricing.CurrencyCode}");
                }

                if (pricing.SalePrice.HasValue && pricing.SalePrice < 0)
                {
                    throw new DomainException($"Sale price cannot be negative for currency {pricing.CurrencyCode}");
                }

                if (pricing.SalePrice.HasValue && pricing.SalePrice >= pricing.BasePrice)
                {
                    throw new DomainException($"Sale price must be less than base price for currency {pricing.CurrencyCode}");
                }

                // Validate Vietnamese marketplace currencies
                if (pricing.CurrencyCode != "VND" && pricing.CurrencyCode != "USD")
                {
                    throw new DomainException($"Unsupported currency {pricing.CurrencyCode}. Only VND and USD are supported");
                }
            }

            // TODO: In Phase 3, implement actual pricing update logic with pricing tables
            _logger.LogInformation("Pricing update logic will be implemented in Phase 3 with pricing tables");

            // For now, just update the product timestamp
            await _productRepository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Pricing updated for product {ProductId}. Effective date: {EffectiveDate}",
                request.ProductId, request.EffectiveDate ?? DateTime.UtcNow);

            // Map to response
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name.Value,
                Slug = product.Slug.Value,
                Status = (byte)product.Status,
                ProductType = product.ProductType,
                ReturnDays = product.ReturnDays,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                VendorId = request.VendorId,
                PrimaryLanguage = "vi"
                // Pricing will be included in response in Phase 3
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pricing for product {ProductId}", request.ProductId);
            throw;
        }
    }
}

/// <summary>
/// Handler for UpdateProductVariantsCommand - Vietnamese marketplace variants
/// </summary>
public class UpdateProductVariantsCommandHandler : IRequestHandler<UpdateProductVariantsCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductVariantsCommandHandler> _logger;

    public UpdateProductVariantsCommandHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductVariantsCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductResponse> Handle(UpdateProductVariantsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating variants for product {ProductId} by vendor {VendorId}",
            request.ProductId, request.VendorId);

        try
        {
            // Get the product
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).ConfigureAwait(false);

            if (product == null)
            {
                throw new DomainException($"Product with ID {request.ProductId} not found");
            }

            // TODO: In Phase 3, validate vendor ownership
            _logger.LogInformation("Vendor ownership validation will be implemented in Phase 3");

            // Validate variant data
            var skuCounts = request.Variants.GroupBy(v => v.Sku).Where(g => g.Count() > 1);
            if (skuCounts.Any())
            {
                var duplicateSkus = string.Join(", ", skuCounts.Select(g => g.Key));
                throw new DomainException($"Duplicate SKUs found: {duplicateSkus}");
            }

            foreach (var variant in request.Variants)
            {
                if (string.IsNullOrWhiteSpace(variant.VariantType))
                {
                    throw new DomainException("Variant type cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(variant.VariantValue))
                {
                    throw new DomainException("Variant value cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(variant.Sku))
                {
                    throw new DomainException("Variant SKU cannot be empty");
                }

                if (variant.StockQuantity < 0)
                {
                    throw new DomainException($"Stock quantity cannot be negative for variant {variant.Sku}");
                }
            }

            // TODO: In Phase 3, implement actual variant update logic with variant tables
            _logger.LogInformation("Variant update logic will be implemented in Phase 3 with variant tables");
            _logger.LogInformation("Replace existing variants: {ReplaceExisting}", request.ReplaceExisting);

            // For now, just update the product timestamp
            await _productRepository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Variants updated for product {ProductId}. Total variants: {Count}",
                request.ProductId, request.Variants.Count);

            // Map to response
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name.Value,
                Slug = product.Slug.Value,
                Status = (byte)product.Status,
                ProductType = product.ProductType,
                ReturnDays = product.ReturnDays,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                VendorId = request.VendorId,
                PrimaryLanguage = "vi"
                // Variants will be included in response in Phase 3
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating variants for product {ProductId}", request.ProductId);
            throw;
        }
    }
}

/// <summary>
/// Handler for UpdateProductI18nContentCommand - Vietnamese marketplace i18n
/// </summary>
public class UpdateProductI18nContentCommandHandler : IRequestHandler<UpdateProductI18nContentCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductI18nContentCommandHandler> _logger;

    public UpdateProductI18nContentCommandHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductI18nContentCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductResponse> Handle(UpdateProductI18nContentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating i18n content for product {ProductId} in language {Language}",
            request.ProductId, request.LanguageCode);

        try
        {
            // Get the product
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).ConfigureAwait(false);

            if (product == null)
            {
                throw new DomainException($"Product with ID {request.ProductId} not found");
            }

            // TODO: In Phase 3, validate vendor ownership
            _logger.LogInformation("Vendor ownership validation will be implemented in Phase 3");

            // Validate language code
            if (request.LanguageCode != "vi" && request.LanguageCode != "en")
            {
                throw new DomainException($"Unsupported language code {request.LanguageCode}. Only 'vi' and 'en' are supported");
            }

            // Validate content
            if (string.IsNullOrWhiteSpace(request.Content.Name))
            {
                throw new DomainException("Product name cannot be empty");
            }

            if (request.Content.Name.Length > 255)
            {
                throw new DomainException("Product name cannot exceed 255 characters");
            }

            // TODO: In Phase 3, implement actual i18n content update logic with i18n tables
            _logger.LogInformation("I18n content update logic will be implemented in Phase 3 with i18n tables");

            // For primary language (Vietnamese), update the main product name
            if (request.LanguageCode == "vi" && product.Name.Value != request.Content.Name)
            {
                // TODO: In Phase 3, implement proper name update with ProductName value object validation
                _logger.LogInformation("Primary language name update will be implemented in Phase 3");
            }

            // For now, just update the product timestamp
            await _productRepository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("I18n content updated for product {ProductId} in {Language}",
                request.ProductId, request.LanguageCode);

            // Map to response
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name.Value,
                Slug = product.Slug.Value,
                Status = (byte)product.Status,
                ProductType = product.ProductType,
                ReturnDays = product.ReturnDays,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                VendorId = request.VendorId,
                PrimaryLanguage = request.LanguageCode
                // I18n content will be included in response in Phase 3
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating i18n content for product {ProductId}", request.ProductId);
            throw;
        }
    }
}