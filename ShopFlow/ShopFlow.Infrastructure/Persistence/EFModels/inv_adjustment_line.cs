using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class inv_adjustment_line
{
    public long id { get; set; }

    public long adjustment_id { get; set; }

    public long sku_id { get; set; }

    public decimal delta_qty { get; set; }

    public virtual inv_adjustment adjustment { get; set; } = null!;

    public virtual cat_sku sku { get; set; } = null!;
}
