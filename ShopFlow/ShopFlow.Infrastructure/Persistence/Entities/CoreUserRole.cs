using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[PrimaryKey("UserId", "RoleId")]
[Table("core_user_role")]
public partial class CoreUserRole
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Key]
    [Column("role_id")]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("CoreUserRoles")]
    public virtual CoreRole Role { get; set; } = null!;

    [InverseProperty("CoreUserRole")]
    public virtual ICollection<RoleAdminProfile> RoleAdminProfiles { get; set; } = new List<RoleAdminProfile>();

    [InverseProperty("CoreUserRole")]
    public virtual ICollection<RoleCustomerProfile> RoleCustomerProfiles { get; set; } = new List<RoleCustomerProfile>();

    [InverseProperty("CoreUserRole")]
    public virtual ICollection<RoleModeratorProfile> RoleModeratorProfiles { get; set; } = new List<RoleModeratorProfile>();

    [InverseProperty("CoreUserRole")]
    public virtual ICollection<RoleVendorStaff> RoleVendorStaffs { get; set; } = new List<RoleVendorStaff>();

    [InverseProperty("CoreUserRole")]
    public virtual ICollection<RoleWarehouseStaff> RoleWarehouseStaffs { get; set; } = new List<RoleWarehouseStaff>();

    [ForeignKey("UserId")]
    [InverseProperty("CoreUserRoles")]
    public virtual CoreUser User { get; set; } = null!;
}
