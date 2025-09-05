using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CartItem
{
    public long Id { get; set; }

    public long CartId { get; set; }

    public long OfferId { get; set; }

    public long VendorId { get; set; }

    public long SkuId { get; set; }

    public decimal Qty { get; set; }

    public decimal PriceGrossSnapshot { get; set; }

    public decimal TaxRateSnapshot { get; set; }

    public string? AttributesSnapshot { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual MktOffer Offer { get; set; } = null!;

    public virtual CatSku Sku { get; set; } = null!;

    public virtual MktVendor Vendor { get; set; } = null!;
}
