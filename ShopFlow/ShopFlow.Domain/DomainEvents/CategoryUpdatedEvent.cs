namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a category is updated
/// </summary>
public class CategoryUpdatedEvent : DomainEvent
{
    public long CategoryId { get; }
    public string OldName { get; }
    public string NewName { get; }

    public CategoryUpdatedEvent(long categoryId, string oldName, string newName)
    {
        CategoryId = categoryId;
        OldName = oldName;
        NewName = newName;
    }
}