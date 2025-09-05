using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class InvReservation
{
    public long Id { get; set; }

    public long WarehouseId { get; set; }

    public long VendorId { get; set; }

    public long SkuId { get; set; }

    public long? CartId { get; set; }

    public long? OrderId { get; set; }

    public decimal Qty { get; set; }

    public byte Status { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual OrdOrder? Order { get; set; }

    public virtual CatSku Sku { get; set; } = null!;

    public virtual MktVendor Vendor { get; set; } = null!;

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
