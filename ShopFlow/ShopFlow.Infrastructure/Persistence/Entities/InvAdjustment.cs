using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("inv_adjustment")]
public partial class InvAdjustment
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("warehouse_id")]
    public long WarehouseId { get; set; }

    [Column("vendor_id")]
    public long VendorId { get; set; }

    [Column("reason")]
    [StringLength(255)]
    public string? Reason { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("InvAdjustments")]
    public virtual CoreUser? CreatedByNavigation { get; set; }

    [InverseProperty("Adjustment")]
    public virtual ICollection<InvAdjustmentLine> InvAdjustmentLines { get; set; } = new List<InvAdjustmentLine>();

    [ForeignKey("VendorId")]
    [InverseProperty("InvAdjustments")]
    public virtual MktVendor Vendor { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    [InverseProperty("InvAdjustments")]
    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
