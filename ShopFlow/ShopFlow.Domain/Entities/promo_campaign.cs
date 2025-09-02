namespace ShopFlow.Domain.Entities;
public class promo_campaign : BaseEntity
{
    public string? name { get; set; }
    public ICollection<promo_rule> promo_rules { get; set; } = new List<promo_rule>();
}