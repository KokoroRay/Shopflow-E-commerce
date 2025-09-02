using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class core_permission
{
    public int id { get; set; }

    public string code { get; set; } = null!;

    public string? description { get; set; }

    public virtual ICollection<core_role> roles { get; set; } = new List<core_role>();
}
