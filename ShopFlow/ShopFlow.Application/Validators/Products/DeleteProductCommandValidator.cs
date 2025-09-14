using FluentValidation;
using ShopFlow.Application.Commands.Products;

namespace ShopFlow.Application.Validators.Products;

/// <summary>
/// Validator for DeleteProductCommand - Vietnamese marketplace specific validation
/// </summary>
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than 0");

        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .WithMessage("Vendor ID must be greater than 0")
            .When(x => x.VendorId.HasValue);

        RuleFor(x => x.AdminNotes)
            .MaximumLength(1000)
            .WithMessage("Admin notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.AdminNotes));
    }
}