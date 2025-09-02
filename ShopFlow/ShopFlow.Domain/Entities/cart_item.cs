namespace ShopFlow.Domain.Entities;
public class cart_item : BaseEntity
{
    public long? cart_id { get; set; }
    public cart? cart { get; set; }
}