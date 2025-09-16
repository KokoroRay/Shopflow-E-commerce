using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Keyless]
[Table("cat_category_i18n")]
[Index("CategoryId", "Lang", Name = "UQ_cat_category_i18n", IsUnique = true)]
[Index("Lang", "Slug", Name = "UQ_cat_category_slug", IsUnique = true)]
public partial class CatCategoryI18n
{
    [Column("category_id")]
    public long CategoryId { get; set; }

    [Column("lang")]
    [StringLength(2)]
    [Unicode(false)]
    public string Lang { get; set; } = null!;

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("slug")]
    [StringLength(255)]
    public string Slug { get; set; } = null!;

    [Column("meta_title")]
    [StringLength(255)]
    public string? MetaTitle { get; set; }

    [Column("meta_desc")]
    [StringLength(500)]
    public string? MetaDesc { get; set; }

    [ForeignKey("CategoryId")]
    public virtual CatCategory Category { get; set; } = null!;
}
