using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class InvStock
{
    public long Id { get; set; }

    public long WarehouseId { get; set; }

    public long VendorId { get; set; }

    public long SkuId { get; set; }

    public decimal OnHand { get; set; }

    public decimal Reserved { get; set; }

    public decimal SafetyStock { get; set; }

    public DateTime UpdatedAt { get; set; }

    public byte[] RowVer { get; set; } = null!;

    public virtual CatSku Sku { get; set; } = null!;

    public virtual MktVendor Vendor { get; set; } = null!;

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
