using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ShipRate
{
    public long Id { get; set; }

    public long ZoneId { get; set; }

    public int WeightFromG { get; set; }

    public int WeightToG { get; set; }

    public decimal BaseFee { get; set; }

    public decimal StepFeePer500g { get; set; }

    public decimal CodSurcharge { get; set; }

    public decimal FuelSurchargePct { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public virtual ShipZone Zone { get; set; } = null!;
}
