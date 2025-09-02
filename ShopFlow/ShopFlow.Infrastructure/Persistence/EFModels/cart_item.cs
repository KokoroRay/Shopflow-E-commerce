using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cart_item
{
    public long id { get; set; }

    public long cart_id { get; set; }

    public long offer_id { get; set; }

    public long vendor_id { get; set; }

    public long sku_id { get; set; }

    public decimal qty { get; set; }

    public decimal price_gross_snapshot { get; set; }

    public decimal tax_rate_snapshot { get; set; }

    public string? attributes_snapshot { get; set; }

    public virtual cart cart { get; set; } = null!;

    public virtual mkt_offer offer { get; set; } = null!;

    public virtual cat_sku sku { get; set; } = null!;

    public virtual mkt_vendor vendor { get; set; } = null!;
}
