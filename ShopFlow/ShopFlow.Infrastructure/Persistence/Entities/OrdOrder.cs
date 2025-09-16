using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("ord_order")]
[Index("PlacedAt", Name = "IX_order_placed", AllDescending = true)]
[Index("UserId", "PlacedAt", Name = "IX_order_user", IsDescending = new[] { false, true })]
[Index("OrderCode", Name = "UQ__ord_orde__99D12D3F43109DAA", IsUnique = true)]
public partial class OrdOrder
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("order_code")]
    [StringLength(20)]
    [Unicode(false)]
    public string OrderCode { get; set; } = null!;

    [Column("user_id")]
    public long? UserId { get; set; }

    [Column("warehouse_id")]
    public long WarehouseId { get; set; }

    [Column("currency")]
    [StringLength(3)]
    [Unicode(false)]
    public string Currency { get; set; } = null!;

    [Column("fx_rate", TypeName = "decimal(18, 8)")]
    public decimal? FxRate { get; set; }

    [Column("subtotal", TypeName = "decimal(19, 4)")]
    public decimal Subtotal { get; set; }

    [Column("discount_total", TypeName = "decimal(19, 4)")]
    public decimal DiscountTotal { get; set; }

    [Column("tax_total", TypeName = "decimal(19, 4)")]
    public decimal TaxTotal { get; set; }

    [Column("shipping_fee", TypeName = "decimal(19, 4)")]
    public decimal ShippingFee { get; set; }

    [Column("grand_total", TypeName = "decimal(19, 4)")]
    public decimal GrandTotal { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("placed_at")]
    [Precision(0)]
    public DateTime PlacedAt { get; set; }

    [Column("completed_at")]
    [Precision(0)]
    public DateTime? CompletedAt { get; set; }

    [Column("cancel_reason")]
    [StringLength(255)]
    public string? CancelReason { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<CeReview> CeReviews { get; set; } = new List<CeReview>();

    [InverseProperty("Order")]
    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    [InverseProperty("Order")]
    public virtual ICollection<OrdOrderAddress> OrdOrderAddresses { get; set; } = new List<OrdOrderAddress>();

    [InverseProperty("Order")]
    public virtual ICollection<OrdOrderItem> OrdOrderItems { get; set; } = new List<OrdOrderItem>();

    [InverseProperty("Order")]
    public virtual OrdShipment? OrdShipment { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<PayTransaction> PayTransactions { get; set; } = new List<PayTransaction>();

    [InverseProperty("Order")]
    public virtual ICollection<PromoApplicationLog> PromoApplicationLogs { get; set; } = new List<PromoApplicationLog>();

    [InverseProperty("Order")]
    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; set; } = new List<SysWebhookEvent>();

    [ForeignKey("UserId")]
    [InverseProperty("OrdOrders")]
    public virtual CoreUser? User { get; set; }

    [ForeignKey("WarehouseId")]
    [InverseProperty("OrdOrders")]
    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
