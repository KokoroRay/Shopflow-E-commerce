using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class PromoRule
{
    public long Id { get; set; }

    public long CampaignId { get; set; }

    public string ConditionJson { get; set; } = null!;

    public string ActionJson { get; set; } = null!;

    public bool StopFurtherRules { get; set; }

    public int Sort { get; set; }

    public byte Status { get; set; }

    public virtual PromoCampaign Campaign { get; set; } = null!;

    public virtual ICollection<CartAppliedPromo> CartAppliedPromos { get; set; } = new List<CartAppliedPromo>();

    public virtual ICollection<PromoApplicationLog> PromoApplicationLogs { get; set; } = new List<PromoApplicationLog>();
}
