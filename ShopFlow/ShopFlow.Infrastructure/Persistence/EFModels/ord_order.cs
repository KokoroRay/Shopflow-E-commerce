using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ord_order
{
    public long id { get; set; }

    public string order_code { get; set; } = null!;

    public long? user_id { get; set; }

    public long warehouse_id { get; set; }

    public string currency { get; set; } = null!;

    public decimal? fx_rate { get; set; }

    public decimal subtotal { get; set; }

    public decimal discount_total { get; set; }

    public decimal tax_total { get; set; }

    public decimal shipping_fee { get; set; }

    public decimal grand_total { get; set; }

    public byte status { get; set; }

    public DateTime placed_at { get; set; }

    public DateTime? completed_at { get; set; }

    public string? cancel_reason { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<ce_review> ce_reviews { get; set; } = new List<ce_review>();

    public virtual ICollection<inv_reservation> inv_reservations { get; set; } = new List<inv_reservation>();

    public virtual ICollection<ord_order_address> ord_order_addresses { get; set; } = new List<ord_order_address>();

    public virtual ICollection<ord_order_item> ord_order_items { get; set; } = new List<ord_order_item>();

    public virtual ord_shipment? ord_shipment { get; set; }

    public virtual ICollection<pay_transaction> pay_transactions { get; set; } = new List<pay_transaction>();

    public virtual ICollection<promo_application_log> promo_application_logs { get; set; } = new List<promo_application_log>();

    public virtual ICollection<sys_webhook_event> sys_webhook_events { get; set; } = new List<sys_webhook_event>();

    public virtual core_user? user { get; set; }

    public virtual inv_warehouse warehouse { get; set; } = null!;
}
