using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CatAttributeOptionI18n
{
    public long OptionId { get; set; }

    public string Lang { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual CatAttributeOption Option { get; set; } = null!;
}
