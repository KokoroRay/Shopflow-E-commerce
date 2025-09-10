using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Application.Contracts.Requests;

/// <summary>
/// Request to initiate a password reset process
/// </summary>
public sealed class ForgotPasswordRequest
{
    /// <summary>
    /// The email address to send the reset OTP to
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(254, ErrorMessage = "Email address is too long")]
    public string Email { get; set; } = string.Empty;
}
