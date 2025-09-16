using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("mkt_offer")]
[Index("SkuId", Name = "IX_offer_sku")]
[Index("VendorId", Name = "IX_offer_vendor")]
[Index("VendorId", "SkuId", Name = "UQ_offer_vendor_sku", IsUnique = true)]
public partial class MktOffer
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("vendor_id")]
    public long VendorId { get; set; }

    [Column("sku_id")]
    public long SkuId { get; set; }

    [Column("price_gross_vnd", TypeName = "decimal(19, 4)")]
    public decimal? PriceGrossVnd { get; set; }

    [Column("price_gross_usd", TypeName = "decimal(19, 4)")]
    public decimal? PriceGrossUsd { get; set; }

    [Column("currency_policy")]
    public byte CurrencyPolicy { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("min_qty", TypeName = "decimal(18, 3)")]
    public decimal? MinQty { get; set; }

    [Column("max_qty", TypeName = "decimal(18, 3)")]
    public decimal? MaxQty { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Offer")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("Offer")]
    public virtual ICollection<OrdOrderItem> OrdOrderItems { get; set; } = new List<OrdOrderItem>();

    [ForeignKey("SkuId")]
    [InverseProperty("MktOffers")]
    public virtual CatSku Sku { get; set; } = null!;

    [ForeignKey("VendorId")]
    [InverseProperty("MktOffers")]
    public virtual MktVendor Vendor { get; set; } = null!;
}
