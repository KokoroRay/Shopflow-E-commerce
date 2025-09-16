using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cat_attribute_option")]
[Index("AttributeId", "Code", Name = "UQ_attr_option", IsUnique = true)]
public partial class CatAttributeOption
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("attribute_id")]
    public long AttributeId { get; set; }

    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [ForeignKey("AttributeId")]
    [InverseProperty("CatAttributeOptions")]
    public virtual CatAttribute Attribute { get; set; } = null!;

    [InverseProperty("Option")]
    public virtual ICollection<CatSkuOptionValue> CatSkuOptionValues { get; set; } = new List<CatSkuOptionValue>();
}
