using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("sys_staging")]
[Index("IntegrationId", "Status", "CreatedAt", Name = "IX_staging_status")]
public partial class SysStaging
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("integration_id")]
    public long IntegrationId { get; set; }

    [Column("entity")]
    [StringLength(30)]
    [Unicode(false)]
    public string Entity { get; set; } = null!;

    [Column("direction")]
    public byte Direction { get; set; }

    [Column("payload_json")]
    public string PayloadJson { get; set; } = null!;

    [Column("status")]
    public byte Status { get; set; }

    [Column("error_msg")]
    [StringLength(1000)]
    public string? ErrorMsg { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("processed_at")]
    [Precision(0)]
    public DateTime? ProcessedAt { get; set; }

    [ForeignKey("IntegrationId")]
    [InverseProperty("SysStagings")]
    public virtual SysIntegration Integration { get; set; } = null!;
}
