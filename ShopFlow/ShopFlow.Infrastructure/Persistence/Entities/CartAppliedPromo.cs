using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cart_applied_promo")]
[Index("CampaignId", Name = "IX_cart_applied_campaign")]
[Index("CouponId", Name = "IX_cart_applied_coupon")]
[Index("RuleId", Name = "IX_cart_applied_rule")]
public partial class CartAppliedPromo
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("cart_id")]
    public long CartId { get; set; }

    [Column("coupon_id")]
    public long? CouponId { get; set; }

    [Column("campaign_id")]
    public long? CampaignId { get; set; }

    [Column("rule_id")]
    public long? RuleId { get; set; }

    [Column("discount_amount", TypeName = "decimal(19, 4)")]
    public decimal DiscountAmount { get; set; }

    [Column("breakdown_json")]
    public string? BreakdownJson { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("CartAppliedPromos")]
    public virtual Cart Cart { get; set; } = null!;

    [ForeignKey("RuleId")]
    [InverseProperty("CartAppliedPromos")]
    public virtual PromoRule? Rule { get; set; }
}
