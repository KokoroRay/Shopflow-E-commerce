using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cat_product")]
public partial class CatProduct
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [Column("slug")]
    [StringLength(255)]
    public string Slug { get; set; }

    [Column("product_type")]
    public byte ProductType { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("return_days")]
    public int? ReturnDays { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<CatSku> Skus { get; set; } = new List<CatSku>();

    [InverseProperty("Product")]
    public virtual ICollection<CatVariantGroup> CatVariantGroups { get; set; } = new List<CatVariantGroup>();

    [InverseProperty("Product")]
    public virtual ICollection<CeReview> CeReviews { get; set; } = new List<CeReview>();

    [InverseProperty("Product")]
    public virtual ICollection<SysWebhookEvent> SysWebhookEvents { get; set; } = new List<SysWebhookEvent>();

    [ForeignKey("ProductId")]
    [InverseProperty("Products")]
    public virtual ICollection<CatCategory> Categories { get; set; } = new List<CatCategory>();
}
