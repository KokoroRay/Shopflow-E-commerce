namespace ShopFlow.Application.Contracts.Requests;

/// <summary>
/// Request for updating product approval status in Vietnamese marketplace
/// </summary>
/// <param name="IsApproved">Whether the product is approved</param>
/// <param name="AdminNotes">Admin notes about the decision</param>
/// <param name="RejectionReason">Reason for rejection if not approved</param>
public record ProductApprovalRequest(
    bool IsApproved,
    string? AdminNotes,
    string? RejectionReason
);

/// <summary>
/// Request for bulk updating product statuses
/// </summary>
/// <param name="ProductIds">Collection of product IDs to update</param>
/// <param name="NewStatus">New status to apply</param>
/// <param name="AdminNotes">Admin notes about the bulk update</param>
public record BulkUpdateStatusRequest(
    IReadOnlyCollection<long> ProductIds,
    byte NewStatus,
    string? AdminNotes
);