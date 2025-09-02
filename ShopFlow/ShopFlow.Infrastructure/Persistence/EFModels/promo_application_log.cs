using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class promo_application_log
{
    public long id { get; set; }

    public long? order_id { get; set; }

    public long? cart_id { get; set; }

    public long? coupon_id { get; set; }

    public long? campaign_id { get; set; }

    public long? rule_id { get; set; }

    public decimal amount { get; set; }

    public string? snapshot_json { get; set; }

    public DateTime applied_at { get; set; }

    public virtual cart? cart { get; set; }

    public virtual ord_order? order { get; set; }

    public virtual promo_rule? rule { get; set; }
}
