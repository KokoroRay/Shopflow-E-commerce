namespace ShopFlow.Domain.Entities;
public class promo_application_log : BaseEntity
{
    public long? cart_id { get; set; }
    public cart? cart { get; set; }
    public long? order_id { get; set; }
    public ord_order? order { get; set; }
    public long? rule_id { get; set; }
    public promo_rule? rule { get; set; }
}