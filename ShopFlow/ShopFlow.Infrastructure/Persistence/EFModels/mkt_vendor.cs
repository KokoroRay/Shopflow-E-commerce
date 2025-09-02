using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class mkt_vendor
{
    public long id { get; set; }

    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public string? tax_code { get; set; }

    public string? invoice_profile_json { get; set; }

    public string? return_policy_json { get; set; }

    public byte status { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<cart_item> cart_items { get; set; } = new List<cart_item>();

    public virtual ICollection<ce_review> ce_reviews { get; set; } = new List<ce_review>();

    public virtual ICollection<inv_adjustment> inv_adjustments { get; set; } = new List<inv_adjustment>();

    public virtual ICollection<inv_reservation> inv_reservations { get; set; } = new List<inv_reservation>();

    public virtual ICollection<inv_stock> inv_stocks { get; set; } = new List<inv_stock>();

    public virtual ICollection<mkt_offer> mkt_offers { get; set; } = new List<mkt_offer>();

    public virtual ICollection<ord_order_item> ord_order_items { get; set; } = new List<ord_order_item>();

    public virtual ICollection<role_vendor_staff> role_vendor_staffs { get; set; } = new List<role_vendor_staff>();

    public virtual ICollection<sys_ext_mapping> sys_ext_mappings { get; set; } = new List<sys_ext_mapping>();

    public virtual ICollection<sys_integration> sys_integrations { get; set; } = new List<sys_integration>();

    public virtual ICollection<sys_webhook_event> sys_webhook_events { get; set; } = new List<sys_webhook_event>();
}
