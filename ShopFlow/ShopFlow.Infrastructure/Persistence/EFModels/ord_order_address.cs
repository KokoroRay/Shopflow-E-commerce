using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ord_order_address
{
    public long id { get; set; }

    public long order_id { get; set; }

    public byte addr_type { get; set; }

    public string full_name { get; set; } = null!;

    public string phone { get; set; } = null!;

    public string line1 { get; set; } = null!;

    public string? line2 { get; set; }

    public string? ward { get; set; }

    public string? district { get; set; }

    public string? province { get; set; }

    public string country { get; set; } = null!;

    public string? postal_code { get; set; }

    public virtual ord_order order { get; set; } = null!;
}
