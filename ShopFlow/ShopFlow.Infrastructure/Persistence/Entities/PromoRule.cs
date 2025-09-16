using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("promo_rule")]
[Index("CampaignId", Name = "IX_promo_rule_campaign")]
public partial class PromoRule
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("campaign_id")]
    public long CampaignId { get; set; }

    [Column("condition_json")]
    public string ConditionJson { get; set; } = null!;

    [Column("action_json")]
    public string ActionJson { get; set; } = null!;

    [Column("stop_further_rules")]
    public bool StopFurtherRules { get; set; }

    [Column("sort")]
    public int Sort { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [ForeignKey("CampaignId")]
    [InverseProperty("PromoRules")]
    public virtual PromoCampaign Campaign { get; set; } = null!;

    [InverseProperty("Rule")]
    public virtual ICollection<CartAppliedPromo> CartAppliedPromos { get; set; } = new List<CartAppliedPromo>();

    [InverseProperty("Rule")]
    public virtual ICollection<PromoApplicationLog> PromoApplicationLogs { get; set; } = new List<PromoApplicationLog>();
}
