using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class RoleVendorStaff
{
    public long VendorId { get; set; }

    public long UserId { get; set; }

    public int RoleId { get; set; }

    public string? Title { get; set; }

    public string? PermissionsJson { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CoreUserRole CoreUserRole { get; set; } = null!;

    public virtual CoreUser User { get; set; } = null!;

    public virtual MktVendor Vendor { get; set; } = null!;
}
