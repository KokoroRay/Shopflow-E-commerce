using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class MktOffer
{
    public long Id { get; set; }

    public long VendorId { get; set; }

    public long SkuId { get; set; }

    public decimal? PriceGrossVnd { get; set; }

    public decimal? PriceGrossUsd { get; set; }

    public byte CurrencyPolicy { get; set; }

    public byte Status { get; set; }

    public decimal? MinQty { get; set; }

    public decimal? MaxQty { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrdOrderItem> OrdOrderItems { get; set; } = new List<OrdOrderItem>();

    public virtual CatSku Sku { get; set; } = null!;

    public virtual MktVendor Vendor { get; set; } = null!;
}
