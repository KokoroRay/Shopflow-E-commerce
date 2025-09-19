using FluentValidation;
using ShopFlow.Application.Commands.Skus;
using System.Globalization;

namespace ShopFlow.Application.Validators.Skus;

/// <summary>
/// Validator for UpdateSkuCommand with Vietnamese marketplace rules
/// </summary>
public class UpdateSkuCommandValidator : AbstractValidator<UpdateSkuCommand>
{
    public UpdateSkuCommandValidator()
    {
        // ID validation
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID SKU không hợp lệ");

        // Name validation - Vietnamese characters supported
        RuleFor(x => x.Name)
            .Length(3, 200)
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Tên SKU phải từ 3 đến 200 ký tự")
            .Must(BeValidVietnameseName)
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Tên SKU chứa ký tự không hợp lệ");

        // Description validation
        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Mô tả không được vượt quá 2000 ký tự");

        // Price validation - Vietnamese Dong
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .When(x => x.Price.HasValue)
            .WithMessage("Giá bán phải lớn hơn 0")
            .LessThan(1_000_000_000m)
            .When(x => x.Price.HasValue)
            .WithMessage("Giá bán không được vượt quá 1 tỷ VNĐ")
            .Must(BeValidVndPrice)
            .When(x => x.Price.HasValue)
            .WithMessage("Giá bán phải là bội số của 1000 VNĐ");

        // Compare at price validation
        RuleFor(x => x.CompareAtPrice)
            .GreaterThan(x => x.Price ?? 0)
            .When(x => x.CompareAtPrice.HasValue && x.Price.HasValue)
            .WithMessage("Giá so sánh phải lớn hơn giá bán")
            .Must(BeValidVndPrice)
            .When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Giá so sánh phải là bội số của 1000 VNĐ");

        // Cost price validation
        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CostPrice.HasValue)
            .WithMessage("Giá vốn không được âm")
            .LessThan(x => x.Price ?? decimal.MaxValue)
            .When(x => x.CostPrice.HasValue && x.Price.HasValue)
            .WithMessage("Giá vốn phải nhỏ hơn giá bán")
            .Must(BeValidVndPrice)
            .When(x => x.CostPrice.HasValue)
            .WithMessage("Giá vốn phải là bội số của 1000 VNĐ");

        // Dimensions validation - Vietnamese postal service limits
        RuleFor(x => x.LengthMm)
            .GreaterThan(0)
            .When(x => x.LengthMm.HasValue)
            .WithMessage("Chiều dài phải lớn hơn 0")
            .LessThanOrEqualTo(600)
            .When(x => x.LengthMm.HasValue)
            .WithMessage("Chiều dài không được vượt quá 60cm (giới hạn bưu chính Việt Nam)");

        RuleFor(x => x.WidthMm)
            .GreaterThan(0)
            .When(x => x.WidthMm.HasValue)
            .WithMessage("Chiều rộng phải lớn hơn 0")
            .LessThanOrEqualTo(600)
            .When(x => x.WidthMm.HasValue)
            .WithMessage("Chiều rộng không được vượt quá 60cm (giới hạn bưu chính Việt Nam)");

        RuleFor(x => x.HeightMm)
            .GreaterThan(0)
            .When(x => x.HeightMm.HasValue)
            .WithMessage("Chiều cao phải lớn hơn 0")
            .LessThanOrEqualTo(600)
            .When(x => x.HeightMm.HasValue)
            .WithMessage("Chiều cao không được vượt quá 60cm (giới hạn bưu chính Việt Nam)");

        // Weight validation - Vietnamese postal service limits
        RuleFor(x => x.WeightGrams)
            .GreaterThan(0)
            .When(x => x.WeightGrams.HasValue)
            .WithMessage("Trọng lượng phải lớn hơn 0")
            .LessThanOrEqualTo(30000)
            .When(x => x.WeightGrams.HasValue)
            .WithMessage("Trọng lượng không được vượt quá 30kg (giới hạn bưu chính Việt Nam)");

        // At least one field must be provided for update
        RuleFor(x => x)
            .Must(HaveAtLeastOneFieldToUpdate)
            .WithMessage("Phải cung cấp ít nhất một trường để cập nhật");

        // Cross-field validation for shipping compatibility (when dimensions or weight are updated)
        RuleFor(x => x)
            .Must(HaveCompatibleDimensionsAndWeight)
            .When(x => HasDimensionOrWeightUpdate(x))
            .WithMessage("Kích thước và trọng lượng không phù hợp với tiêu chuẩn vận chuyển");
    }

    /// <summary>
    /// Validates Vietnamese Dong pricing (should be multiples of 1000)
    /// </summary>
    private static bool BeValidVndPrice(decimal? price)
    {
        if (!price.HasValue) return true;
        return price.Value % 1000 == 0;
    }

    /// <summary>
    /// Validates Vietnamese name (allows Vietnamese diacritics)
    /// </summary>
    private static bool BeValidVietnameseName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        
        // Allow Vietnamese characters, letters, numbers, spaces, and common punctuation
        var vietnamesePattern = @"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểếỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵýỷỹ0-9\s\-_.,()&]+$";
        
        return System.Text.RegularExpressions.Regex.IsMatch(name, vietnamesePattern);
    }

    /// <summary>
    /// Validates that at least one field is provided for update
    /// </summary>
    private static bool HaveAtLeastOneFieldToUpdate(UpdateSkuCommand command)
    {
        return !string.IsNullOrEmpty(command.Name) ||
               !string.IsNullOrEmpty(command.Description) ||
               command.Price.HasValue ||
               command.CompareAtPrice.HasValue ||
               command.CostPrice.HasValue ||
               command.IsActive.HasValue ||
               command.LengthMm.HasValue ||
               command.WidthMm.HasValue ||
               command.HeightMm.HasValue ||
               command.WeightGrams.HasValue;
    }

    /// <summary>
    /// Checks if the command has dimension or weight updates
    /// </summary>
    private static bool HasDimensionOrWeightUpdate(UpdateSkuCommand command)
    {
        return command.LengthMm.HasValue ||
               command.WidthMm.HasValue ||
               command.HeightMm.HasValue ||
               command.WeightGrams.HasValue;
    }

    /// <summary>
    /// Validates dimensions and weight compatibility for shipping (basic validation for partial updates)
    /// </summary>
    private static bool HaveCompatibleDimensionsAndWeight(UpdateSkuCommand command)
    {
        // For update commands, we can only do basic validation since we don't have all values
        // Full validation should be done in the handler with current SKU data
        
        // If updating weight, ensure it's reasonable
        if (command.WeightGrams.HasValue)
        {
            var weightKg = command.WeightGrams.Value / 1000m;
            
            // Basic weight sanity check
            if (weightKg > 30) return false; // Over 30kg not allowed for postal service
            if (weightKg < 0.001m) return false; // Less than 1g is suspicious
        }

        // If updating dimensions, do basic volume check
        if (command.LengthMm.HasValue || command.WidthMm.HasValue || command.HeightMm.HasValue)
        {
            // Individual dimension checks are already done in field validators
            // For cross-validation, we need all dimensions which we don't have in partial update
            return true; // Let the handler do the full validation with current SKU data
        }

        return true;
    }
}