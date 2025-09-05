using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class TaxRateRule
{
    public int Id { get; set; }

    public int TaxClassId { get; set; }

    public int TaxRateId { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public virtual TaxClass TaxClass { get; set; } = null!;

    public virtual TaxRate TaxRate { get; set; } = null!;
}
