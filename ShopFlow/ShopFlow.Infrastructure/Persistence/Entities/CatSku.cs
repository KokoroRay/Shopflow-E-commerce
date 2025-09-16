using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("cat_sku")]
[Index("Barcode", Name = "IX_cat_sku_barcode")]
[Index("CreatedAt", Name = "IX_cat_sku_created", AllDescending = true)]
[Index("ProductId", Name = "IX_cat_sku_product")]
[Index("SkuCode", Name = "UQ__cat_sku__843F428F3122342E", IsUnique = true)]
public partial class CatSku
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("sku_code")]
    [StringLength(64)]
    [Unicode(false)]
    public string SkuCode { get; set; } = null!;

    [Column("barcode")]
    [StringLength(64)]
    [Unicode(false)]
    public string? Barcode { get; set; }

    [Column("options_json")]
    [StringLength(1000)]
    public string? OptionsJson { get; set; }

    [Column("weight_g")]
    public int? WeightG { get; set; }

    [Column("length_mm")]
    public int? LengthMm { get; set; }

    [Column("width_mm")]
    public int? WidthMm { get; set; }

    [Column("height_mm")]
    public int? HeightMm { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("auto_sku_generation")]
    public bool AutoSkuGeneration { get; set; }

    [Column("manual_sku_override")]
    [StringLength(64)]
    [Unicode(false)]
    public string? ManualSkuOverride { get; set; }

    [Column("barcode_type")]
    [StringLength(20)]
    [Unicode(false)]
    public string? BarcodeType { get; set; }

    [Column("auto_barcode_generation")]
    public bool AutoBarcodeGeneration { get; set; }

    [Column("cost_per_unit", TypeName = "decimal(19, 4)")]
    public decimal? CostPerUnit { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("updated_by")]
    public long? UpdatedBy { get; set; }

    [InverseProperty("Sku")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("Sku")]
    public virtual ICollection<CatSkuMedium> CatSkuMedia { get; set; } = new List<CatSkuMedium>();

    [InverseProperty("Sku")]
    public virtual ICollection<CatSkuOptionValue> CatSkuOptionValues { get; set; } = new List<CatSkuOptionValue>();

    [InverseProperty("Sku")]
    public virtual ICollection<CatSkuPricing> CatSkuPricings { get; set; } = new List<CatSkuPricing>();

    [InverseProperty("Sku")]
    public virtual ICollection<CeReview> CeReviews { get; set; } = new List<CeReview>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("CatSkuCreatedByNavigations")]
    public virtual CoreUser? CreatedByNavigation { get; set; }

    [InverseProperty("Sku")]
    public virtual ICollection<InvAdjustmentLine> InvAdjustmentLines { get; set; } = new List<InvAdjustmentLine>();

    [InverseProperty("Sku")]
    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    [InverseProperty("Sku")]
    public virtual ICollection<InvStockChangeLog> InvStockChangeLogs { get; set; } = new List<InvStockChangeLog>();

    [InverseProperty("Sku")]
    public virtual ICollection<InvStock> InvStocks { get; set; } = new List<InvStock>();

    [InverseProperty("Sku")]
    public virtual ICollection<MktOffer> MktOffers { get; set; } = new List<MktOffer>();

    [InverseProperty("Sku")]
    public virtual ICollection<OrdOrderItem> OrdOrderItems { get; set; } = new List<OrdOrderItem>();

    [ForeignKey("ProductId")]
    [InverseProperty("CatSkus")]
    public virtual CatProduct Product { get; set; } = null!;

    [ForeignKey("UpdatedBy")]
    [InverseProperty("CatSkuUpdatedByNavigations")]
    public virtual CoreUser? UpdatedByNavigation { get; set; }

    [ForeignKey("SkuId")]
    [InverseProperty("Skus")]
    public virtual ICollection<TaxClass> TaxClasses { get; set; } = new List<TaxClass>();
}
