using ShopFlow.Domain.DomainEvents;

namespace ShopFlow.Application.Abstractions.Messaging;

public interface IDomainEventPublisher
{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : DomainEvent;
    Task PublishAllAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

public interface IDomainEventHandler<in T> where T : DomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken = default);
}
