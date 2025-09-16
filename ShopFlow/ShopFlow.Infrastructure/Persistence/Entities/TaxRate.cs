using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("tax_rate")]
public partial class TaxRate
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("country")]
    [StringLength(2)]
    [Unicode(false)]
    public string Country { get; set; } = null!;

    [Column("region")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Region { get; set; }

    [Column("rate_percent", TypeName = "decimal(5, 2)")]
    public decimal RatePercent { get; set; }

    [InverseProperty("TaxRate")]
    public virtual ICollection<TaxRateRule> TaxRateRules { get; set; } = new List<TaxRateRule>();
}
