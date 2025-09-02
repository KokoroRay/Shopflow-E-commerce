using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class sys_staging
{
    public long id { get; set; }

    public long integration_id { get; set; }

    public string entity { get; set; } = null!;

    public byte direction { get; set; }

    public string payload_json { get; set; } = null!;

    public byte status { get; set; }

    public string? error_msg { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? processed_at { get; set; }

    public virtual sys_integration integration { get; set; } = null!;
}
