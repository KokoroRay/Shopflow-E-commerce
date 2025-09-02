using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ship_zone
{
    public long id { get; set; }

    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public string country { get; set; } = null!;

    public string? region_pattern { get; set; }

    public string? district_pattern { get; set; }

    public virtual ICollection<ship_rate> ship_rates { get; set; } = new List<ship_rate>();
}
