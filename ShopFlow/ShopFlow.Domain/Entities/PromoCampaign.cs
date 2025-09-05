using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class PromoCampaign
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    public int Priority { get; set; }

    public byte Status { get; set; }

    public string? ScopeJson { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<PromoCoupon> PromoCoupons { get; set; } = new List<PromoCoupon>();

    public virtual ICollection<PromoRule> PromoRules { get; set; } = new List<PromoRule>();
}
