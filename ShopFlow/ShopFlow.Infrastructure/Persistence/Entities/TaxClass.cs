using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("tax_class")]
[Index("Code", Name = "UQ__tax_clas__357D4CF90297EC6E", IsUnique = true)]
public partial class TaxClass
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [InverseProperty("TaxClass")]
    public virtual ICollection<TaxRateRule> TaxRateRules { get; set; } = new List<TaxRateRule>();

    [ForeignKey("TaxClassId")]
    [InverseProperty("TaxClasses")]
    public virtual ICollection<CatSku> Skus { get; set; } = new List<CatSku>();
}
