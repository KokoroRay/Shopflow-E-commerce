using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("inv_stock")]
[Index("SkuId", "WarehouseId", Name = "IX_stock_sku_wh")]
[Index("WarehouseId", "VendorId", "SkuId", Name = "UQ_stock_wh_vendor_sku", IsUnique = true)]
public partial class InvStock
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("warehouse_id")]
    public long WarehouseId { get; set; }

    [Column("vendor_id")]
    public long VendorId { get; set; }

    [Column("sku_id")]
    public long SkuId { get; set; }

    [Column("on_hand", TypeName = "decimal(18, 3)")]
    public decimal OnHand { get; set; }

    [Column("reserved", TypeName = "decimal(18, 3)")]
    public decimal Reserved { get; set; }

    [Column("safety_stock", TypeName = "decimal(18, 3)")]
    public decimal SafetyStock { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [Column("row_ver")]
    public byte[] RowVer { get; set; } = null!;

    [ForeignKey("SkuId")]
    [InverseProperty("InvStocks")]
    public virtual CatSku Sku { get; set; } = null!;

    [ForeignKey("VendorId")]
    [InverseProperty("InvStocks")]
    public virtual MktVendor Vendor { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    [InverseProperty("InvStocks")]
    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
