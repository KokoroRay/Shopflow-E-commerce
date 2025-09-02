namespace ShopFlow.Domain.Entities;

public class cat_sku : BaseEntity
{
    public long? product_id { get; set; }
    public string? sku_code { get; set; }
    public bool is_active { get; set; } = true;

    public cat_product? product { get; set; }

    // Collections referenced elsewhere (keep empty for now)
    public ICollection<inv_reservation> inv_reservations { get; set; } = new List<inv_reservation>();
}   