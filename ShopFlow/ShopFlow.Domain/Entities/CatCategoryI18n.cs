using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CatCategoryI18n
{
    public long CategoryId { get; set; }

    public string Lang { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? MetaTitle { get; set; }

    public string? MetaDesc { get; set; }

    public virtual CatCategory Category { get; set; } = null!;
}
