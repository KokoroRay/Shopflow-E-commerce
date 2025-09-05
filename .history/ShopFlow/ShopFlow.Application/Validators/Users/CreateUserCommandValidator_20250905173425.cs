using FluentValidation;
using ShopFlow.Application.Commands.Users;

namespace ShopFlow.Application.Validators.Users;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit and one special character");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Phone number format is invalid")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.Gender)
            .Must(gender => string.IsNullOrEmpty(gender) || new[] { "Male", "Female", "Other" }.Contains(gender))
            .WithMessage("Gender must be Male, Female, or Other");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now.AddYears(-13))
            .WithMessage("Must be at least 13 years old")
            .When(x => x.DateOfBirth.HasValue);
    }
}
