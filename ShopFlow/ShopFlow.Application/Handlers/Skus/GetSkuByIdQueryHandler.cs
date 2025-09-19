using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.DTOs;
using ShopFlow.Application.Exceptions;
using ShopFlow.Application.Queries.Skus;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.Handlers.Skus;

/// <summary>
/// Handler for retrieving a single SKU by ID
/// </summary>
public class GetSkuByIdQueryHandler : IRequestHandler<GetSkuByIdQuery, GetSkuResponse>
{
    private readonly ISkuRepository _skuRepository;

    /// <summary>
    /// Initializes a new instance of the GetSkuByIdQueryHandler class
    /// </summary>
    /// <param name="skuRepository">SKU repository</param>
    public GetSkuByIdQueryHandler(ISkuRepository skuRepository)
    {
        _skuRepository = skuRepository;
    }

    /// <summary>
    /// Handles the GetSkuByIdQuery request
    /// </summary>
    /// <param name="request">Query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>SKU response</returns>
    public async Task<GetSkuResponse> Handle(GetSkuByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sku = await _skuRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (sku == null)
        {
            throw new NotFoundException($"Không tìm thấy SKU với ID {request.Id}");
        }

        return new GetSkuResponse
        {
            Sku = MapToDto(sku)
        };
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