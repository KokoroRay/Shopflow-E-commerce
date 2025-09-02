using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ord_order_item
{
    public long id { get; set; }

    public long order_id { get; set; }

    public long vendor_id { get; set; }

    public long offer_id { get; set; }

    public long sku_id { get; set; }

    public string name_snapshot { get; set; } = null!;

    public string? attributes_snapshot { get; set; }

    public decimal qty { get; set; }

    public decimal unit_price_gross { get; set; }

    public decimal discount { get; set; }

    public decimal tax_amount { get; set; }

    public decimal? line_total { get; set; }

    public virtual mkt_offer offer { get; set; } = null!;

    public virtual ord_order order { get; set; } = null!;

    public virtual cat_sku sku { get; set; } = null!;

    public virtual mkt_vendor vendor { get; set; } = null!;
}
