namespace ShopFlow.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a new category is created
/// </summary>
public class CategoryCreatedEvent : DomainEvent
{
    public long CategoryId { get; }
    public string Name { get; }
    public string Slug { get; }
    public long? ParentId { get; }

    public CategoryCreatedEvent(long categoryId, string name, string slug, long? parentId)
    {
        CategoryId = categoryId;
        Name = name;
        Slug = slug;
        ParentId = parentId;
    }
}