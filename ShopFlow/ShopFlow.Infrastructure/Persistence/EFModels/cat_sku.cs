using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cat_sku
{
    public long id { get; set; }

    public long product_id { get; set; }

    public string sku_code { get; set; } = null!;

    public string? barcode { get; set; }

    public string? options_json { get; set; }

    public int? weight_g { get; set; }

    public int? length_mm { get; set; }

    public int? width_mm { get; set; }

    public int? height_mm { get; set; }

    public bool is_active { get; set; }

    public virtual ICollection<cart_item> cart_items { get; set; } = new List<cart_item>();

    public virtual ICollection<cat_sku_medium> cat_sku_media { get; set; } = new List<cat_sku_medium>();

    public virtual ICollection<cat_sku_option_value> cat_sku_option_values { get; set; } = new List<cat_sku_option_value>();

    public virtual ICollection<ce_review> ce_reviews { get; set; } = new List<ce_review>();

    public virtual ICollection<inv_adjustment_line> inv_adjustment_lines { get; set; } = new List<inv_adjustment_line>();

    public virtual ICollection<inv_reservation> inv_reservations { get; set; } = new List<inv_reservation>();

    public virtual ICollection<inv_stock> inv_stocks { get; set; } = new List<inv_stock>();

    public virtual ICollection<mkt_offer> mkt_offers { get; set; } = new List<mkt_offer>();

    public virtual ICollection<ord_order_item> ord_order_items { get; set; } = new List<ord_order_item>();

    public virtual cat_product product { get; set; } = null!;

    public virtual ICollection<tax_class> tax_classes { get; set; } = new List<tax_class>();
}
