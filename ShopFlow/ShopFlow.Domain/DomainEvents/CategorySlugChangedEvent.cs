namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a category slug is changed
/// </summary>
public class CategorySlugChangedEvent : DomainEvent
{
    public long CategoryId { get; }
    public string OldSlug { get; }
    public string NewSlug { get; }

    public CategorySlugChangedEvent(long categoryId, string oldSlug, string newSlug)
    {
        CategoryId = categoryId;
        OldSlug = oldSlug;
        NewSlug = newSlug;
    }
}