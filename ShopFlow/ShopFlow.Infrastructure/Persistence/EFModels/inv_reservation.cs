using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class inv_reservation
{
    public long id { get; set; }

    public long warehouse_id { get; set; }

    public long vendor_id { get; set; }

    public long sku_id { get; set; }

    public long? cart_id { get; set; }

    public long? order_id { get; set; }

    public decimal qty { get; set; }

    public byte status { get; set; }

    public DateTime? expires_at { get; set; }

    public DateTime created_at { get; set; }

    public virtual cart? cart { get; set; }

    public virtual ord_order? order { get; set; }

    public virtual cat_sku sku { get; set; } = null!;

    public virtual mkt_vendor vendor { get; set; } = null!;

    public virtual inv_warehouse warehouse { get; set; } = null!;
}
