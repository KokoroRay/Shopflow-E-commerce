using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class Cart
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string? GuestToken { get; set; }

    public string Currency { get; set; } = null!;

    public long? WarehouseId { get; set; }

    public string? TotalsJson { get; set; }

    public DateTime LastActivityAt { get; set; }

    public virtual ICollection<CartAppliedPromo> CartAppliedPromos { get; set; } = new List<CartAppliedPromo>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    public virtual ICollection<PromoApplicationLog> PromoApplicationLogs { get; set; } = new List<PromoApplicationLog>();

    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; set; } = new List<SysWebhookEvent>();

    public virtual CoreUser? User { get; set; }

    public virtual InvWarehouse? Warehouse { get; set; }
}
