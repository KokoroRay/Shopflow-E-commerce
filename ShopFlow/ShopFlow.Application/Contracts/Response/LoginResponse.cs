namespace ShopFlow.Application.Contracts.Response;

/// <summary>
/// Response model for successful user login
/// </summary>
public sealed record LoginResponse
{
    /// <summary>
    /// JWT access token for authentication
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Refresh token for renewing access tokens
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// Token expiration time in UTC
    /// </summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>
    /// User information
    /// </summary>
    public UserInfo User { get; init; } = null!;
}

/// <summary>
/// User information included in login response
/// </summary>
public sealed record UserInfo
{
    /// <summary>
    /// User ID
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string? FullName { get; init; }

    /// <summary>
    /// User roles
    /// </summary>
    public IEnumerable<string> Roles { get; init; } = new List<string>();

    /// <summary>
    /// Primary role for determining default page/dashboard
    /// </summary>
    public string? PrimaryRole { get; init; }
}
