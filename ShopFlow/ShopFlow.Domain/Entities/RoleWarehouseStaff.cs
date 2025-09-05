using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class RoleWarehouseStaff
{
    public long WarehouseId { get; set; }

    public long UserId { get; set; }

    public int RoleId { get; set; }

    public string? Shift { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CoreUserRole CoreUserRole { get; set; } = null!;

    public virtual CoreUser User { get; set; } = null!;

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
