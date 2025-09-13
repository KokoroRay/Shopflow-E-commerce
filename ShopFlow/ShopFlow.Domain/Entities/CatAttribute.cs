using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CatAttribute
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string DataType { get; set; } = null!;

    public virtual ICollection<CatAttributeOption> CatAttributeOptions { get; } = new List<CatAttributeOption>();

    public virtual ICollection<CatSkuOptionValue> CatSkuOptionValues { get; } = new List<CatSkuOptionValue>();
}
