using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("promo_campaign")]
[Index("PeriodStart", "PeriodEnd", "Status", Name = "IX_promo_campaign_period")]
public partial class PromoCampaign
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("period_start")]
    [Precision(0)]
    public DateTime PeriodStart { get; set; }

    [Column("period_end")]
    [Precision(0)]
    public DateTime PeriodEnd { get; set; }

    [Column("priority")]
    public int Priority { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("scope_json")]
    public string? ScopeJson { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Campaign")]
    public virtual ICollection<PromoCoupon> PromoCoupons { get; set; } = new List<PromoCoupon>();

    [InverseProperty("Campaign")]
    public virtual ICollection<PromoRule> PromoRules { get; set; } = new List<PromoRule>();
}
