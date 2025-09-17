using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("sys_integration")]
public partial class SysIntegration
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("vendor_id")]
    public long? VendorId { get; set; }

    [Column("type")]
    [StringLength(20)]
    [Unicode(false)]
    public string Type { get; set; } = null!;

    [Column("config_json")]
    public string? ConfigJson { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Integration")]
    public virtual ICollection<SysStaging> SysStagings { get; set; } = new List<SysStaging>();

    [InverseProperty("Integration")]
    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; set; } = new List<SysWebhookEvent>();

    [ForeignKey("VendorId")]
    [InverseProperty("SysIntegrations")]
    public virtual MktVendor? Vendor { get; set; }
}
