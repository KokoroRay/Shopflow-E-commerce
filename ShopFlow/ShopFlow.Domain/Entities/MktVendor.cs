using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class MktVendor
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? TaxCode { get; set; }

    public string? InvoiceProfileJson { get; set; }

    public string? ReturnPolicyJson { get; set; }

    public byte Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<CeReview> CeReviews { get; set; } = new List<CeReview>();

    public virtual ICollection<InvAdjustment> InvAdjustments { get; set; } = new List<InvAdjustment>();

    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    public virtual ICollection<InvStock> InvStocks { get; set; } = new List<InvStock>();

    public virtual ICollection<MktOffer> MktOffers { get; set; } = new List<MktOffer>();

    public virtual ICollection<OrdOrderItem> OrdOrderItems { get; set; } = new List<OrdOrderItem>();

    public virtual ICollection<RoleVendorStaff> RoleVendorStaffs { get; set; } = new List<RoleVendorStaff>();

    public virtual ICollection<SysExtMapping> SysExtMappings { get; set; } = new List<SysExtMapping>();

    public virtual ICollection<SysIntegration> SysIntegrations { get; set; } = new List<SysIntegration>();

    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; set; } = new List<SysWebhookEvent>();
}
