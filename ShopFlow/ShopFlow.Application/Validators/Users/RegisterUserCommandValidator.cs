using FluentValidation;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Validators.Users;

/// <summary>
/// Validator for RegisterUserCommand
/// </summary>
public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    /// <summary>
    /// Initializes a new instance of the RegisterUserCommandValidator class
    /// </summary>
    public RegisterUserCommandValidator()
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
            .WithMessage("Password must be at least 8 characters long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");

        RuleFor(x => x.Phone)
            .Must(BeValidPhoneNumber)
            .WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.FullName)
            .MaximumLength(200)
            .WithMessage("Full name cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.Gender)
            .Must(BeValidGender)
            .WithMessage("Gender must be 'M', 'F', or 'O'")
            .When(x => !string.IsNullOrWhiteSpace(x.Gender));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today)
            .WithMessage("Date of birth must be in the past")
            .GreaterThan(DateTime.Today.AddYears(-120))
            .WithMessage("Date of birth cannot be more than 120 years ago")
            .When(x => x.DateOfBirth.HasValue);
    }

    private static bool BeValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return Email.IsValid(email);
    }

    private static bool BeValidPhoneNumber(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true;

        try
        {
            _ = new PhoneNumber(phone);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static bool BeValidGender(string? gender)
    {
        if (string.IsNullOrWhiteSpace(gender))
            return true;

        return gender.ToUpperInvariant() is "M" or "F" or "O";
    }
}
