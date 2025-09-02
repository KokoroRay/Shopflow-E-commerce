using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cat_sku_medium
{
    public long id { get; set; }

    public long sku_id { get; set; }

    public string url { get; set; } = null!;

    public bool is_primary { get; set; }

    public int sort { get; set; }

    public virtual cat_sku sku { get; set; } = null!;
}
