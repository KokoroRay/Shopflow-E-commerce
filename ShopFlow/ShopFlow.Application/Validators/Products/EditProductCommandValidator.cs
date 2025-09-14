using FluentValidation;
using ShopFlow.Application.Commands.Products;

namespace ShopFlow.Application.Validators.Products;

/// <summary>
/// Validator for EditProductCommand - Vietnamese marketplace specific validation
/// </summary>
public class EditProductCommandValidator : AbstractValidator<EditProductCommand>
{
    public EditProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MinimumLength(2)
            .WithMessage("Product name must be at least 2 characters")
            .MaximumLength(255)
            .WithMessage("Product name cannot exceed 255 characters")
            .Must(BeValidVietnameseProductName)
            .WithMessage("Product name contains invalid characters for Vietnamese marketplace");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(1000)
            .WithMessage("Short description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.ShortDescription));

        RuleFor(x => x.LongDescription)
            .MaximumLength(5000)
            .WithMessage("Long description cannot exceed 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.LongDescription));

        RuleFor(x => x.ProductType)
            .GreaterThan((byte)0)
            .WithMessage("Product type must be greater than 0")
            .LessThanOrEqualTo((byte)10)
            .WithMessage("Product type must be between 1 and 10")
            .When(x => x.ProductType.HasValue);

        RuleFor(x => x.ReturnDays)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Return days cannot be negative")
            .LessThanOrEqualTo(365)
            .WithMessage("Return days cannot exceed 365 days")
            .When(x => x.ReturnDays.HasValue);

        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .WithMessage("Vendor ID must be greater than 0")
            .When(x => x.VendorId.HasValue);
    }

    /// <summary>
    /// Validates Vietnamese product name allowing Vietnamese characters, alphanumeric, and common symbols
    /// </summary>
    private static bool BeValidVietnameseProductName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // Allow Vietnamese characters (à-ỹ, À-Ỹ), alphanumeric, spaces, and common symbols
        // This regex pattern allows most common Vietnamese product names
        var allowedPattern = @"^[a-zA-Z0-9àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđĐÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸ\s\-\.\,\(\)\[\]\/\&\+\%\$\#\@\!\?]+$";

        return System.Text.RegularExpressions.Regex.IsMatch(name, allowedPattern);
    }
}