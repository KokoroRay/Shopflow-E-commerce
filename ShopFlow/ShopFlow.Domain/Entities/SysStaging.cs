using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class SysStaging
{
    public long Id { get; set; }

    public long IntegrationId { get; set; }

    public string Entity { get; set; } = null!;

    public byte Direction { get; set; }

    public string PayloadJson { get; set; } = null!;

    public byte Status { get; set; }

    public string? ErrorMsg { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public virtual SysIntegration Integration { get; set; } = null!;
}
