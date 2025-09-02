namespace ShopFlow.Domain.Entities;

public class cat_product : BaseEntity
{
    public byte product_type { get; set; }       // ??i string? -> byte
    public byte status { get; set; }
    public int? return_days { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }

    // minimal navigations referenced by DbContext
    public ICollection<cat_sku> cat_skus { get; set; } = new List<cat_sku>();
    // keep without explicit ce_review type to avoid dependency; DbContext uses WithMany()
    public ICollection<sys_webhook_event> sys_webhook_events { get; set; } = new List<sys_webhook_event>();
}