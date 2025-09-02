using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class tax_rate_rule
{
    public int id { get; set; }

    public int tax_class_id { get; set; }

    public int tax_rate_id { get; set; }

    public DateTime effective_from { get; set; }

    public DateTime? effective_to { get; set; }

    public virtual tax_class tax_class { get; set; } = null!;

    public virtual tax_rate tax_rate { get; set; } = null!;
}
