using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class core_address
{
    public long id { get; set; }

    public long user_id { get; set; }

    public byte addr_type { get; set; }

    public string full_name { get; set; } = null!;

    public string phone { get; set; } = null!;

    public string line1 { get; set; } = null!;

    public string? line2 { get; set; }

    public string? ward { get; set; }

    public string? district { get; set; }

    public string? province { get; set; }

    public string country { get; set; } = null!;

    public string? postal_code { get; set; }

    public bool is_default { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual core_user user { get; set; } = null!;
}
