using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cat_attribute
{
    public long id { get; set; }

    public string code { get; set; } = null!;

    public string data_type { get; set; } = null!;

    public virtual ICollection<cat_attribute_option> cat_attribute_options { get; set; } = new List<cat_attribute_option>();

    public virtual ICollection<cat_sku_option_value> cat_sku_option_values { get; set; } = new List<cat_sku_option_value>();
}
