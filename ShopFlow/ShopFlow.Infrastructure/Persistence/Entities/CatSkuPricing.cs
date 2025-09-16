using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cat_sku_pricing")]
[Index("SkuId", "EffectiveFrom", Name = "IX_cat_sku_pricing_sku", IsDescending = new[] { false, true })]
public partial class CatSkuPricing
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("sku_id")]
    public long SkuId { get; set; }

    [Column("pricing_type")]
    [StringLength(20)]
    [Unicode(false)]
    public string PricingType { get; set; } = null!;

    [Column("base_price_vnd", TypeName = "decimal(19, 4)")]
    public decimal? BasePriceVnd { get; set; }

    [Column("base_price_usd", TypeName = "decimal(19, 4)")]
    public decimal? BasePriceUsd { get; set; }

    [Column("sale_price_vnd", TypeName = "decimal(19, 4)")]
    public decimal? SalePriceVnd { get; set; }

    [Column("sale_price_usd", TypeName = "decimal(19, 4)")]
    public decimal? SalePriceUsd { get; set; }

    [Column("cost_price_vnd", TypeName = "decimal(19, 4)")]
    public decimal? CostPriceVnd { get; set; }

    [Column("markup_percent", TypeName = "decimal(5, 2)")]
    public decimal? MarkupPercent { get; set; }

    [Column("effective_from")]
    [Precision(0)]
    public DateTime EffectiveFrom { get; set; }

    [Column("effective_to")]
    [Precision(0)]
    public DateTime? EffectiveTo { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("CatSkuPricings")]
    public virtual CoreUser? CreatedByNavigation { get; set; }

    [ForeignKey("SkuId")]
    [InverseProperty("CatSkuPricings")]
    public virtual CatSku Sku { get; set; } = null!;
}
