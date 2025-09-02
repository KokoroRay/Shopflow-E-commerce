using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class inv_adjustment
{
    public long id { get; set; }

    public long warehouse_id { get; set; }

    public long vendor_id { get; set; }

    public string? reason { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    public virtual core_user? created_byNavigation { get; set; }

    public virtual ICollection<inv_adjustment_line> inv_adjustment_lines { get; set; } = new List<inv_adjustment_line>();

    public virtual mkt_vendor vendor { get; set; } = null!;

    public virtual inv_warehouse warehouse { get; set; } = null!;
}
