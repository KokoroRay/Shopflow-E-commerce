using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class role_vendor_staff
{
    public long vendor_id { get; set; }

    public long user_id { get; set; }

    public int role_id { get; set; }

    public string? title { get; set; }

    public string? permissions_json { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public virtual core_user_role core_user_role { get; set; } = null!;

    public virtual core_user user { get; set; } = null!;

    public virtual mkt_vendor vendor { get; set; } = null!;
}
