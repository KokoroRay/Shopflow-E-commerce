using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CatProductI18n
{
    public long ProductId { get; set; }

    public string Lang { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? ShortDesc { get; set; }

    public string? LongDesc { get; set; }

    public virtual CatProduct Product { get; set; } = null!;
}
