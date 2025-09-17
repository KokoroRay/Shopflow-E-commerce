using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("role_customer_profile")]
public partial class RoleCustomerProfile
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("full_name")]
    [StringLength(200)]
    public string? FullName { get; set; }

    [Column("birthday")]
    public DateOnly? Birthday { get; set; }

    [Column("gender")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Gender { get; set; }

    [Column("loyalty_points")]
    public int LoyaltyPoints { get; set; }

    [Column("marketing_optin")]
    public bool MarketingOptin { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId, RoleId")]
    [InverseProperty("RoleCustomerProfiles")]
    public virtual CoreUserRole CoreUserRole { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("RoleCustomerProfile")]
    public virtual CoreUser User { get; set; } = null!;
}
