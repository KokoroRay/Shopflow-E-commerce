using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class OrdShipment
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long WarehouseId { get; set; }

    public string? ShipperCode { get; set; }

    public string? TrackingNo { get; set; }

    public decimal Cost { get; set; }

    public byte Status { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public virtual OrdOrder Order { get; set; } = null!;

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
