using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class sys_ext_mapping
{
    public long id { get; set; }

    public long? vendor_id { get; set; }

    public string entity { get; set; } = null!;

    public long local_id { get; set; }

    public string external_id { get; set; } = null!;

    public string system_id { get; set; } = null!;

    public virtual mkt_vendor? vendor { get; set; }
}
