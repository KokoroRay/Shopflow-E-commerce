using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ord_shipment
{
    public long id { get; set; }

    public long order_id { get; set; }

    public long warehouse_id { get; set; }

    public string? shipper_code { get; set; }

    public string? tracking_no { get; set; }

    public decimal cost { get; set; }

    public byte status { get; set; }

    public DateTime? shipped_at { get; set; }

    public DateTime? delivered_at { get; set; }

    public virtual ord_order order { get; set; } = null!;

    public virtual inv_warehouse warehouse { get; set; } = null!;
}
