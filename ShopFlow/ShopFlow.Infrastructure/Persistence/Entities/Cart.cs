using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cart")]
[Index("UserId", Name = "IX_cart_user")]
public partial class Cart
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public long? UserId { get; set; }

    [Column("guest_token")]
    [StringLength(64)]
    [Unicode(false)]
    public string? GuestToken { get; set; }

    [Column("currency")]
    [StringLength(3)]
    [Unicode(false)]
    public string Currency { get; set; } = null!;

    [Column("warehouse_id")]
    public long? WarehouseId { get; set; }

    [Column("totals_json")]
    public string? TotalsJson { get; set; }

    [Column("last_activity_at")]
    [Precision(0)]
    public DateTime LastActivityAt { get; set; }

    [InverseProperty("Cart")]
    public virtual ICollection<CartAppliedPromo> CartAppliedPromos { get; set; } = new List<CartAppliedPromo>();

    [InverseProperty("Cart")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("Cart")]
    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    [InverseProperty("Cart")]
    public virtual ICollection<PromoApplicationLog> PromoApplicationLogs { get; set; } = new List<PromoApplicationLog>();

    [InverseProperty("Cart")]
    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; set; } = new List<SysWebhookEvent>();

    [ForeignKey("UserId")]
    [InverseProperty("Carts")]
    public virtual CoreUser? User { get; set; }

    [ForeignKey("WarehouseId")]
    [InverseProperty("Carts")]
    public virtual InvWarehouse? Warehouse { get; set; }
}
