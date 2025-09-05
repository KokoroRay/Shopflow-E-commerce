using MediatR;
using ShopFlow.Application.Abstractions.Messaging;
using ShopFlow.Domain.DomainEvents;

namespace ShopFlow.Application.Handlers.DomainEvents;

public class UserCreatedEventHandler : IDomainEventHandler<UserCreatedEvent>, INotificationHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User created: {UserId} with email {Email}", 
            domainEvent.UserId, domainEvent.Email);

        // Additional side effects:
        // - Send welcome email
        // - Create customer profile
        // - Initialize shopping cart
        // - Send notification to admin
        
        await Task.CompletedTask;
    }
}
