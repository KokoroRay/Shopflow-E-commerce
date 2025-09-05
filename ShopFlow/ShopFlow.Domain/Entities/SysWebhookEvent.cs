using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class SysWebhookEvent
{
    public long Id { get; set; }

    public string Topic { get; set; } = null!;

    public string PayloadJson { get; set; } = null!;

    public int Attempts { get; set; }

    public byte Status { get; set; }

    public DateTime? LastAttemptAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? VendorId { get; set; }

    public long? IntegrationId { get; set; }

    public long? OrderId { get; set; }

    public long? CartId { get; set; }

    public long? ProductId { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual SysIntegration? Integration { get; set; }

    public virtual OrdOrder? Order { get; set; }

    public virtual CatProduct? Product { get; set; }

    public virtual MktVendor? Vendor { get; set; }
}
