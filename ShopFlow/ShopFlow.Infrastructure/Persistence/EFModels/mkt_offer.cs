using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class mkt_offer
{
    public long id { get; set; }

    public long vendor_id { get; set; }

    public long sku_id { get; set; }

    public decimal? price_gross_vnd { get; set; }

    public decimal? price_gross_usd { get; set; }

    public byte currency_policy { get; set; }

    public byte status { get; set; }

    public decimal? min_qty { get; set; }

    public decimal? max_qty { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<cart_item> cart_items { get; set; } = new List<cart_item>();

    public virtual ICollection<ord_order_item> ord_order_items { get; set; } = new List<ord_order_item>();

    public virtual cat_sku sku { get; set; } = null!;

    public virtual mkt_vendor vendor { get; set; } = null!;
}
