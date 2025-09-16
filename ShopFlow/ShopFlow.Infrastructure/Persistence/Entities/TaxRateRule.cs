using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("tax_rate_rule")]
[Index("TaxClassId", "EffectiveFrom", Name = "IX_trr_class", IsDescending = new[] { false, true })]
public partial class TaxRateRule
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("tax_class_id")]
    public int TaxClassId { get; set; }

    [Column("tax_rate_id")]
    public int TaxRateId { get; set; }

    [Column("effective_from")]
    [Precision(0)]
    public DateTime EffectiveFrom { get; set; }

    [Column("effective_to")]
    [Precision(0)]
    public DateTime? EffectiveTo { get; set; }

    [ForeignKey("TaxClassId")]
    [InverseProperty("TaxRateRules")]
    public virtual TaxClass TaxClass { get; set; } = null!;

    [ForeignKey("TaxRateId")]
    [InverseProperty("TaxRateRules")]
    public virtual TaxRate TaxRate { get; set; } = null!;
}
