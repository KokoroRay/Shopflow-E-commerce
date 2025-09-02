using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class cat_category
{
    public long id { get; set; }

    public long? parent_id { get; set; }

    public int sort { get; set; }

    public bool is_active { get; set; }

    public virtual ICollection<cat_category> Inverseparent { get; set; } = new List<cat_category>();

    public virtual cat_category? parent { get; set; }

    public virtual ICollection<cat_product> products { get; set; } = new List<cat_product>();
}
