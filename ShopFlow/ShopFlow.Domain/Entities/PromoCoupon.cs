using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class PromoCoupon
{
    public long Id { get; set; }

    public long CampaignId { get; set; }

    public string Code { get; set; } = null!;

    public int? MaxUses { get; set; }

    public int? MaxUsesPerUser { get; set; }

    public int Used { get; set; }

    public byte Status { get; set; }

    public virtual PromoCampaign Campaign { get; set; } = null!;
}
