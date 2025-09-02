using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cat_category_i18n
{
    public long category_id { get; set; }

    public string lang { get; set; } = null!;

    public string name { get; set; } = null!;

    public string slug { get; set; } = null!;

    public string? meta_title { get; set; }

    public string? meta_desc { get; set; }

    public virtual cat_category category { get; set; } = null!;
}
