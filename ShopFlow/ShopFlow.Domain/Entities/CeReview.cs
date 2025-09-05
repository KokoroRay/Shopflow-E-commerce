using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CeReview
{
    public long Id { get; set; }

    public long? OrderId { get; set; }

    public long? UserId { get; set; }

    public long ProductId { get; set; }

    public long? SkuId { get; set; }

    public long VendorId { get; set; }

    public byte Rating { get; set; }

    public string? Content { get; set; }

    public string? ImagesJson { get; set; }

    public byte Status { get; set; }

    public long? ModeratedBy { get; set; }

    public DateTime? ModeratedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CoreUser? ModeratedByNavigation { get; set; }

    public virtual OrdOrder? Order { get; set; }

    public virtual CatProduct Product { get; set; } = null!;

    public virtual CatSku? Sku { get; set; }

    public virtual CoreUser? User { get; set; }

    public virtual MktVendor Vendor { get; set; } = null!;
}
