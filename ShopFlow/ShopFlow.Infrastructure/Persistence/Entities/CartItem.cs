using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cart_item")]
[Index("CartId", Name = "IX_cart_item_cart")]
public partial class CartItem
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("cart_id")]
    public long CartId { get; set; }

    [Column("offer_id")]
    public long OfferId { get; set; }

    [Column("vendor_id")]
    public long VendorId { get; set; }

    [Column("sku_id")]
    public long SkuId { get; set; }

    [Column("qty", TypeName = "decimal(18, 3)")]
    public decimal Qty { get; set; }

    [Column("price_gross_snapshot", TypeName = "decimal(19, 4)")]
    public decimal PriceGrossSnapshot { get; set; }

    [Column("tax_rate_snapshot", TypeName = "decimal(5, 2)")]
    public decimal TaxRateSnapshot { get; set; }

    [Column("attributes_snapshot")]
    [StringLength(1000)]
    public string? AttributesSnapshot { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("CartItems")]
    public virtual Cart Cart { get; set; } = null!;

    [ForeignKey("OfferId")]
    [InverseProperty("CartItems")]
    public virtual MktOffer Offer { get; set; } = null!;

    [ForeignKey("SkuId")]
    [InverseProperty("CartItems")]
    public virtual CatSku Sku { get; set; } = null!;

    [ForeignKey("VendorId")]
    [InverseProperty("CartItems")]
    public virtual MktVendor Vendor { get; set; } = null!;
}
