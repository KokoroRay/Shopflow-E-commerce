using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class role_warehouse_staff
{
    public long warehouse_id { get; set; }

    public long user_id { get; set; }

    public int role_id { get; set; }

    public string? shift { get; set; }

    public DateTime created_at { get; set; }

    public virtual core_user_role core_user_role { get; set; } = null!;

    public virtual core_user user { get; set; } = null!;

    public virtual inv_warehouse warehouse { get; set; } = null!;
}
