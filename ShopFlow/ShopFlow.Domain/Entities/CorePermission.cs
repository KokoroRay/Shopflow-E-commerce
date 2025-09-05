using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CorePermission
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<CoreRole> Roles { get; set; } = new List<CoreRole>();
}
