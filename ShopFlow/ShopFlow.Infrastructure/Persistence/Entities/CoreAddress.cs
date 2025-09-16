using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("core_address")]
[Index("UserId", "AddrType", "IsDefault", Name = "IX_core_address_user", IsDescending = new[] { false, false, true })]
public partial class CoreAddress
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Column("addr_type")]
    public byte AddrType { get; set; }

    [Column("full_name")]
    [StringLength(120)]
    public string FullName { get; set; } = null!;

    [Column("phone")]
    [StringLength(20)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [Column("line1")]
    [StringLength(255)]
    public string Line1 { get; set; } = null!;

    [Column("line2")]
    [StringLength(255)]
    public string? Line2 { get; set; }

    [Column("ward")]
    [StringLength(120)]
    public string? Ward { get; set; }

    [Column("district")]
    [StringLength(120)]
    public string? District { get; set; }

    [Column("province")]
    [StringLength(120)]
    public string? Province { get; set; }

    [Column("country")]
    [StringLength(2)]
    [Unicode(false)]
    public string Country { get; set; } = null!;

    [Column("postal_code")]
    [StringLength(20)]
    public string? PostalCode { get; set; }

    [Column("is_default")]
    public bool IsDefault { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("CoreAddresses")]
    public virtual CoreUser User { get; set; } = null!;
}
