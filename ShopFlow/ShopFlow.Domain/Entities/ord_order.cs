namespace ShopFlow.Domain.Entities;

public class ord_order : BaseEntity
{
    public long? user_id { get; set; }

    // Basic scalar snapshots (add more later if needed)
    public string? order_code { get; set; }
    public decimal? grand_total { get; set; }
    public DateTime? placed_at { get; set; }

    // Navigations (minimal sets to satisfy other relationships)
    public core_user? user { get; set; }

    public ICollection<promo_application_log> promo_application_logs { get; set; } = new List<promo_application_log>();
    public ICollection<inv_reservation> inv_reservations { get; set; } = new List<inv_reservation>();
    public ICollection<sys_webhook_event> sys_webhook_events { get; set; } = new List<sys_webhook_event>();
}