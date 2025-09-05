using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class OrdOrder
{
    public long Id { get; set; }

    public string OrderCode { get; set; } = null!;

    public long? UserId { get; set; }

    public long WarehouseId { get; set; }

    public string Currency { get; set; } = null!;

    public decimal? FxRate { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountTotal { get; set; }

    public decimal TaxTotal { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal GrandTotal { get; set; }

    public byte Status { get; set; }

    public DateTime PlacedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? CancelReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CeReview> CeReviews { get; set; } = new List<CeReview>();

    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    public virtual ICollection<OrdOrderAddress> OrdOrderAddresses { get; set; } = new List<OrdOrderAddress>();

    public virtual ICollection<OrdOrderItem> OrdOrderItems { get; set; } = new List<OrdOrderItem>();

    public virtual OrdShipment? OrdShipment { get; set; }

    public virtual ICollection<PayTransaction> PayTransactions { get; set; } = new List<PayTransaction>();

    public virtual ICollection<PromoApplicationLog> PromoApplicationLogs { get; set; } = new List<PromoApplicationLog>();

    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; set; } = new List<SysWebhookEvent>();

    public virtual CoreUser? User { get; set; }

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
