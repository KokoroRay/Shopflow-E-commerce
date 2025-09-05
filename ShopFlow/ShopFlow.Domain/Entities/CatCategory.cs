using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CatCategory
{
    public long Id { get; set; }

    public long? ParentId { get; set; }

    public int Sort { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CatCategory> InverseParent { get; set; } = new List<CatCategory>();

    public virtual CatCategory? Parent { get; set; }

    public virtual ICollection<CatProduct> Products { get; set; } = new List<CatProduct>();
}
