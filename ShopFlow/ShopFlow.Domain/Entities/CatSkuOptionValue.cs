using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CatSkuOptionValue
{
    public long SkuId { get; set; }

    public long AttributeId { get; set; }

    public long OptionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CatAttribute Attribute { get; set; } = null!;

    public virtual CatAttributeOption Option { get; set; } = null!;

    public virtual CatSku Sku { get; set; } = null!;
}
