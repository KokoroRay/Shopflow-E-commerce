using System;
using System.Collections.Generic;
using System.Linq;
using ShopFlow.Domain.DomainEvents;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Domain.Entities;

public class CatCategory : BaseEntity
{
    private readonly List<CatCategory> _children = new();
    private readonly List<CatProduct> _products = new();
    private readonly List<CatCategoryI18n> _translations = new();

    public long? ParentId { get; private set; }
    public CategoryName Name { get; private set; }
    public CategorySlug Slug { get; private set; }
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? IconUrl { get; private set; }
    public int SortOrder { get; private set; }
    public CategoryStatus Status { get; private set; }
    public string? MetaTitle { get; private set; }
    public string? MetaDescription { get; private set; }
    public string? MetaKeywords { get; private set; }
    public bool IsLeaf => _children.Count == 0;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public CatCategory? Parent { get; private set; }
    public IReadOnlyCollection<CatCategory> Children => _children.AsReadOnly();
    public IReadOnlyCollection<CatProduct> Products => _products.AsReadOnly();
    public IReadOnlyCollection<CatCategoryI18n> Translations => _translations.AsReadOnly();

    // Private constructor for EF
    private CatCategory()
    {
        Name = null!;
        Slug = null!;
    }

    public CatCategory(
        CategoryName name,
        CategorySlug slug,
        string? description = null,
        long? parentId = null,
        int sortOrder = 0)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        Description = description;
        ParentId = parentId;
        SortOrder = sortOrder;
        Status = CategoryStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryCreatedEvent(Id, Name.Value, Slug.Value, ParentId));
    }

    public void UpdateBasicInfo(CategoryName name, string? description = null)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        var oldName = Name.Value;
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryUpdatedEvent(Id, oldName, Name.Value));
    }

    public void UpdateName(CategoryName name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        var oldName = Name.Value;
        Name = name;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryUpdatedEvent(Id, oldName, Name.Value));
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSlug(CategorySlug slug)
    {
        if (slug == null) throw new ArgumentNullException(nameof(slug));

        var oldSlug = Slug.Value;
        Slug = slug;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategorySlugChangedEvent(Id, oldSlug, Slug.Value));
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateImages(string? imageUrl, string? iconUrl)
    {
        ImageUrl = imageUrl;
        IconUrl = iconUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSeoMeta(string? metaTitle, string? metaDescription, string? metaKeywords)
    {
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        MetaKeywords = metaKeywords;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeParent(long? newParentId)
    {
        if (newParentId == Id)
            throw new InvalidOperationException("Category cannot be its own parent");

        // No change needed if parent is the same
        if (ParentId == newParentId) return;

        var oldParentId = ParentId;
        ParentId = newParentId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryParentChangedEvent(Id, oldParentId, newParentId));
    }

    public void Activate()
    {
        if (Status == CategoryStatus.Active) return;

        Status = CategoryStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryStatusChangedEvent(Id, CategoryStatus.Inactive, CategoryStatus.Active));
    }

    public void Deactivate()
    {
        if (Status == CategoryStatus.Inactive) return;

        if (_products.Any())
            throw new InvalidOperationException("Cannot deactivate category with products");

        Status = CategoryStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryStatusChangedEvent(Id, CategoryStatus.Active, CategoryStatus.Inactive));
    }

    public void Delete()
    {
        if (Status == CategoryStatus.Deleted) return;

        if (_products.Any())
            throw new InvalidOperationException("Cannot delete category with products");

        if (_children.Any())
            throw new InvalidOperationException("Cannot delete category with subcategories");

        Status = CategoryStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CategoryDeletedEvent(Id, Name.Value));
    }

    public void AddProduct(CatProduct product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        if (_products.Contains(product)) return;

        _products.Add(product);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveProduct(CatProduct product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));

        _products.Remove(product);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanHaveChildren()
    {
        return Status == CategoryStatus.Active;
    }

    public bool CanBeDeleted()
    {
        return !_products.Any() && !_children.Any() && Status != CategoryStatus.Deleted;
    }
}
