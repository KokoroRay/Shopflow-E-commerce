using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("sys_webhook_event")]
[Index("OrderId", "CartId", "ProductId", Name = "IX_webhook_entity")]
[Index("IntegrationId", "Status", "CreatedAt", Name = "IX_webhook_integration")]
[Index("Status", "CreatedAt", Name = "IX_webhook_status")]
[Index("VendorId", "Status", "CreatedAt", Name = "IX_webhook_vendor")]
public partial class SysWebhookEvent
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("topic")]
    [StringLength(100)]
    [Unicode(false)]
    public string Topic { get; set; } = null!;

    [Column("payload_json")]
    public string PayloadJson { get; set; } = null!;

    [Column("attempts")]
    public int Attempts { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("last_attempt_at")]
    [Precision(0)]
    public DateTime? LastAttemptAt { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("vendor_id")]
    public long? VendorId { get; set; }

    [Column("integration_id")]
    public long? IntegrationId { get; set; }

    [Column("order_id")]
    public long? OrderId { get; set; }

    [Column("cart_id")]
    public long? CartId { get; set; }

    [Column("product_id")]
    public long? ProductId { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("SysWebhookEvents")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("IntegrationId")]
    [InverseProperty("SysWebhookEvents")]
    public virtual SysIntegration? Integration { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("SysWebhookEvents")]
    public virtual OrdOrder? Order { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("SysWebhookEvents")]
    public virtual CatProduct? Product { get; set; }

    [ForeignKey("VendorId")]
    [InverseProperty("SysWebhookEvents")]
    public virtual MktVendor? Vendor { get; set; }
}
