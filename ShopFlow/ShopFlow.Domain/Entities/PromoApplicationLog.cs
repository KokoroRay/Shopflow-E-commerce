using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class PromoApplicationLog
{
    public long Id { get; set; }

    public long? OrderId { get; set; }

    public long? CartId { get; set; }

    public long? CouponId { get; set; }

    public long? CampaignId { get; set; }

    public long? RuleId { get; set; }

    public decimal Amount { get; set; }

    public string? SnapshotJson { get; set; }

    public DateTime AppliedAt { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual OrdOrder? Order { get; set; }

    public virtual PromoRule? Rule { get; set; }
}
