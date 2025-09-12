using System;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Domain.Entities;

public class CatCategoryI18n
{
    public long CategoryId { get; private set; }
    public string Language { get; private set; }
    public CategoryName Name { get; private set; }
    public CategorySlug Slug { get; private set; }
    public string? MetaTitle { get; private set; }
    public string? MetaDescription { get; private set; }
    public string? MetaKeywords { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public CatCategory Category { get; private set; } = null!;

    // Private constructor for EF
    private CatCategoryI18n() { }

    public CatCategoryI18n(
        long categoryId,
        string language,
        CategoryName name,
        CategorySlug slug,
        string? metaTitle = null,
        string? metaDescription = null,
        string? metaKeywords = null)
    {
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language cannot be empty", nameof(language));

        CategoryId = categoryId;
        Language = language.ToLowerInvariant();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        MetaKeywords = metaKeywords;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTranslation(
        CategoryName name,
        CategorySlug slug,
        string? metaTitle = null,
        string? metaDescription = null,
        string? metaKeywords = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        MetaKeywords = metaKeywords;
        UpdatedAt = DateTime.UtcNow;
    }
}
