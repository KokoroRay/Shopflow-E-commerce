using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ShipConfig
{
    public long Id { get; set; }

    public long WarehouseId { get; set; }

    public int LeadTimeDays { get; set; }

    public bool Enabled { get; set; }

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
