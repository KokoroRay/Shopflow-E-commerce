using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ship_rate
{
    public long id { get; set; }

    public long zone_id { get; set; }

    public int weight_from_g { get; set; }

    public int weight_to_g { get; set; }

    public decimal base_fee { get; set; }

    public decimal step_fee_per_500g { get; set; }

    public decimal cod_surcharge { get; set; }

    public decimal fuel_surcharge_pct { get; set; }

    public DateTime effective_from { get; set; }

    public DateTime? effective_to { get; set; }

    public virtual ship_zone zone { get; set; } = null!;
}
