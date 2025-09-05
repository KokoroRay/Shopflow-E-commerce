using MediatR;

namespace ShopFlow.Domain.DomainEvents;

public abstract class DomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid Id { get; } = Guid.NewGuid();
}
