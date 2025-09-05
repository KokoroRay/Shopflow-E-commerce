using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CoreUserRole
{
    public long UserId { get; set; }

    public int RoleId { get; set; }

    public virtual CoreRole Role { get; set; } = null!;

    public virtual ICollection<RoleAdminProfile> RoleAdminProfiles { get; set; } = new List<RoleAdminProfile>();

    public virtual ICollection<RoleCustomerProfile> RoleCustomerProfiles { get; set; } = new List<RoleCustomerProfile>();

    public virtual ICollection<RoleModeratorProfile> RoleModeratorProfiles { get; set; } = new List<RoleModeratorProfile>();

    public virtual ICollection<RoleVendorStaff> RoleVendorStaffs { get; set; } = new List<RoleVendorStaff>();

    public virtual ICollection<RoleWarehouseStaff> RoleWarehouseStaffs { get; set; } = new List<RoleWarehouseStaff>();

    public virtual CoreUser User { get; set; } = null!;
}
