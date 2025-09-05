using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class SysExtMapping
{
    public long Id { get; set; }

    public long? VendorId { get; set; }

    public string Entity { get; set; } = null!;

    public long LocalId { get; set; }

    public string ExternalId { get; set; } = null!;

    public string SystemId { get; set; } = null!;

    public virtual MktVendor? Vendor { get; set; }
}
