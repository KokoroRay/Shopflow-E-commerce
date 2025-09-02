using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class inv_stock
{
    public long id { get; set; }

    public long warehouse_id { get; set; }

    public long vendor_id { get; set; }

    public long sku_id { get; set; }

    public decimal on_hand { get; set; }

    public decimal reserved { get; set; }

    public decimal safety_stock { get; set; }

    public DateTime updated_at { get; set; }

    public byte[] row_ver { get; set; } = null!;

    public virtual cat_sku sku { get; set; } = null!;

    public virtual mkt_vendor vendor { get; set; } = null!;

    public virtual inv_warehouse warehouse { get; set; } = null!;
}
