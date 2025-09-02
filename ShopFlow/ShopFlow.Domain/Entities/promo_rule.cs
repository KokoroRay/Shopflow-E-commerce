namespace ShopFlow.Domain.Entities;
public class promo_rule : BaseEntity
{
    public long? campaign_id { get; set; }
    public promo_campaign? campaign { get; set; }
    public ICollection<cart_applied_promo> cart_applied_promos { get; set; } = new List<cart_applied_promo>();
    public ICollection<promo_application_log> promo_application_logs { get; set; } = new List<promo_application_log>();
}