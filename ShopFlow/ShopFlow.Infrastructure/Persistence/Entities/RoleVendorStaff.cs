using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[PrimaryKey("VendorId", "UserId")]
[Table("role_vendor_staff")]
public partial class RoleVendorStaff
{
    [Key]
    [Column("vendor_id")]
    public long VendorId { get; set; }

    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("title")]
    [StringLength(100)]
    public string? Title { get; set; }

    [Column("permissions_json")]
    public string? PermissionsJson { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId, RoleId")]
    [InverseProperty("RoleVendorStaffs")]
    public virtual CoreUserRole CoreUserRole { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("RoleVendorStaffs")]
    public virtual CoreUser User { get; set; } = null!;

    [ForeignKey("VendorId")]
    [InverseProperty("RoleVendorStaffs")]
    public virtual MktVendor Vendor { get; set; } = null!;
}
