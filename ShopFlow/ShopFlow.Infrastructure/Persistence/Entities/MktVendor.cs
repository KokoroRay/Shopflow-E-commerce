using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("mkt_vendor")]
[Index("Code", Name = "UQ__mkt_vend__357D4CF93AA2C749", IsUnique = true)]
public partial class MktVendor
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("tax_code")]
    [StringLength(50)]
    [Unicode(false)]
    public string? TaxCode { get; set; }

    [Column("invoice_profile_json")]
    public string? InvoiceProfileJson { get; set; }

    [Column("return_policy_json")]
    public string? ReturnPolicyJson { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Vendor")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("Vendor")]
    public virtual ICollection<CeReview> CeReviews { get; set; } = new List<CeReview>();

    [InverseProperty("Vendor")]
    public virtual ICollection<InvAdjustment> InvAdjustments { get; set; } = new List<InvAdjustment>();

    [InverseProperty("Vendor")]
    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    [InverseProperty("Vendor")]
    public virtual ICollection<InvStockChangeLog> InvStockChangeLogs { get; set; } = new List<InvStockChangeLog>();

    [InverseProperty("Vendor")]
    public virtual ICollection<InvStock> InvStocks { get; set; } = new List<InvStock>();

    [InverseProperty("Vendor")]
    public virtual ICollection<MktOffer> MktOffers { get; set; } = new List<MktOffer>();

    [InverseProperty("Vendor")]
    public virtual ICollection<OrdOrderItem> OrdOrderItems { get; set; } = new List<OrdOrderItem>();

    [InverseProperty("Vendor")]
    public virtual ICollection<RoleVendorStaff> RoleVendorStaffs { get; set; } = new List<RoleVendorStaff>();

    [InverseProperty("Vendor")]
    public virtual ICollection<SysExtMapping> SysExtMappings { get; set; } = new List<SysExtMapping>();

    [InverseProperty("Vendor")]
    public virtual ICollection<SysIntegration> SysIntegrations { get; set; } = new List<SysIntegration>();

    [InverseProperty("Vendor")]
    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; set; } = new List<SysWebhookEvent>();
}
