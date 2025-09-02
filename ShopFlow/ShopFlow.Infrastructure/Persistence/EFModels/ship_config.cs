using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ship_config
{
    public long id { get; set; }

    public long warehouse_id { get; set; }

    public int lead_time_days { get; set; }

    public bool enabled { get; set; }

    public virtual inv_warehouse warehouse { get; set; } = null!;
}
