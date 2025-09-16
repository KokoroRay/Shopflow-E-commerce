using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cat_category")]
[Index("ParentId", Name = "IX_cat_category_parent")]
public partial class CatCategory
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("parent_id")]
    public long? ParentId { get; set; }

    [Column("sort")]
    public int Sort { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<CatCategory> InverseParent { get; set; } = new List<CatCategory>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual CatCategory? Parent { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Categories")]
    public virtual ICollection<CatProduct> Products { get; set; } = new List<CatProduct>();
}
