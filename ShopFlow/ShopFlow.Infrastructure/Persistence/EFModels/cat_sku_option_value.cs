using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cat_sku_option_value
{
    public long sku_id { get; set; }

    public long attribute_id { get; set; }

    public long option_id { get; set; }

    public DateTime created_at { get; set; }

    public virtual cat_attribute attribute { get; set; } = null!;

    public virtual cat_attribute_option option { get; set; } = null!;

    public virtual cat_sku sku { get; set; } = null!;
}
