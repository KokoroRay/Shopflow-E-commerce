using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CatSkuMedium
{
    public long Id { get; set; }

    public long SkuId { get; set; }

    public string Url { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public int Sort { get; set; }

    public virtual CatSku Sku { get; set; } = null!;
}
