namespace ShopFlow.Application.Contracts.Response;

/// <summary>
/// Generic paged response for Vietnamese marketplace pagination
/// </summary>
/// <typeparam name="T">Type of items in the response</typeparam>
public record PagedResponse<T>
{
    /// <summary>
    /// Collection of items for current page
    /// </summary>
    public IEnumerable<T> Items { get; init; } = new List<T>();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int CurrentPage { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Number of items on current page
    /// </summary>
    public int CurrentPageCount => Items.Count();

    /// <summary>
    /// Starting item number for current page
    /// </summary>
    public int StartIndex => (CurrentPage - 1) * PageSize + 1;

    /// <summary>
    /// Ending item number for current page
    /// </summary>
    public int EndIndex => Math.Min(StartIndex + PageSize - 1, TotalCount);
}