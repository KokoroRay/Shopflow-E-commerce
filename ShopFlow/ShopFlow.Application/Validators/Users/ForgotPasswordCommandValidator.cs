using FluentValidation;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Validators.Users;

/// <summary>
/// Validator for the ForgotPasswordCommand
/// </summary>
public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    /// <summary>
    /// Initializes a new instance of the ForgotPasswordCommandValidator
    /// </summary>
    public ForgotPasswordCommandValidator()
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
}
