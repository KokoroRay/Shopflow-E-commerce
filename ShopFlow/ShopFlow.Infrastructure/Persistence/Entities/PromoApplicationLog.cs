using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("promo_application_log")]
[Index("CampaignId", Name = "IX_promo_log_campaign")]
[Index("CartId", "AppliedAt", Name = "IX_promo_log_cart")]
[Index("CouponId", Name = "IX_promo_log_coupon")]
[Index("OrderId", "AppliedAt", Name = "IX_promo_log_order")]
public partial class PromoApplicationLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("order_id")]
    public long? OrderId { get; set; }

    [Column("cart_id")]
    public long? CartId { get; set; }

    [Column("coupon_id")]
    public long? CouponId { get; set; }

    [Column("campaign_id")]
    public long? CampaignId { get; set; }

    [Column("rule_id")]
    public long? RuleId { get; set; }

    [Column("amount", TypeName = "decimal(19, 4)")]
    public decimal Amount { get; set; }

    [Column("snapshot_json")]
    public string? SnapshotJson { get; set; }

    [Column("applied_at")]
    [Precision(0)]
    public DateTime AppliedAt { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("PromoApplicationLogs")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("PromoApplicationLogs")]
    public virtual OrdOrder? Order { get; set; }

    [ForeignKey("RuleId")]
    [InverseProperty("PromoApplicationLogs")]
    public virtual PromoRule? Rule { get; set; }
}
