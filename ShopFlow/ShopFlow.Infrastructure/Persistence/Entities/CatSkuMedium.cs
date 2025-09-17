using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cat_sku_media")]
[Index("SkuId", "IsPrimary", "Sort", Name = "IX_cat_sku_media", IsDescending = new[] { false, true, false })]
public partial class CatSkuMedium
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("sku_id")]
    public long SkuId { get; set; }

    [Column("url")]
    [StringLength(1000)]
    public string Url { get; set; } = null!;

    [Column("is_primary")]
    public bool IsPrimary { get; set; }

    [Column("sort")]
    public int Sort { get; set; }

    [ForeignKey("SkuId")]
    [InverseProperty("CatSkuMedia")]
    public virtual CatSku Sku { get; set; } = null!;
}
