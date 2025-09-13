using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CatAttributeOption
{
    public long Id { get; set; }

    public long AttributeId { get; set; }

    public string Code { get; set; } = null!;

    public virtual CatAttribute Attribute { get; set; } = null!;

    public virtual ICollection<CatSkuOptionValue> CatSkuOptionValues { get; } = new List<CatSkuOptionValue>();
}
