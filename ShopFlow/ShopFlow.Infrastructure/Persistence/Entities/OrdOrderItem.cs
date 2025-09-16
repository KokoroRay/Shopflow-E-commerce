using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("ord_order_item")]
[Index("OrderId", Name = "IX_order_item_order")]
[Index("VendorId", Name = "IX_order_item_vendor")]
public partial class OrdOrderItem
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("order_id")]
    public long OrderId { get; set; }

    [Column("vendor_id")]
    public long VendorId { get; set; }

    [Column("offer_id")]
    public long OfferId { get; set; }

    [Column("sku_id")]
    public long SkuId { get; set; }

    [Column("name_snapshot")]
    [StringLength(255)]
    public string NameSnapshot { get; set; } = null!;

    [Column("attributes_snapshot")]
    [StringLength(1000)]
    public string? AttributesSnapshot { get; set; }

    [Column("qty", TypeName = "decimal(18, 3)")]
    public decimal Qty { get; set; }

    [Column("unit_price_gross", TypeName = "decimal(19, 4)")]
    public decimal UnitPriceGross { get; set; }

    [Column("discount", TypeName = "decimal(19, 4)")]
    public decimal Discount { get; set; }

    [Column("tax_amount", TypeName = "decimal(19, 4)")]
    public decimal TaxAmount { get; set; }

    [Column("line_total", TypeName = "decimal(38, 6)")]
    public decimal? LineTotal { get; set; }

    [ForeignKey("OfferId")]
    [InverseProperty("OrdOrderItems")]
    public virtual MktOffer Offer { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("OrdOrderItems")]
    public virtual OrdOrder Order { get; set; } = null!;

    [ForeignKey("SkuId")]
    [InverseProperty("OrdOrderItems")]
    public virtual CatSku Sku { get; set; } = null!;

    [ForeignKey("VendorId")]
    [InverseProperty("OrdOrderItems")]
    public virtual MktVendor Vendor { get; set; } = null!;
}
