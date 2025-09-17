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

    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [Column("slug")]
    [StringLength(255)]
    public string Slug { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("sort")]
    public int Sort { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<CatCategory> InverseParent { get; set; } = new List<CatCategory>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual CatCategory? Parent { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<CatCategoryI18n> Translations { get; set; } = new List<CatCategoryI18n>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Categories")]
    public virtual ICollection<CatProduct> Products { get; set; } = new List<CatProduct>();
}
