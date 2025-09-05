using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class SysIntegration
{
    public long Id { get; set; }

    public long? VendorId { get; set; }

    public string Type { get; set; } = null!;

    public string? ConfigJson { get; set; }

    public byte Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<SysStaging> SysStagings { get; set; } = new List<SysStaging>();

    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; set; } = new List<SysWebhookEvent>();

    public virtual MktVendor? Vendor { get; set; }
}
