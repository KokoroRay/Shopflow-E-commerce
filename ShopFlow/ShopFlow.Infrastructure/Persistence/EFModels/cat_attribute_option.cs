using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cat_attribute_option
{
    public long id { get; set; }

    public long attribute_id { get; set; }

    public string code { get; set; } = null!;

    public virtual cat_attribute attribute { get; set; } = null!;

    public virtual ICollection<cat_sku_option_value> cat_sku_option_values { get; set; } = new List<cat_sku_option_value>();
}
