namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a category is deleted
/// </summary>
public class CategoryDeletedEvent : DomainEvent
{
    public long CategoryId { get; }
    public string CategoryName { get; }

    public CategoryDeletedEvent(long categoryId, string categoryName)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
    }
}