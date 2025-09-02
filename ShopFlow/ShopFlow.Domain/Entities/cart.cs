namespace ShopFlow.Domain.Entities;

public class cart : BaseEntity
{
    public long? user_id { get; set; }
    public long? warehouse_id { get; set; }
    public string? guest_token { get; set; }
    public string? currency { get; set; }
    public DateTime last_activity_at { get; set; }

    // Navigations (t?i thi?u ?? DbContext fluent mapping kh�ng l?i)
    public core_user? user { get; set; }

    // Collections tham chi?u trong DbContext / EFModels
    public ICollection<cart_item> cart_items { get; set; } = new List<cart_item>();
    public ICollection<cart_applied_promo> cart_applied_promos { get; set; } = new List<cart_applied_promo>();
    public ICollection<promo_application_log> promo_application_logs { get; set; } = new List<promo_application_log>();
    public ICollection<inv_reservation> inv_reservations { get; set; } = new List<inv_reservation>();
    public ICollection<sys_webhook_event> sys_webhook_events { get; set; } = new List<sys_webhook_event>();
}