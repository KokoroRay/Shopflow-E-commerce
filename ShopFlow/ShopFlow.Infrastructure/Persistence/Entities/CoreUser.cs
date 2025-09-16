using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("core_user")]
[Index("Email", Name = "UQ__core_use__AB6E616475EA5D8A", IsUnique = true)]
[Index("Phone", Name = "UQ__core_use__B43B145F76BC2A4B", IsUnique = true)]
public partial class CoreUser
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("phone")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [Column("password_hash")]
    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [Column("status")]
    public byte Status { get; set; }

    [Column("email_verified")]
    public bool EmailVerified { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<CatSku> CatSkuCreatedByNavigations { get; set; } = new List<CatSku>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<CatSkuPricing> CatSkuPricings { get; set; } = new List<CatSkuPricing>();

    [InverseProperty("UpdatedByNavigation")]
    public virtual ICollection<CatSku> CatSkuUpdatedByNavigations { get; set; } = new List<CatSku>();

    [InverseProperty("ModeratedByNavigation")]
    public virtual ICollection<CeReview> CeReviewModeratedByNavigations { get; set; } = new List<CeReview>();

    [InverseProperty("User")]
    public virtual ICollection<CeReview> CeReviewUsers { get; set; } = new List<CeReview>();

    [InverseProperty("User")]
    public virtual ICollection<CoreAddress> CoreAddresses { get; set; } = new List<CoreAddress>();

    [InverseProperty("User")]
    public virtual ICollection<CoreUserRole> CoreUserRoles { get; set; } = new List<CoreUserRole>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<InvAdjustment> InvAdjustments { get; set; } = new List<InvAdjustment>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<InvStockChangeLog> InvStockChangeLogs { get; set; } = new List<InvStockChangeLog>();

    [InverseProperty("User")]
    public virtual ICollection<OrdOrder> OrdOrders { get; set; } = new List<OrdOrder>();

    [InverseProperty("User")]
    public virtual RoleAdminProfile? RoleAdminProfile { get; set; }

    [InverseProperty("User")]
    public virtual RoleCustomerProfile? RoleCustomerProfile { get; set; }

    [InverseProperty("User")]
    public virtual RoleModeratorProfile? RoleModeratorProfile { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<RoleVendorStaff> RoleVendorStaffs { get; set; } = new List<RoleVendorStaff>();

    [InverseProperty("User")]
    public virtual ICollection<RoleWarehouseStaff> RoleWarehouseStaffs { get; set; } = new List<RoleWarehouseStaff>();
}
