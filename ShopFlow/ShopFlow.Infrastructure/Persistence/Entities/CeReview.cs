using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("ce_review")]
[Index("ProductId", "VendorId", "Status", "CreatedAt", Name = "IX_review_target", IsDescending = new[] { false, false, false, true })]
[Index("UserId", "CreatedAt", Name = "IX_review_user", IsDescending = new[] { false, true })]
public partial class CeReview
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("order_id")]
    public long? OrderId { get; set; }

    [Column("user_id")]
    public long? UserId { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("sku_id")]
    public long? SkuId { get; set; }

    [Column("vendor_id")]
    public long VendorId { get; set; }

    [Column("rating")]
    public byte Rating { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("images_json")]
    public string? ImagesJson { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("moderated_by")]
    public long? ModeratedBy { get; set; }

    [Column("moderated_at")]
    [Precision(0)]
    public DateTime? ModeratedAt { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ModeratedBy")]
    [InverseProperty("CeReviewModeratedByNavigations")]
    public virtual CoreUser? ModeratedByNavigation { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("CeReviews")]
    public virtual OrdOrder? Order { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("CeReviews")]
    public virtual CatProduct Product { get; set; } = null!;

    [ForeignKey("SkuId")]
    [InverseProperty("CeReviews")]
    public virtual CatSku? Sku { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("CeReviewUsers")]
    public virtual CoreUser? User { get; set; }

    [ForeignKey("VendorId")]
    [InverseProperty("CeReviews")]
    public virtual MktVendor Vendor { get; set; } = null!;
}
