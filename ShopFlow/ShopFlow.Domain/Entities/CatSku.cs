using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CatSku
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string SkuCode { get; set; } = null!;

    public string? Barcode { get; set; }

    public string? OptionsJson { get; set; }

    public int? WeightG { get; set; }

    public int? LengthMm { get; set; }

    public int? WidthMm { get; set; }

    public int? HeightMm { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<CatSkuMedium> CatSkuMedia { get; set; } = new List<CatSkuMedium>();

    public virtual ICollection<CatSkuOptionValue> CatSkuOptionValues { get; set; } = new List<CatSkuOptionValue>();

    public virtual ICollection<CeReview> CeReviews { get; set; } = new List<CeReview>();

    public virtual ICollection<InvAdjustmentLine> InvAdjustmentLines { get; set; } = new List<InvAdjustmentLine>();

    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    public virtual ICollection<InvStock> InvStocks { get; set; } = new List<InvStock>();

    public virtual ICollection<MktOffer> MktOffers { get; set; } = new List<MktOffer>();

    public virtual ICollection<OrdOrderItem> OrdOrderItems { get; set; } = new List<OrdOrderItem>();

    public virtual CatProduct Product { get; set; } = null!;

    public virtual ICollection<TaxClass> TaxClasses { get; set; } = new List<TaxClass>();
}
