namespace ShopFlow.Domain.DomainEvents;

public class UserCreatedEvent : DomainEvent
{
    public long UserId { get; }
    public string Email { get; }
    
    public UserCreatedEvent(long userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}
