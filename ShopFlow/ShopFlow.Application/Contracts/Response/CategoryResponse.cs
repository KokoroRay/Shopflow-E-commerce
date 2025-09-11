namespace ShopFlow.Application.Contracts.Response;

/// <summary>
/// Response model for category information
/// </summary>
public class CategoryResponse
{
    /// <summary>
    /// Category identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category slug
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Category description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Parent category identifier
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// Sort order
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Category image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Category icon URL
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Category status
    /// </summary>
    public byte Status { get; set; }

    /// <summary>
    /// Whether the category is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Response model for category hierarchy
/// </summary>
public class CategoryHierarchyResponse : CategoryResponse
{
    /// <summary>
    /// Child categories
    /// </summary>
    public List<CategoryHierarchyResponse> Children { get; set; } = new();

    /// <summary>
    /// Whether this category has children
    /// </summary>
    public bool HasChildren => Children.Count > 0;

    /// <summary>
    /// Depth level in the hierarchy (root = 0)
    /// </summary>
    public int Level { get; set; }
}

/// <summary>
/// Response model for category list with pagination
/// </summary>
public class CategoryListResponse
{
    /// <summary>
    /// List of categories
    /// </summary>
    public List<CategoryResponse> Categories { get; set; } = new();

    /// <summary>
    /// Total count of categories
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there are more pages
    /// </summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// Whether there are previous pages
    /// </summary>
    public bool HasPrevious => Page > 1;
}