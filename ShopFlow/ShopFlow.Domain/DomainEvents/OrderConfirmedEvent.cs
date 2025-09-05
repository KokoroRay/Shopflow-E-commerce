namespace ShopFlow.Domain.DomainEvents;

public class OrderConfirmedEvent : DomainEvent
{
    public long OrderId { get; }
    public long CustomerId { get; }
    public decimal TotalAmount { get; }
    
    public OrderConfirmedEvent(long orderId, long customerId, decimal totalAmount)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }
}
