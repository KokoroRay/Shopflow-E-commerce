using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[PrimaryKey("SkuId", "AttributeId")]
[Table("cat_sku_option_value")]
[Index("AttributeId", Name = "IX_skuopt_attr")]
[Index("OptionId", "SkuId", Name = "UQ_sku_option", IsUnique = true)]
public partial class CatSkuOptionValue
{
    [Key]
    [Column("sku_id")]
    public long SkuId { get; set; }

    [Key]
    [Column("attribute_id")]
    public long AttributeId { get; set; }

    [Column("option_id")]
    public long OptionId { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("AttributeId")]
    [InverseProperty("CatSkuOptionValues")]
    public virtual CatAttribute Attribute { get; set; } = null!;

    [ForeignKey("OptionId")]
    [InverseProperty("CatSkuOptionValues")]
    public virtual CatAttributeOption Option { get; set; } = null!;

    [ForeignKey("SkuId")]
    [InverseProperty("CatSkuOptionValues")]
    public virtual CatSku Sku { get; set; } = null!;
}
