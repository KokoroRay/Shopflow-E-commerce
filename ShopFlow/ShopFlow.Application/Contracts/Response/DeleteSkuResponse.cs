namespace ShopFlow.Application.Contracts.Response;

/// <summary>
/// Response model for deleting SKU operations
/// </summary>
public class DeleteSkuResponse
{
    /// <summary>
    /// Indicates whether the delete operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Result message describing the delete operation
    /// </summary>
    public string ResultMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the deleted SKU
    /// </summary>
    public long DeletedId { get; set; }
}