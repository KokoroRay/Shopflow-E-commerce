using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("inv_stock_change_log")]
[Index("SkuId", "CreatedAt", Name = "IX_inv_stock_change_log_sku", IsDescending = new[] { false, true })]
public partial class InvStockChangeLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("sku_id")]
    public long SkuId { get; set; }

    [Column("warehouse_id")]
    public long WarehouseId { get; set; }

    [Column("vendor_id")]
    public long VendorId { get; set; }

    [Column("change_type")]
    [StringLength(20)]
    [Unicode(false)]
    public string ChangeType { get; set; } = null!;

    [Column("old_quantity", TypeName = "decimal(18, 3)")]
    public decimal OldQuantity { get; set; }

    [Column("new_quantity", TypeName = "decimal(18, 3)")]
    public decimal NewQuantity { get; set; }

    [Column("delta_quantity", TypeName = "decimal(18, 3)")]
    public decimal DeltaQuantity { get; set; }

    [Column("reference_id")]
    public long? ReferenceId { get; set; }

    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("InvStockChangeLogs")]
    public virtual CoreUser? CreatedByNavigation { get; set; }

    [ForeignKey("SkuId")]
    [InverseProperty("InvStockChangeLogs")]
    public virtual CatSku Sku { get; set; } = null!;

    [ForeignKey("VendorId")]
    [InverseProperty("InvStockChangeLogs")]
    public virtual MktVendor Vendor { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    [InverseProperty("InvStockChangeLogs")]
    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
