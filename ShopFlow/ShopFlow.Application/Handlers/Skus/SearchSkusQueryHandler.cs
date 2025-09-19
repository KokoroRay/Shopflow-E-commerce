using MediatR;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.DTOs;
using ShopFlow.Application.Queries.Skus;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.Handlers.Skus;

/// <summary>
/// Handler for searching SKUs with pagination and filtering
/// </summary>
public class SearchSkusQueryHandler : IRequestHandler<SearchSkusQuery, GetSkusResponse>
{
    private readonly ISkuRepository _skuRepository;

    /// <summary>
    /// Initializes a new instance of the SearchSkusQueryHandler class
    /// </summary>
    /// <param name="skuRepository">SKU repository</param>
    public SearchSkusQueryHandler(ISkuRepository skuRepository)
    {
        _skuRepository = skuRepository;
    }

    /// <summary>
    /// Handles the SearchSkusQuery request
    /// </summary>
    /// <param name="request">Query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated SKU list response</returns>
    public async Task<GetSkusResponse> Handle(SearchSkusQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var skus = await _skuRepository.SearchAsync(
            searchTerm: request.SearchTerm,
            searchFields: request.SearchFields,
            filters: request.Filters,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var skuList = skus.ToList();
        var totalCount = skuList.Count; // For simple implementation

        var skuDtos = skuList.Select(MapToDto).ToList();

        return new GetSkusResponse
        {
            Skus = skuDtos,
            TotalCount = totalCount,
            Page = request.Filters.Page,
            PageSize = request.Filters.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.Filters.PageSize)
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