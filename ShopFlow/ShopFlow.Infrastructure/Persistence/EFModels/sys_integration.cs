using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class sys_integration
{
    public long id { get; set; }

    public long? vendor_id { get; set; }

    public string type { get; set; } = null!;

    public string? config_json { get; set; }

    public byte status { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<sys_staging> sys_stagings { get; set; } = new List<sys_staging>();

    public virtual ICollection<sys_webhook_event> sys_webhook_events { get; set; } = new List<sys_webhook_event>();

    public virtual mkt_vendor? vendor { get; set; }
}
