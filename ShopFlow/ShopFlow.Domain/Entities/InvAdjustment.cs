using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class InvAdjustment
{
    public long Id { get; set; }

    public long WarehouseId { get; set; }

    public long VendorId { get; set; }

    public string? Reason { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CoreUser? CreatedByNavigation { get; set; }

    public virtual ICollection<InvAdjustmentLine> InvAdjustmentLines { get; set; } = new List<InvAdjustmentLine>();

    public virtual MktVendor Vendor { get; set; } = null!;

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
