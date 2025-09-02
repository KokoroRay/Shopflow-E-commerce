using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cart_applied_promo
{
    public long id { get; set; }

    public long cart_id { get; set; }

    public long? coupon_id { get; set; }

    public long? campaign_id { get; set; }

    public long? rule_id { get; set; }

    public decimal discount_amount { get; set; }

    public string? breakdown_json { get; set; }

    public virtual cart cart { get; set; } = null!;

    public virtual promo_rule? rule { get; set; }
}
