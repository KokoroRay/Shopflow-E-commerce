using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class TaxRate
{
    public int Id { get; set; }

    public string Country { get; set; } = null!;

    public string? Region { get; set; }

    public decimal RatePercent { get; set; }

    public virtual ICollection<TaxRateRule> TaxRateRules { get; set; } = new List<TaxRateRule>();
}
