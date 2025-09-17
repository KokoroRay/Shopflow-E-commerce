using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("role_admin_profile")]
public partial class RoleAdminProfile
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("level")]
    public byte Level { get; set; }

    [Column("note")]
    [StringLength(500)]
    public string? Note { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId, RoleId")]
    [InverseProperty("RoleAdminProfiles")]
    public virtual CoreUserRole CoreUserRole { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("RoleAdminProfile")]
    public virtual CoreUser User { get; set; } = null!;
}
