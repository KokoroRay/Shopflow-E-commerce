using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class tax_rate
{
    public int id { get; set; }

    public string country { get; set; } = null!;

    public string? region { get; set; }

    public decimal rate_percent { get; set; }

    public virtual ICollection<tax_rate_rule> tax_rate_rules { get; set; } = new List<tax_rate_rule>();
}
