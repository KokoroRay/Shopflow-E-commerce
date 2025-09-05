using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class InvAdjustmentLine
{
    public long Id { get; set; }

    public long AdjustmentId { get; set; }

    public long SkuId { get; set; }

    public decimal DeltaQty { get; set; }

    public virtual InvAdjustment Adjustment { get; set; } = null!;

    public virtual CatSku Sku { get; set; } = null!;
}
