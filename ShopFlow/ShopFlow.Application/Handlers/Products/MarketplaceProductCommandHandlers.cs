using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.Exceptions;

namespace ShopFlow.Application.Handlers.Products;

/// <summary>
/// Handler for UpdateProductStatusCommand - Vietnamese marketplace status management
/// </summary>
public class UpdateProductStatusCommandHandler : IRequestHandler<UpdateProductStatusCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductStatusCommandHandler> _logger;

    public UpdateProductStatusCommandHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductStatusCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductResponse> Handle(UpdateProductStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product {ProductId} status to {Status}",
            request.ProductId, request.NewStatus);

        try
        {
            // Get the product
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).ConfigureAwait(false);

            if (product == null)
            {
                throw new DomainException($"Product with ID {request.ProductId} not found");
            }

            // Validate status transition
            if (!IsValidStatusTransition(product.Status, request.NewStatus))
            {
                throw new DomainException($"Invalid status transition from {product.Status} to {request.NewStatus}");
            }

            // Update status based on the new status
            switch (request.NewStatus)
            {
                case ProductStatus.Active:
                    product.Activate();
                    break;
                case ProductStatus.Inactive:
                    product.Deactivate();
                    break;
                case ProductStatus.Discontinued:
                    product.Discontinue();
                    break;
                default:
                    // For other statuses, we'll implement specific methods in Phase 3
                    _logger.LogInformation("Status {Status} requires special handling - will be implemented in Phase 3", request.NewStatus);
                    break;
            }

            // Save changes
            await _productRepository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

            // Log admin notes if provided
            if (!string.IsNullOrEmpty(request.AdminNotes))
            {
                _logger.LogInformation("Admin notes for product {ProductId}: {Notes}",
                    request.ProductId, request.AdminNotes);
            }

            // TODO: In Phase 3, implement vendor notification if requested
            if (request.NotifyVendor)
            {
                _logger.LogInformation("Vendor notification will be implemented in Phase 3");
            }

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
                // Additional properties will be mapped in Phase 3
                VendorId = 1, // Placeholder
                PrimaryLanguage = "vi"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId} status", request.ProductId);
            throw;
        }
    }

    private static bool IsValidStatusTransition(ProductStatus currentStatus, ProductStatus newStatus)
    {
        // Vietnamese marketplace status transition rules
        return (currentStatus, newStatus) switch
        {
            (ProductStatus.Draft, ProductStatus.Pending) => true,
            (ProductStatus.Pending, ProductStatus.UnderReview) => true,
            (ProductStatus.UnderReview, ProductStatus.Active) => true,
            (ProductStatus.UnderReview, ProductStatus.Rejected) => true,
            (ProductStatus.Active, ProductStatus.Inactive) => true,
            (ProductStatus.Inactive, ProductStatus.Active) => true,
            (ProductStatus.Active, ProductStatus.Discontinued) => true,
            (ProductStatus.Rejected, ProductStatus.Draft) => true,
            _ => false
        };
    }
}

/// <summary>
/// Handler for ApproveRejectProductCommand - Vietnamese marketplace approval workflow
/// </summary>
public class ApproveRejectProductCommandHandler : IRequestHandler<ApproveRejectProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ApproveRejectProductCommandHandler> _logger;

    public ApproveRejectProductCommandHandler(
        IProductRepository productRepository,
        ILogger<ApproveRejectProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductResponse> Handle(ApproveRejectProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing approval for product {ProductId}: {Decision}",
            request.ProductId, request.IsApproved ? "APPROVED" : "REJECTED");

        try
        {
            // Get the product
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).ConfigureAwait(false);

            if (product == null)
            {
                throw new DomainException($"Product with ID {request.ProductId} not found");
            }

            // Validate current status allows approval/rejection
            if (product.Status != ProductStatus.Pending && product.Status != ProductStatus.UnderReview)
            {
                throw new DomainException($"Product status {product.Status} does not allow approval/rejection");
            }

            // Process approval decision
            if (request.IsApproved)
            {
                product.Activate(); // Approved products become active
                _logger.LogInformation("Product {ProductId} approved and activated", request.ProductId);
            }
            else
            {
                // For rejection, we need to implement a reject method in Phase 3
                // For now, we'll use deactivate as placeholder
                product.Deactivate();
                _logger.LogInformation("Product {ProductId} rejected. Reason: {Reason}",
                    request.ProductId, request.RejectionReason ?? "No reason provided");
            }

            // Save changes
            await _productRepository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

            // Log admin notes and decision
            if (!string.IsNullOrEmpty(request.AdminNotes))
            {
                _logger.LogInformation("Admin notes for product {ProductId}: {Notes}",
                    request.ProductId, request.AdminNotes);
            }

            // TODO: In Phase 3, implement vendor notification
            _logger.LogInformation("Vendor notification for approval decision will be implemented in Phase 3");

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
                VendorId = 1, // Placeholder
                PrimaryLanguage = "vi"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing approval for product {ProductId}", request.ProductId);
            throw;
        }
    }
}

/// <summary>
/// Handler for BulkUpdateProductsCommand - Vietnamese marketplace bulk operations
/// </summary>
public class BulkUpdateProductsCommandHandler : IRequestHandler<BulkUpdateProductsCommand, BulkUpdateProductsResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<BulkUpdateProductsCommandHandler> _logger;

    public BulkUpdateProductsCommandHandler(
        IProductRepository productRepository,
        ILogger<BulkUpdateProductsCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<BulkUpdateProductsResponse> Handle(BulkUpdateProductsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing bulk update for {Count} products to status {Status}",
            request.ProductIds.Count, request.NewStatus);

        var updatedIds = new List<long>();
        var failedIds = new List<long>();
        var errorMessages = new List<string>();

        try
        {
            foreach (var productId in request.ProductIds)
            {
                try
                {
                    var product = await _productRepository.GetByIdAsync(productId, cancellationToken).ConfigureAwait(false);

                    if (product == null)
                    {
                        failedIds.Add(productId);
                        errorMessages.Add($"Product {productId} not found");
                        continue;
                    }

                    // Apply status change
                    switch (request.NewStatus)
                    {
                        case ProductStatus.Active:
                            product.Activate();
                            break;
                        case ProductStatus.Inactive:
                            product.Deactivate();
                            break;
                        case ProductStatus.Discontinued:
                            product.Discontinue();
                            break;
                        default:
                            failedIds.Add(productId);
                            errorMessages.Add($"Unsupported status {request.NewStatus} for product {productId}");
                            continue;
                    }

                    await _productRepository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);
                    updatedIds.Add(productId);

                    _logger.LogDebug("Successfully updated product {ProductId} to status {Status}",
                        productId, request.NewStatus);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update product {ProductId}", productId);
                    failedIds.Add(productId);
                    errorMessages.Add($"Error updating product {productId}: {ex.Message}");
                }
            }

            // Log admin notes if provided
            if (!string.IsNullOrEmpty(request.AdminNotes))
            {
                _logger.LogInformation("Bulk update admin notes: {Notes}", request.AdminNotes);
            }

            // TODO: In Phase 3, implement vendor notifications if requested
            if (request.NotifyVendors)
            {
                _logger.LogInformation("Vendor notifications for bulk update will be implemented in Phase 3");
            }

            var response = new BulkUpdateProductsResponse
            {
                UpdatedCount = updatedIds.Count,
                FailedCount = failedIds.Count,
                UpdatedProductIds = updatedIds,
                FailedProductIds = failedIds,
                ErrorMessages = errorMessages
            };

            _logger.LogInformation("Bulk update completed: {Updated} successful, {Failed} failed",
                response.UpdatedCount, response.FailedCount);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during bulk update operation");
            throw;
        }
    }
}

/// <summary>
/// Handler for EditProductCommand - Vietnamese marketplace product editing
/// </summary>
public class EditProductCommandHandler : IRequestHandler<EditProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<EditProductCommandHandler> _logger;

    public EditProductCommandHandler(
        IProductRepository productRepository,
        ILogger<EditProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductResponse> Handle(EditProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Editing product {ProductId} by vendor {VendorId}",
            request.ProductId, request.VendorId);

        try
        {
            // Get the product
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).ConfigureAwait(false);

            if (product == null)
            {
                return new ProductResponse
                {
                    Success = false,
                    Message = $"Product with ID {request.ProductId} not found"
                };
            }

            // Validate that the product can be edited (business rules)
            if (product.Status == ProductStatus.Discontinued)
            {
                return new ProductResponse
                {
                    Success = false,
                    Message = "Cannot edit a discontinued product"
                };
            }

            // Create updated product name and slug
            var productName = ShopFlow.Domain.ValueObjects.ProductName.FromDisplayName(request.Name);
            var productSlug = ShopFlow.Domain.ValueObjects.ProductSlug.FromProductName(productName);

            // Update product details using domain method
            product.UpdateProductDetails(
                name: productName,
                slug: productSlug,
                productType: request.ProductType,
                returnDays: request.ReturnDays
            );

            // Save changes
            await _productRepository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Successfully updated product {ProductId}", request.ProductId);

            return new ProductResponse
            {
                Success = true,
                Message = "Product updated successfully",
                Id = product.Id,
                Name = product.Name.Value,
                Slug = product.Slug.Value,
                Status = (byte)product.Status,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error while editing product {ProductId}", request.ProductId);
            return new ProductResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while editing product {ProductId}", request.ProductId);
            return new ProductResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while editing product {ProductId}", request.ProductId);
            return new ProductResponse
            {
                Success = false,
                Message = "An unexpected error occurred while updating the product"
            };
        }
    }
}

/// <summary>
/// Handler for DeleteProductCommand - Vietnamese marketplace product deletion
/// </summary>
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product {ProductId}", request.ProductId);

        try
        {
            // Get the product
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).ConfigureAwait(false);

            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found", request.ProductId);
                return new ProductResponse
                {
                    Success = false,
                    Message = $"Product with ID {request.ProductId} not found"
                };
            }

            // Check if product can be deleted
            if (!product.CanBeDeleted())
            {
                _logger.LogWarning("Product {ProductId} cannot be deleted - already discontinued", request.ProductId);
                return new ProductResponse
                {
                    Success = false,
                    Message = "Product is already discontinued and cannot be deleted"
                };
            }

            // Delete the product (set to discontinued status)
            product.Delete();

            // Save changes
            await _productRepository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);

            // Log admin notes if provided
            if (!string.IsNullOrEmpty(request.AdminNotes))
            {
                _logger.LogInformation("Admin notes for product {ProductId} deletion: {Notes}",
                    request.ProductId, request.AdminNotes);
            }

            // TODO: In Phase 3, implement vendor notification if requested
            if (request.NotifyVendor)
            {
                _logger.LogInformation("Vendor notification for product deletion will be implemented in Phase 3");
            }

            _logger.LogInformation("Product {ProductId} successfully deleted (discontinued)", request.ProductId);

            // Return success response
            return new ProductResponse
            {
                Success = true,
                Message = "Product deleted successfully",
                Id = product.Id,
                Name = product.Name.Value,
                Slug = product.Slug.Value,
                Status = (byte)product.Status,
                ProductType = product.ProductType,
                ReturnDays = product.ReturnDays,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                VendorId = request.VendorId ?? 1,
                PrimaryLanguage = "vi"
            };
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while deleting product {ProductId}", request.ProductId);
            return new ProductResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting product {ProductId}", request.ProductId);
            return new ProductResponse
            {
                Success = false,
                Message = "An unexpected error occurred while deleting the product"
            };
        }
    }
}