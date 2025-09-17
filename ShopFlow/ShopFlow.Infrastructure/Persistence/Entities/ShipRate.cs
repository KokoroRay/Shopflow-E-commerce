using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("ship_rate")]
[Index("ZoneId", "WeightFromG", "WeightToG", "EffectiveFrom", Name = "IX_ship_rate_zone", IsDescending = new[] { false, false, false, true })]
public partial class ShipRate
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("zone_id")]
    public long ZoneId { get; set; }

    [Column("weight_from_g")]
    public int WeightFromG { get; set; }

    [Column("weight_to_g")]
    public int WeightToG { get; set; }

    [Column("base_fee", TypeName = "decimal(19, 4)")]
    public decimal BaseFee { get; set; }

    [Column("step_fee_per_500g", TypeName = "decimal(19, 4)")]
    public decimal StepFeePer500g { get; set; }

    [Column("cod_surcharge", TypeName = "decimal(19, 4)")]
    public decimal CodSurcharge { get; set; }

    [Column("fuel_surcharge_pct", TypeName = "decimal(5, 2)")]
    public decimal FuelSurchargePct { get; set; }

    [Column("effective_from")]
    [Precision(0)]
    public DateTime EffectiveFrom { get; set; }

    [Column("effective_to")]
    [Precision(0)]
    public DateTime? EffectiveTo { get; set; }

    [ForeignKey("ZoneId")]
    [InverseProperty("ShipRates")]
    public virtual ShipZone Zone { get; set; } = null!;
}
