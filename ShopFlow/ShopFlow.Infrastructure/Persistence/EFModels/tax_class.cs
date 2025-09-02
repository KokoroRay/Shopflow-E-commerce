using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class tax_class
{
    public int id { get; set; }

    public string code { get; set; } = null!;

    public virtual ICollection<tax_rate_rule> tax_rate_rules { get; set; } = new List<tax_rate_rule>();

    public virtual ICollection<cat_sku> skus { get; set; } = new List<cat_sku>();
}
