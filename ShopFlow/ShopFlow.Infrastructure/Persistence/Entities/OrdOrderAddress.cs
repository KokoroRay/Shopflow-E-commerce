using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("ord_order_address")]
[Index("OrderId", "AddrType", Name = "UQ_order_addr_type", IsUnique = true)]
public partial class OrdOrderAddress
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("order_id")]
    public long OrderId { get; set; }

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

    [ForeignKey("OrderId")]
    [InverseProperty("OrdOrderAddresses")]
    public virtual OrdOrder Order { get; set; } = null!;
}
