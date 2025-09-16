using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cat_attribute")]
[Index("Code", Name = "UQ__cat_attr__357D4CF9E5D564CD", IsUnique = true)]
public partial class CatAttribute
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("data_type")]
    [StringLength(20)]
    [Unicode(false)]
    public string DataType { get; set; } = null!;

    [InverseProperty("Attribute")]
    public virtual ICollection<CatAttributeOption> CatAttributeOptions { get; set; } = new List<CatAttributeOption>();

    [InverseProperty("Attribute")]
    public virtual ICollection<CatSkuOptionValue> CatSkuOptionValues { get; set; } = new List<CatSkuOptionValue>();
}
