using MediatR;
using ShopFlow.Application.Abstractions.Messaging;
using ShopFlow.Domain.DomainEvents;

namespace ShopFlow.Infrastructure.Services;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IMediator _mediator;

    public DomainEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : DomainEvent
    {
        await _mediator.Publish(domainEvent, cancellationToken);
    }

    public async Task PublishAllAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
