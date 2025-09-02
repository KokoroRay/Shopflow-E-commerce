using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cat_attribute_option_i18n
{
    public long option_id { get; set; }

    public string lang { get; set; } = null!;

    public string name { get; set; } = null!;

    public virtual cat_attribute_option option { get; set; } = null!;
}
