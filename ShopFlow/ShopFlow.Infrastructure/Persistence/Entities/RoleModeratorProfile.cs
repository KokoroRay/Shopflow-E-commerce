using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("role_moderator_profile")]
public partial class RoleModeratorProfile
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("team")]
    [StringLength(100)]
    public string? Team { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId, RoleId")]
    [InverseProperty("RoleModeratorProfiles")]
    public virtual CoreUserRole CoreUserRole { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("RoleModeratorProfile")]
    public virtual CoreUser User { get; set; } = null!;
}
