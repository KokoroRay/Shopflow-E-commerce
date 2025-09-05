using ShopFlow.Domain.DomainEvents;

namespace ShopFlow.Domain.Entities;

public abstract class BaseEntity
{
    private readonly List<DomainEvent> _domainEvents = new();
    
    public long Id { get; set; }
    
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}   