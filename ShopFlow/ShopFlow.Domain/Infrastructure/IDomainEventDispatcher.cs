using ShopFlow.Domain.DomainEvents;

namespace ShopFlow.Domain.Infrastructure;

/// <summary>
/// Abstraction for publishing domain events without external dependencies
/// </summary>
public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task DispatchAll(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
