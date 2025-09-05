using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class OrdOrderItem
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long VendorId { get; set; }

    public long OfferId { get; set; }

    public long SkuId { get; set; }

    public string NameSnapshot { get; set; } = null!;

    public string? AttributesSnapshot { get; set; }

    public decimal Qty { get; set; }

    public decimal UnitPriceGross { get; set; }

    public decimal Discount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal? LineTotal { get; set; }

    public virtual MktOffer Offer { get; set; } = null!;

    public virtual OrdOrder Order { get; set; } = null!;

    public virtual CatSku Sku { get; set; } = null!;

    public virtual MktVendor Vendor { get; set; } = null!;
}
