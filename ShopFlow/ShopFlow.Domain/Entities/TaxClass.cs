using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class TaxClass
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public virtual ICollection<TaxRateRule> TaxRateRules { get; set; } = new List<TaxRateRule>();

    public virtual ICollection<CatSku> Skus { get; set; } = new List<CatSku>();
}
