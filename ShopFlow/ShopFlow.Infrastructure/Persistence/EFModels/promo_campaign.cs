using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class promo_campaign
{
    public long id { get; set; }

    public string name { get; set; } = null!;

    public DateTime period_start { get; set; }

    public DateTime period_end { get; set; }

    public int priority { get; set; }

    public byte status { get; set; }

    public string? scope_json { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<promo_coupon> promo_coupons { get; set; } = new List<promo_coupon>();

    public virtual ICollection<promo_rule> promo_rules { get; set; } = new List<promo_rule>();
}
