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
}