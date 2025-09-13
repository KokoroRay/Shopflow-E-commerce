using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Asp.Versioning;
using ShopFlow.Application.Commands.Categories;
using ShopFlow.Application.Queries.Categories;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Domain.Enums;
using static ShopFlow.Application.Abstractions.Security.RoleAttributes;

namespace ShopFlow.API.Controllers;

/// <summary>
/// Controller for managing categories
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize] // Require authentication by default
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the CategoriesController class
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Create a new category (Admin only)
    /// </summary>
    /// <param name="request">Create category request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category response</returns>
    [HttpPost]
    [AdminOnly]
    [RequirePermission(PermissionCode.MANAGE_CATEGORIES)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest("Request cannot be null");

        var result = await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing category (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Update category request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category response</returns>
    [HttpPut("{id}")]
    [AdminOnly]
    [RequirePermission(PermissionCode.MANAGE_CATEGORIES)]
    public async Task<IActionResult> UpdateCategory(long id, [FromBody] UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest("Request cannot be null");

        if (id != request.Id)
            return BadRequest("ID in URL does not match ID in request body");

        var result = await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Delete a category (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [AdminOnly]
    [RequirePermission(PermissionCode.MANAGE_CATEGORIES)]
    public async Task<IActionResult> DeleteCategory(long id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);
        await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Activate a category (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful</returns>
    [HttpPatch("{id}/activate")]
    [AdminOnly]
    [RequirePermission(PermissionCode.MANAGE_CATEGORIES)]
    public async Task<IActionResult> ActivateCategory(long id, CancellationToken cancellationToken)
    {
        var command = new ActivateCategoryCommand(id);
        await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Deactivate a category (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful</returns>
    [HttpPatch("{id}/deactivate")]
    [AdminOnly]
    [RequirePermission(PermissionCode.MANAGE_CATEGORIES)]
    public async Task<IActionResult> DeactivateCategory(long id, CancellationToken cancellationToken)
    {
        var command = new DeactivateCategoryCommand(id);
        await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Get a category by ID (Public)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous] // Public access for reading categories
    public async Task<IActionResult> GetCategory(long id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryQuery(id);
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (result == null)
            return NotFound($"Category with ID {id} not found");

        return Ok(result);
    }

    /// <summary>
    /// Get a category by slug (Public)
    /// </summary>
    /// <param name="slug">Category slug</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category details</returns>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous] // Public access for reading categories
    public async Task<IActionResult> GetCategoryBySlug(string slug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return BadRequest("Slug cannot be empty");

        var query = new GetCategoryBySlugQuery(slug);
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        if (result == null)
            return NotFound($"Category with slug '{slug}' not found");

        return Ok(result);
    }

    /// <summary>
    /// Get all root categories (categories without parent) (Public)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of root categories</returns>
    [HttpGet("roots")]
    [AllowAnonymous] // Public access for reading categories
    public async Task<IActionResult> GetRootCategories(CancellationToken cancellationToken)
    {
        var query = new GetRootCategoriesQuery();
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Get child categories of a parent category (Public)
    /// </summary>
    /// <param name="parentId">Parent category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of child categories</returns>
    [HttpGet("{parentId}/children")]
    [AllowAnonymous] // Public access for reading categories
    public async Task<IActionResult> GetChildCategories(long parentId, CancellationToken cancellationToken)
    {
        var query = new GetChildCategoriesQuery(parentId);
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Get category hierarchy (tree structure) (Public)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category hierarchy</returns>
    [HttpGet("hierarchy")]
    [AllowAnonymous] // Public access for reading categories
    public async Task<IActionResult> GetCategoryHierarchy(CancellationToken cancellationToken)
    {
        var query = new GetCategoryHierarchyQuery();
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Get all categories with pagination (Admin or Staff access)
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of categories</returns>
    [HttpGet]
    [AdminOrModerator] // Requires admin or moderator access for full category listing
    public async Task<IActionResult> GetAllCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var query = new GetAllCategoriesQuery(page, pageSize);
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    #region Vietnamese Marketplace Category Features

    /// <summary>
    /// Get categories with Vietnamese translations - Public access for Vietnamese marketplace
    /// </summary>
    /// <param name="language">Language code (vi/en) - default: vi</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Categories with Vietnamese localization</returns>
    [HttpGet("localized")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLocalizedCategories(
        [FromQuery] string language = "vi",
        CancellationToken cancellationToken = default)
    {
        // Use existing GetRootCategories for simplicity - in real implementation,
        // this would be a specific localized query
        var query = new GetRootCategoriesQuery();
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        return Ok(new
        {
            Language = language,
            Message = $"Vietnamese marketplace: Categories in {language}",
            Categories = result
        });
    }

    /// <summary>
    /// Get popular categories for Vietnamese marketplace homepage
    /// </summary>
    /// <param name="limit">Number of categories to return (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Popular categories for Vietnamese market</returns>
    [HttpGet("popular")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopularCategories(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        // For now, use root categories - in real implementation,
        // this would be based on product count/popularity metrics
        var query = new GetRootCategoriesQuery();
        var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);

        var popularCategories = result.Take(limit).ToList();

        return Ok(new
        {
            Message = $"Top {limit} popular categories for Vietnamese marketplace",
            Count = popularCategories.Count,
            Categories = popularCategories
        });
    }

    /// <summary>
    /// Get category statistics for Vietnamese marketplace analytics
    /// </summary>
    /// <param name="categoryId">Category ID (optional - if not provided, returns overall stats)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category statistics for Vietnamese market</returns>
    [HttpGet("stats")]
    [AdminOrModerator]
    public Task<IActionResult> GetCategoryStats(
        [FromQuery] long? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IActionResult>(Ok(new
        {
            Message = "Vietnamese marketplace category statistics",
            CategoryId = categoryId,
            Stats = new
            {
                TotalProducts = 0, // Placeholder - would come from database
                TotalVendors = 0,
                AverageProductPrice = 0m,
                TopSellingProducts = Array.Empty<string>(),
                LastUpdated = DateTime.UtcNow
            }
        }));
    }

    /// <summary>
    /// Search categories with Vietnamese text support
    /// </summary>
    /// <param name="query">Search term in Vietnamese or English</param>
    /// <param name="language">Language preference (vi/en)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching categories with Vietnamese search support</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchCategories(
        [FromQuery] string query,
        [FromQuery] string language = "vi",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Search query cannot be empty");

        // For now, use root categories - in real implementation,
        // this would search across category names and descriptions with Vietnamese text support
        var rootQuery = new GetRootCategoriesQuery();
        var allCategories = await _mediator.Send(rootQuery, cancellationToken).ConfigureAwait(false);

        // Simple search simulation - in real implementation would use proper Vietnamese text search
        var searchResults = allCategories
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                       (c.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        return Ok(new
        {
            SearchTerm = query,
            Language = language,
            ResultCount = searchResults.Count,
            Message = $"Vietnamese marketplace category search for '{query}'",
            Results = searchResults
        });
    }

    #endregion
}