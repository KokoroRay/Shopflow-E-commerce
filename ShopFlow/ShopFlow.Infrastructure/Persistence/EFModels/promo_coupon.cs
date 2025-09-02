using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class promo_coupon
{
    public long id { get; set; }

    public long campaign_id { get; set; }

    public string code { get; set; } = null!;

    public int? max_uses { get; set; }

    public int? max_uses_per_user { get; set; }

    public int used { get; set; }

    public byte status { get; set; }

    public virtual promo_campaign campaign { get; set; } = null!;
}
