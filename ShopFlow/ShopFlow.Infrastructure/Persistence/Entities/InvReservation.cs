using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("inv_reservation")]
[Index("WarehouseId", "VendorId", "SkuId", "Status", Name = "IX_reservation_lookup")]
public partial class InvReservation
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

    [Column("cart_id")]
    public long? CartId { get; set; }

    [Column("order_id")]
    public long? OrderId { get; set; }

    [Column("qty", TypeName = "decimal(18, 3)")]
    public decimal Qty { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("expires_at")]
    [Precision(0)]
    public DateTime? ExpiresAt { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("InvReservations")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("InvReservations")]
    public virtual OrdOrder? Order { get; set; }

    [ForeignKey("SkuId")]
    [InverseProperty("InvReservations")]
    public virtual CatSku Sku { get; set; } = null!;

    [ForeignKey("VendorId")]
    [InverseProperty("InvReservations")]
    public virtual MktVendor Vendor { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    [InverseProperty("InvReservations")]
    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
