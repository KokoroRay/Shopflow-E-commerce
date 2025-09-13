using System;
using System.Collections.Generic;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Domain.Entities;

public class CatProduct : BaseEntity
{
    private readonly List<CatSku> _skus = new();
    private readonly List<CeReview> _reviews = new();
    private readonly List<CatCategory> _categories = new();

    // Vietnamese Marketplace Properties
    public ProductName Name { get; private set; } = null!;
    public ProductSlug Slug { get; private set; } = null!;

    public byte ProductType { get; private set; }
    public ProductStatus Status { get; private set; }
    public int? ReturnDays { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public IReadOnlyCollection<CatSku> Skus => _skus.AsReadOnly();
    public IReadOnlyCollection<CeReview> Reviews => _reviews.AsReadOnly();
    public IReadOnlyCollection<CatCategory> Categories => _categories.AsReadOnly();

    // Private constructor for EF
    private CatProduct() { }

    public CatProduct(ProductName name, ProductSlug slug, byte productType, int? returnDays = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        ProductType = productType;
        Status = ProductStatus.Draft;
        ReturnDays = returnDays;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == ProductStatus.Discontinued)
            throw new InvalidOperationException("Cannot activate a discontinued product");

        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Discontinue()
    {
        Status = ProductStatus.Discontinued;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateReturnDays(int? returnDays)
    {
        if (returnDays.HasValue && returnDays.Value < 0)
            throw new ArgumentException("Return days cannot be negative", nameof(returnDays));

        ReturnDays = returnDays;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSku(CatSku sku)
    {
        if (sku == null) throw new ArgumentNullException(nameof(sku));

        _skus.Add(sku);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCategory(CatCategory category)
    {
        if (category == null) throw new ArgumentNullException(nameof(category));

        if (!_categories.Contains(category))
        {
            _categories.Add(category);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveCategory(CatCategory category)
    {
        if (category == null) throw new ArgumentNullException(nameof(category));

        _categories.Remove(category);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive()
    {
        return Status == ProductStatus.Active;
    }

    public bool CanBeOrdered()
    {
        return Status == ProductStatus.Active && _skus.Any(s => s.IsActive);
    }
}
