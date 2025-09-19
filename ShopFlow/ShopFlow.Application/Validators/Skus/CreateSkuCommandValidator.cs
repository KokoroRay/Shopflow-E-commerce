using FluentValidation;
using ShopFlow.Application.Commands.Skus;
using System.Globalization;

namespace ShopFlow.Application.Validators.Skus;

/// <summary>
/// Validator for CreateSkuCommand with Vietnamese marketplace rules
/// </summary>
public class CreateSkuCommandValidator : AbstractValidator<CreateSkuCommand>
{
    public CreateSkuCommandValidator()
    {
        // SKU Code validation
        RuleFor(x => x.Code)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.Code))
            .WithMessage("Mã SKU không được vượt quá 50 ký tự")
            .Matches(@"^[A-Z0-9_-]+$")
            .When(x => !string.IsNullOrEmpty(x.Code))
            .WithMessage("Mã SKU chỉ được chứa chữ hoa, số, dấu gạch dưới và gạch ngang");

        // Barcode validation
        RuleFor(x => x.Barcode)
            .Must(BeValidBarcode)
            .When(x => !string.IsNullOrEmpty(x.Barcode))
            .WithMessage("Mã vạch không hợp lệ. Phải là EAN13 (13 số), UPC-A (12 số), hoặc CODE128");

        // Barcode type validation
        RuleFor(x => x.BarcodeType)
            .Must(BeValidBarcodeType)
            .When(x => !string.IsNullOrEmpty(x.BarcodeType))
            .WithMessage("Loại mã vạch phải là: EAN13, EAN8, UPC-A, UPC-E, CODE128, hoặc CODE39");

        // Name validation - Vietnamese characters supported
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên SKU là bắt buộc")
            .Length(3, 200)
            .WithMessage("Tên SKU phải từ 3 đến 200 ký tự")
            .Must(BeValidVietnameseName)
            .WithMessage("Tên SKU chứa ký tự không hợp lệ");

        // Description validation
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Mô tả SKU là bắt buộc")
            .MaximumLength(2000)
            .WithMessage("Mô tả không được vượt quá 2000 ký tự");

        // Price validation - Vietnamese Dong
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Giá bán phải lớn hơn 0")
            .LessThan(1_000_000_000m)
            .WithMessage("Giá bán không được vượt quá 1 tỷ VNĐ")
            .Must(BeValidVndPrice)
            .WithMessage("Giá bán phải là bội số của 1000 VNĐ");

        // Compare at price validation
        RuleFor(x => x.CompareAtPrice)
            .GreaterThan(x => x.Price)
            .When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Giá so sánh phải lớn hơn giá bán")
            .Must(BeValidVndPrice)
            .When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Giá so sánh phải là bội số của 1000 VNĐ");

        // Cost price validation
        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Giá vốn không được âm")
            .LessThan(x => x.Price)
            .WithMessage("Giá vốn phải nhỏ hơn giá bán")
            .Must(BeValidVndPrice)
            .WithMessage("Giá vốn phải là bội số của 1000 VNĐ");

        // Product ID validation
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ID sản phẩm không hợp lệ");

        // Dimensions validation - Vietnamese postal service limits
        RuleFor(x => x.LengthMm)
            .GreaterThan(0)
            .WithMessage("Chiều dài phải lớn hơn 0")
            .LessThanOrEqualTo(600)
            .WithMessage("Chiều dài không được vượt quá 60cm (giới hạn bưu chính Việt Nam)");

        RuleFor(x => x.WidthMm)
            .GreaterThan(0)
            .WithMessage("Chiều rộng phải lớn hơn 0")
            .LessThanOrEqualTo(600)
            .WithMessage("Chiều rộng không được vượt quá 60cm (giới hạn bưu chính Việt Nam)");

        RuleFor(x => x.HeightMm)
            .GreaterThan(0)
            .WithMessage("Chiều cao phải lớn hơn 0")
            .LessThanOrEqualTo(600)
            .WithMessage("Chiều cao không được vượt quá 60cm (giới hạn bưu chính Việt Nam)");

        // Weight validation - Vietnamese postal service limits
        RuleFor(x => x.WeightGrams)
            .GreaterThan(0)
            .WithMessage("Trọng lượng phải lớn hơn 0")
            .LessThanOrEqualTo(30000)
            .WithMessage("Trọng lượng không được vượt quá 30kg (giới hạn bưu chính Việt Nam)");

        // Option values validation
        RuleFor(x => x.OptionValues)
            .NotEmpty()
            .When(x => x.OptionValues != null)
            .WithMessage("Phải có ít nhất một thuộc tính biến thể");

        RuleForEach(x => x.OptionValues)
            .ChildRules(optionValue =>
            {
                optionValue.RuleFor(ov => ov.AttributeId)
                    .GreaterThan(0)
                    .WithMessage("ID thuộc tính không hợp lệ");

                optionValue.RuleFor(ov => ov.OptionId)
                    .GreaterThan(0)
                    .WithMessage("ID giá trị thuộc tính không hợp lệ");
            });

        // Cross-field validation for shipping compatibility
        RuleFor(x => x)
            .Must(HaveCompatibleDimensionsAndWeight)
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
    /// Validates Vietnamese Dong pricing (should be multiples of 1000)
    /// </summary>
    private static bool BeValidVndPrice(decimal price)
    {
        return price % 1000 == 0;
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
    /// Validates barcode format
    /// </summary>
    private static bool BeValidBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode)) return true;

        // EAN13: 13 digits
        if (barcode.Length == 13 && barcode.All(char.IsDigit))
            return ValidateEan13Checksum(barcode);

        // UPC-A: 12 digits
        if (barcode.Length == 12 && barcode.All(char.IsDigit))
            return ValidateUpcChecksum(barcode);

        // EAN8: 8 digits
        if (barcode.Length == 8 && barcode.All(char.IsDigit))
            return ValidateEan8Checksum(barcode);

        // CODE128: alphanumeric
        if (barcode.Length >= 6 && barcode.Length <= 48)
            return ValidateCode128(barcode);

        return false;
    }

    /// <summary>
    /// Validates barcode type
    /// </summary>
    private static bool BeValidBarcodeType(string barcodeType)
    {
        var validTypes = new[] { "EAN13", "EAN8", "UPC-A", "UPC-E", "CODE128", "CODE39" };
        return validTypes.Contains(barcodeType?.ToUpperInvariant());
    }

    /// <summary>
    /// Validates EAN13 checksum
    /// </summary>
    private static bool ValidateEan13Checksum(string barcode)
    {
        if (barcode.Length != 13) return false;

        var sum = 0;
        for (var i = 0; i < 12; i++)
        {
            var digit = int.Parse(barcode[i].ToString(), CultureInfo.InvariantCulture);
            sum += (i % 2 == 0) ? digit : digit * 3;
        }

        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == int.Parse(barcode[12].ToString(), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Validates UPC checksum
    /// </summary>
    private static bool ValidateUpcChecksum(string barcode)
    {
        if (barcode.Length != 12) return false;

        var sum = 0;
        for (var i = 0; i < 11; i++)
        {
            var digit = int.Parse(barcode[i].ToString(), CultureInfo.InvariantCulture);
            sum += (i % 2 == 0) ? digit * 3 : digit;
        }

        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == int.Parse(barcode[11].ToString(), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Validates EAN8 checksum
    /// </summary>
    private static bool ValidateEan8Checksum(string barcode)
    {
        if (barcode.Length != 8) return false;

        var sum = 0;
        for (var i = 0; i < 7; i++)
        {
            var digit = int.Parse(barcode[i].ToString(), CultureInfo.InvariantCulture);
            sum += (i % 2 == 0) ? digit : digit * 3;
        }

        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == int.Parse(barcode[7].ToString(), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Validates CODE128 format
    /// </summary>
    private static bool ValidateCode128(string barcode)
    {
        // Basic validation for CODE128 - alphanumeric characters
        return barcode.All(c => char.IsLetterOrDigit(c) || "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~".Contains(c));
    }

    /// <summary>
    /// Validates dimensions and weight compatibility for shipping
    /// </summary>
    private static bool HaveCompatibleDimensionsAndWeight(CreateSkuCommand command)
    {
        // Calculate volume in cubic centimeters
        var volumeCm3 = (command.LengthMm / 10m) * (command.WidthMm / 10m) * (command.HeightMm / 10m);
        var weightKg = command.WeightGrams / 1000m;

        // Vietnamese postal service dimensional weight formula: Volume(cm3) / 6000
        var dimensionalWeight = volumeCm3 / 6000m;

        // Check if actual weight and dimensional weight are reasonable
        if (dimensionalWeight > weightKg * 10) // Dimensional weight too high compared to actual weight
            return false;

        // Check for extremely light items with large dimensions (suspicious)
        if (volumeCm3 > 1000 && weightKg < 0.1m) // 1 liter volume but less than 100g
            return false;

        return true;
    }
}