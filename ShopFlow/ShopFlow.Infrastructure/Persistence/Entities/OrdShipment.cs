using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("ord_shipment")]
[Index("OrderId", Name = "UQ_ship_order", IsUnique = true)]
public partial class OrdShipment
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("order_id")]
    public long OrderId { get; set; }

    [Column("warehouse_id")]
    public long WarehouseId { get; set; }

    [Column("shipper_code")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ShipperCode { get; set; }

    [Column("tracking_no")]
    [StringLength(100)]
    [Unicode(false)]
    public string? TrackingNo { get; set; }

    [Column("cost", TypeName = "decimal(19, 4)")]
    public decimal Cost { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("shipped_at")]
    [Precision(0)]
    public DateTime? ShippedAt { get; set; }

    [Column("delivered_at")]
    [Precision(0)]
    public DateTime? DeliveredAt { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrdShipment")]
    public virtual OrdOrder Order { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    [InverseProperty("OrdShipments")]
    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
