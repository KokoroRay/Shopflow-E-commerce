namespace ShopFlow.Domain.Entities;
public class cart_applied_promo : BaseEntity
{
    public long? cart_id { get; set; }
    public cart? cart { get; set; }
    public long? rule_id { get; set; }
    public promo_rule? rule { get; set; }
}