using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("inv_adjustment_line")]
public partial class InvAdjustmentLine
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("adjustment_id")]
    public long AdjustmentId { get; set; }

    [Column("sku_id")]
    public long SkuId { get; set; }

    [Column("delta_qty", TypeName = "decimal(18, 3)")]
    public decimal DeltaQty { get; set; }

    [ForeignKey("AdjustmentId")]
    [InverseProperty("InvAdjustmentLines")]
    public virtual InvAdjustment Adjustment { get; set; } = null!;

    [ForeignKey("SkuId")]
    [InverseProperty("InvAdjustmentLines")]
    public virtual CatSku Sku { get; set; } = null!;
}
