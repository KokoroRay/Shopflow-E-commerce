using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

/// <summary>
/// Represents a shopping cart entity
/// </summary>
public partial class Cart
{
    /// <summary>
    /// Gets or sets the cart identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the user identifier (null for guest carts)
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Gets or sets the guest token for anonymous carts
    /// </summary>
    public string? GuestToken { get; set; }

    /// <summary>
    /// Gets or sets the cart currency
    /// </summary>
    public string Currency { get; set; } = null!;

    /// <summary>
    /// Gets or sets the warehouse identifier
    /// </summary>
    public long? WarehouseId { get; set; }

    /// <summary>
    /// Gets or sets the JSON representation of cart totals
    /// </summary>
    public string? TotalsJson { get; set; }

    /// <summary>
    /// Gets or sets the last activity timestamp
    /// </summary>
    public DateTime LastActivityAt { get; set; }

    /// <summary>
    /// Gets the collection of applied promotional codes
    /// </summary>
    public virtual ICollection<CartAppliedPromo> CartAppliedPromos { get; } = new List<CartAppliedPromo>();

    /// <summary>
    /// Gets the collection of cart items
    /// </summary>
    public virtual ICollection<CartItem> CartItems { get; } = new List<CartItem>();

    /// <summary>
    /// Gets the collection of inventory reservations
    /// </summary>
    public virtual ICollection<InvReservation> InvReservations { get; } = new List<InvReservation>();

    /// <summary>
    /// Gets the collection of promotion application logs
    /// </summary>
    public virtual ICollection<PromoApplicationLog> PromoApplicationLogs { get; } = new List<PromoApplicationLog>();

    /// <summary>
    /// Gets the collection of webhook events
    /// </summary>
    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; } = new List<SysWebhookEvent>();

    /// <summary>
    /// Gets or sets the associated user
    /// </summary>
    public virtual CoreUser? User { get; set; }

    /// <summary>
    /// Gets or sets the associated warehouse
    /// </summary>
    public virtual InvWarehouse? Warehouse { get; set; }
}
