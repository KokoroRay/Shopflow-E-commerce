using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CartAppliedPromo
{
    public long Id { get; set; }

    public long CartId { get; set; }

    public long? CouponId { get; set; }

    public long? CampaignId { get; set; }

    public long? RuleId { get; set; }

    public decimal DiscountAmount { get; set; }

    public string? BreakdownJson { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual PromoRule? Rule { get; set; }
}
