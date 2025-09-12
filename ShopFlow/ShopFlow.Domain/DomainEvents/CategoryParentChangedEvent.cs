namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a category parent is changed
/// </summary>
public class CategoryParentChangedEvent : DomainEvent
{
    public long CategoryId { get; }
    public long? OldParentId { get; }
    public long? NewParentId { get; }

    public CategoryParentChangedEvent(long categoryId, long? oldParentId, long? newParentId)
    {
        CategoryId = categoryId;
        OldParentId = oldParentId;
        NewParentId = newParentId;
    }
}