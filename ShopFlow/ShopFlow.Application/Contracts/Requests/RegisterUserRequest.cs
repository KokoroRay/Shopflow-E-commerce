namespace ShopFlow.Application.Contracts.Requests;

/// <summary>
/// Request model for user registration
/// </summary>
public sealed class RegisterUserRequest
{
    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number (optional)
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the full name (optional)
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the gender (optional)
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// Gets or sets the date of birth (optional)
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
}
