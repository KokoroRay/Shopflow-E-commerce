using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Commands.Skus;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.DTOs;
using ShopFlow.Application.Exceptions;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Exceptions;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Handlers.Skus;

/// <summary>
/// Handler for updating existing SKUs with Vietnamese marketplace validation
/// </summary>
public class UpdateSkuCommandHandler : IRequestHandler<UpdateSkuCommand, UpdateSkuResponse>
{
    private readonly ISkuRepository _skuRepository;
    private readonly ILogger<UpdateSkuCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateSkuCommandHandler class
    /// </summary>
    /// <param name="skuRepository">SKU repository</param>
    /// <param name="logger">Logger</param>
    public UpdateSkuCommandHandler(
        ISkuRepository skuRepository,
        ILogger<UpdateSkuCommandHandler> logger)
    {
        _skuRepository = skuRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateSkuCommand request
    /// </summary>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update response</returns>
    public async Task<UpdateSkuResponse> Handle(UpdateSkuCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Get existing SKU
        var sku = await _skuRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        if (sku == null)
        {
            throw new NotFoundException($"Không tìm thấy SKU với ID {request.Id}");
        }

        try
        {
            // Update dimensions if provided
            if (request.LengthMm.HasValue && request.WidthMm.HasValue && request.HeightMm.HasValue)
            {
                var lengthMm = (int)Math.Round(request.LengthMm.Value);
                var widthMm = (int)Math.Round(request.WidthMm.Value);
                var heightMm = (int)Math.Round(request.HeightMm.Value);
                sku.SetDimensions(lengthMm, widthMm, heightMm);
            }

            // Update weight if provided
            if (request.WeightGrams.HasValue)
            {
                var weightGrams = (int)Math.Round(request.WeightGrams.Value);
                sku.SetWeight(weightGrams);
            }

            // Update cost price if provided
            if (request.CostPrice.HasValue && request.CostPrice.Value > 0)
            {
                sku.SetCost(request.CostPrice.Value, "VND");
            }

            // Update status if provided
            if (request.IsActive.HasValue && request.IsActive.Value != sku.IsActive)
            {
                if (request.IsActive.Value)
                {
                    sku.Activate();
                }
                else
                {
                    sku.Deactivate();
                }
            }

            // Save changes
            var updatedSku = await _skuRepository.UpdateAsync(sku, cancellationToken)
                .ConfigureAwait(false);

            var response = new UpdateSkuResponse
            {
                Sku = MapToDto(updatedSku),
                Message = $"SKU '{updatedSku.Code.Value}' đã được cập nhật thành công"
            };

            return response;
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            throw new ValidationException($"Lỗi validation: {ex.Message}");
        }
        catch (ValidationException)
        {
            throw; // Re-throw validation exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating SKU {SkuId}", request.Id);
            throw new InvalidOperationException("Có lỗi xảy ra khi cập nhật SKU. Vui lòng thử lại.");
        }
    }

    /// <summary>
    /// Maps SKU entity to DTO
    /// </summary>
    private static SkuDto MapToDto(Sku sku)
    {
        return new SkuDto
        {
            Id = sku.Id,
            Code = sku.Code.Value,
            Barcode = sku.BarcodeValue?.Value ?? string.Empty,
            BarcodeType = sku.BarcodeValue?.Type.ToString() ?? string.Empty,
            Name = $"SKU {sku.Code.Value}",
            Description = $"SKU variant for product {sku.ProductId}",
            Price = 0, // SKU doesn't have price, it's on Product level
            CompareAtPrice = 0,
            CostPrice = sku.CostPerUnit?.Amount ?? 0,
            IsActive = sku.IsActive,
            ProductId = sku.ProductId,
            Dimensions = new DimensionsDto
            {
                LengthMm = sku.DimensionsValue?.LengthMm ?? 0,
                WidthMm = sku.DimensionsValue?.WidthMm ?? 0,
                HeightMm = sku.DimensionsValue?.HeightMm ?? 0
            },
            Weight = new WeightDto
            {
                Grams = sku.WeightValue?.Grams ?? 0
            },
            CreatedAt = sku.CreatedAt,
            UpdatedAt = sku.UpdatedAt,
            Media = sku.Media.Select(m => new SkuMediaDto
            {
                Id = m.Id,
                SkuId = m.SkuId,
                MediaType = m.MediaType,
                Url = m.Url.ToString(),
                DisplayOrder = m.DisplayOrder,
                IsDefault = m.IsDefault
            }).ToList(),
            OptionValues = sku.OptionValues.Select(ov => new SkuOptionValueDto
            {
                SkuId = ov.SkuId,
                AttributeId = ov.AttributeId,
                OptionId = ov.OptionId,
                AttributeName = ov.Attribute?.Code ?? string.Empty,
                OptionValue = ov.Option?.Code ?? string.Empty
            }).ToList()
        };
    }
}