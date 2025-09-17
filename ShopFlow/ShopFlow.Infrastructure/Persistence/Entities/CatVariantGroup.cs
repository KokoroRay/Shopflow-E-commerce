using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cat_variant_group")]
[Index("ProductId", "Code", Name = "UQ_variant_group_product_code", IsUnique = true)]
public partial class CatVariantGroup
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("parent_group_id")]
    public long? ParentGroupId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("is_required")]
    public bool IsRequired { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("ParentGroup")]
    public virtual ICollection<CatVariantGroup> InverseParentGroup { get; set; } = new List<CatVariantGroup>();

    [ForeignKey("ParentGroupId")]
    [InverseProperty("InverseParentGroup")]
    public virtual CatVariantGroup? ParentGroup { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("CatVariantGroups")]
    public virtual CatProduct Product { get; set; } = null!;
}
