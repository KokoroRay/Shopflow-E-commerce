using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cat_category_i18n")]
[PrimaryKey("CategoryId", "Language")]
public partial class CatCategoryI18n
{
    [Column("category_id")]
    public long CategoryId { get; set; }

    [Column("language")]
    [StringLength(10)]
    public string Language { get; set; }

    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [Column("slug")]
    [StringLength(255)]
    public string Slug { get; set; }

    [Column("meta_title")]
    [StringLength(255)]
    public string? MetaTitle { get; set; }

    [Column("meta_description")]
    public string? MetaDescription { get; set; }

    [Column("meta_keywords")]
    public string? MetaKeywords { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Translations")]
    public virtual CatCategory Category { get; set; } = null!;
}