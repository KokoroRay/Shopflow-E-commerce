using FluentValidation;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Validators.Users;

/// <summary>
/// Validator for LoginCommand
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Initializes a new instance of the LoginCommandValidator class
    /// </summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .Must(BeValidEmail)
            .WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long");
    }

    /// <summary>
    /// Validates if the email format is correct
    /// </summary>
    private static bool BeValidEmail(string email)
    {
        try
        {
            _ = new Email(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
