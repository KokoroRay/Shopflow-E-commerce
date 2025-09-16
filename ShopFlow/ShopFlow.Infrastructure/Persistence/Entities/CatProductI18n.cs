using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Keyless]
[Table("cat_product_i18n")]
[Index("ProductId", "Lang", Name = "UQ_cat_product_i18n", IsUnique = true)]
[Index("Lang", "Slug", Name = "UQ_cat_product_slug", IsUnique = true)]
public partial class CatProductI18n
{
    [Column("product_id")]
    public long ProductId { get; set; }

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

    [Column("short_desc")]
    [StringLength(1000)]
    public string? ShortDesc { get; set; }

    [Column("long_desc")]
    public string? LongDesc { get; set; }

    [ForeignKey("ProductId")]
    public virtual CatProduct Product { get; set; } = null!;
}
