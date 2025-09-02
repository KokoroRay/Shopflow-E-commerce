namespace ShopFlow.Domain.Entities;
public class inv_reservation : BaseEntity
{
    public long? cart_id { get; set; }
    public cart? cart { get; set; }
    public long? order_id { get; set; }
    public ord_order? order { get; set; }
    public long? sku_id { get; set; }
    public cat_sku? sku { get; set; }
}