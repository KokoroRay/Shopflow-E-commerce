using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class RoleModeratorProfile
{
    public long UserId { get; set; }

    public int RoleId { get; set; }

    public string? Team { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CoreUserRole CoreUserRole { get; set; } = null!;

    public virtual CoreUser User { get; set; } = null!;
}
