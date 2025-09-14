using FluentValidation;
using ShopFlow.Application.Commands.Products;

namespace ShopFlow.Application.Validators.Products;

/// <summary>
/// Validator for CreateProductCommand with Vietnamese marketplace-specific rules
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        // Basic Product Information
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
            .Length(3, 200).WithMessage("Tên sản phẩm phải từ 3 đến 200 ký tự")
            .Matches(@"^[\p{L}\p{N}\s\-_.,()]+$").WithMessage("Tên sản phẩm chứa ký tự không hợp lệ");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Mô tả ngắn không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.ShortDescription));

        RuleFor(x => x.LongDescription)
            .MaximumLength(5000).WithMessage("Mô tả chi tiết không được vượt quá 5000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.LongDescription));

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug không được để trống")
            .Length(3, 200).WithMessage("Slug phải từ 3 đến 200 ký tự")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug chỉ được chứa ký tự thường, số và dấu gạch ngang");

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("Phải chọn ít nhất một danh mục")
            .Must(categories => categories.Count <= 5)
            .WithMessage("Không được chọn quá 5 danh mục");

        RuleForEach(x => x.CategoryIds)
            .GreaterThan(0).WithMessage("ID danh mục phải lớn hơn 0");

        RuleFor(x => x.VendorId)
            .GreaterThan(0).WithMessage("ID nhà cung cấp phải lớn hơn 0");

        RuleFor(x => x.PrimaryLanguage)
            .NotEmpty().WithMessage("Ngôn ngữ chính không được để trống")
            .Must(lang => lang == "vi" || lang == "en")
            .WithMessage("Ngôn ngữ chỉ hỗ trợ 'vi' hoặc 'en'");

        // Product Type validation for Vietnamese marketplace (byte values)
        RuleFor(x => x.ProductType)
            .Must(type => type >= 1 && type <= 3)
            .WithMessage("Loại sản phẩm không hợp lệ (1: Vật lý, 2: Số, 3: Dịch vụ)");

        // Return days validation specific to Vietnamese e-commerce laws
        RuleFor(x => x.ReturnDays)
            .InclusiveBetween(7, 30).WithMessage("Số ngày đổi trả phải từ 7 đến 30 ngày")
            .When(x => x.ReturnDays.HasValue && x.ProductType == 1); // Physical products

        // VAT validation for Vietnamese tax compliance
        RuleFor(x => x.VatRate)
            .InclusiveBetween(0, 0.2m).WithMessage("Thuế VAT phải từ 0% đến 20%")
            .When(x => x.VatRate.HasValue);

        // Pricing validation
        RuleFor(x => x.Pricing)
            .NotEmpty().WithMessage("Phải có ít nhất một thông tin giá")
            .Must(pricing => pricing.Any(p => p.CurrencyCode == "VND"))
            .WithMessage("Phải có giá bằng VND cho thị trường Việt Nam");

        RuleForEach(x => x.Pricing).SetValidator(new CreateProductPricingRequestValidator());

        // Variants validation
        RuleForEach(x => x.Variants)
            .SetValidator(new CreateProductVariantRequestValidator())
            .When(x => x.Variants != null && x.Variants.Any());

        // Meta validation for SEO
        RuleFor(x => x.MetaTitle)
            .MaximumLength(60).WithMessage("Meta title không được vượt quá 60 ký tự")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(160).WithMessage("Meta description không được vượt quá 160 ký tự")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));

        // Tags validation
        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Không được có quá 10 tags");

        RuleForEach(x => x.Tags)
            .Length(2, 50).WithMessage("Mỗi tag phải từ 2 đến 50 ký tự")
            .When(x => x.Tags != null);

        // Tax code validation for Vietnamese businesses
        RuleFor(x => x.TaxCode)
            .Matches(@"^\d{10}(-\d{3})?$").WithMessage("Mã số thuế phải có định dạng 10 số hoặc 10 số-3 số")
            .When(x => !string.IsNullOrEmpty(x.TaxCode));
    }
}

/// <summary>
/// Validator for pricing requests
/// </summary>
public class CreateProductPricingRequestValidator : AbstractValidator<CreateProductPricingRequest>
{
    public CreateProductPricingRequestValidator()
    {
        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("Mã tiền tệ không được để trống")
            .Must(currency => currency == "VND" || currency == "USD")
            .WithMessage("Chỉ hỗ trợ VND và USD");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("Giá cơ bản phải lớn hơn 0");

        RuleFor(x => x.SalePrice)
            .LessThan(x => x.BasePrice).WithMessage("Giá khuyến mãi phải thấp hơn giá cơ bản")
            .When(x => x.SalePrice.HasValue);

        RuleFor(x => x.SaleStartDate)
            .LessThan(x => x.SaleEndDate).WithMessage("Ngày bắt đầu khuyến mãi phải trước ngày kết thúc")
            .When(x => x.SaleStartDate.HasValue && x.SaleEndDate.HasValue);
    }
}

/// <summary>
/// Validator for variant requests
/// </summary>
public class CreateProductVariantRequestValidator : AbstractValidator<CreateProductVariantRequest>
{
    public CreateProductVariantRequestValidator()
    {
        RuleFor(x => x.VariantType)
            .NotEmpty().WithMessage("Loại biến thể không được để trống")
            .Length(1, 50).WithMessage("Loại biến thể phải từ 1 đến 50 ký tự");

        RuleFor(x => x.VariantValue)
            .NotEmpty().WithMessage("Giá trị biến thể không được để trống")
            .Length(1, 100).WithMessage("Giá trị biến thể phải từ 1 đến 100 ký tự");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho phải lớn hơn hoặc bằng 0");

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0).WithMessage("ID kho phải lớn hơn 0")
            .When(x => x.WarehouseId.HasValue);
    }
}