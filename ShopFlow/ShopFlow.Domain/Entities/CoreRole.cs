using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CoreRole
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<CoreUserRole> CoreUserRoles { get; set; } = new List<CoreUserRole>();

    public virtual ICollection<CorePermission> Permissions { get; set; } = new List<CorePermission>();
}
