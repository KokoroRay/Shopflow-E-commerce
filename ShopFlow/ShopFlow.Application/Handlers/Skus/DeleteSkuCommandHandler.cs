using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Commands.Skus;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Exceptions;

namespace ShopFlow.Application.Handlers.Skus;

/// <summary>
/// Handler for deleting SKUs with Vietnamese marketplace validation
/// </summary>
public class DeleteSkuCommandHandler : IRequestHandler<DeleteSkuCommand, DeleteSkuResponse>
{
    private readonly ISkuRepository _skuRepository;
    private readonly ILogger<DeleteSkuCommandHandler> _logger;

    public DeleteSkuCommandHandler(
        ISkuRepository skuRepository,
        ILogger<DeleteSkuCommandHandler> logger)
    {
        _skuRepository = skuRepository;
        _logger = logger;
    }

    public async Task<DeleteSkuResponse> Handle(DeleteSkuCommand request, CancellationToken cancellationToken)
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
            // Validate if SKU can be deleted
            var validationResult = await _skuRepository.ValidateDeleteAsync(request.Id, cancellationToken)
                .ConfigureAwait(false);
            
            if (!validationResult.CanDelete)
            {
                throw new ValidationException($"Không thể xóa SKU: {string.Join(", ", validationResult.ValidationErrors)}");
            }

            // Perform deletion
            var isDeleted = await _skuRepository.DeleteAsync(request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (!isDeleted)
            {
                throw new InvalidOperationException("Không thể xóa SKU. Vui lòng thử lại.");
            }

            var response = new DeleteSkuResponse
            {
                IsSuccess = true,
                Message = $"SKU '{sku.Code.Value}' đã được xóa thành công",
                DeletedId = request.Id
            };

            return response;
        }
        catch (ValidationException)
        {
            throw; // Re-throw validation exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting SKU {SkuId}", request.Id);
            throw new InvalidOperationException("Có lỗi xảy ra khi xóa SKU. Vui lòng thử lại.");
        }
    }
}