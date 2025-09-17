using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("promo_coupon")]
[Index("Code", Name = "UQ__promo_co__357D4CF95A251AC3", IsUnique = true)]
public partial class PromoCoupon
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("campaign_id")]
    public long CampaignId { get; set; }

    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("max_uses")]
    public int? MaxUses { get; set; }

    [Column("max_uses_per_user")]
    public int? MaxUsesPerUser { get; set; }

    [Column("used")]
    public int Used { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [ForeignKey("CampaignId")]
    [InverseProperty("PromoCoupons")]
    public virtual PromoCampaign Campaign { get; set; } = null!;
}
