using FluentValidation;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Validators.Users;

/// <summary>
/// Validator for the ResetPasswordCommand
/// </summary>
public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    /// <summary>
    /// Initializes a new instance of the ResetPasswordCommandValidator
    /// </summary>
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(254)
            .WithMessage("Email address is too long")
            .Must(BeValidEmailFormat)
            .WithMessage("Email format is invalid");

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .WithMessage("OTP code is required")
            .Length(6)
            .WithMessage("OTP code must be exactly 6 digits")
            .Matches(@"^\d{6}$")
            .WithMessage("OTP code must contain only digits")
            .Must(BeValidOtpFormat)
            .WithMessage("Invalid OTP code format");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(128)
            .WithMessage("Password cannot exceed 128 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one digit");
    }

    /// <summary>
    /// Validates if the email format is valid using the Email value object
    /// </summary>
    /// <param name="email">The email to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    private static bool BeValidEmailFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return Email.IsValid(email);
    }

    /// <summary>
    /// Validates if the OTP code format is valid using the OtpCode value object
    /// </summary>
    /// <param name="otpCode">The OTP code to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    private static bool BeValidOtpFormat(string otpCode)
    {
        if (string.IsNullOrWhiteSpace(otpCode))
            return false;

        return OtpCode.IsValidFormat(otpCode);
    }
}
