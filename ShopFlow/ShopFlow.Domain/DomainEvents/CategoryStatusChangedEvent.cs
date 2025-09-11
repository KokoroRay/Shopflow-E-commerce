using ShopFlow.Domain.Enums;

namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a category status is changed
/// </summary>
public class CategoryStatusChangedEvent : DomainEvent
{
    public long CategoryId { get; }
    public CategoryStatus OldStatus { get; }
    public CategoryStatus NewStatus { get; }

    public CategoryStatusChangedEvent(long categoryId, CategoryStatus oldStatus, CategoryStatus newStatus)
    {
        CategoryId = categoryId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}