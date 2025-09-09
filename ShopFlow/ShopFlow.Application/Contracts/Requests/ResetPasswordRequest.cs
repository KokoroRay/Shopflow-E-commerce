using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Application.Contracts.Requests;

/// <summary>
/// Request to reset user password using OTP verification
/// </summary>
public sealed class ResetPasswordRequest
{
    /// <summary>
    /// The email address of the user
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(254, ErrorMessage = "Email address is too long")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The OTP code received via email
    /// </summary>
    [Required(ErrorMessage = "OTP code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP code must be exactly 6 digits")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP code must contain only digits")]
    public string OtpCode { get; set; } = string.Empty;

    /// <summary>
    /// The new password to set
    /// </summary>
    [Required(ErrorMessage = "New password is required")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 128 characters")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation of the new password
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
