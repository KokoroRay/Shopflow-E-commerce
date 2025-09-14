using FluentValidation.TestHelper;
using ShopFlow.Application.Commands.Products;
using ShopFlow.Application.Validators.Products;

namespace ShopFlow.Application.Tests.Validators.Products;

/// <summary>
/// Unit tests for CreateProductCommandValidator with Vietnamese marketplace validation rules
/// </summary>
public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: "Áo thun Vietnamese Designer",
            ShortDescription: "Áo thun thiết kế Việt Nam cao cấp",
            LongDescription: "Áo thun được thiết kế bởi designer Việt Nam, chất liệu cotton 100% thoáng mát, phù hợp với khí hậu nhiệt đới",
            Slug: "ao-thun-vietnamese-designer",
            ProductType: (byte)1, // Physical
            ReturnDays: 30,
            CategoryIds: new List<long> { 1L, 2L },
            VendorId: 1L,
            PrimaryLanguage: "vi",
            SecondaryLanguageContent: null,
            Pricing: new List<CreateProductPricingRequest>
            {
                new("VND", 299000m, null, null, null, null, null)
            },
            Variants: null,
            MetaTitle: "Áo thun designer Việt Nam",
            MetaDescription: "Áo thun cao cấp từ designer Việt Nam",
            Tags: new List<string> { "fashion", "vietnamese", "designer" },
            VatRate: 0.1m,
            IsVatIncluded: true,
            TaxCode: "1234567890",
            AdminNotes: null,
            RequestImmediateReview: false
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyOrNullName_ShouldHaveValidationError(string name)
    {
        // Arrange
        var command = CreateValidCommand() with { Name = name };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Tên sản phẩm không được để trống");
    }

    [Fact]
    public void Validate_NameTooShort_ShouldHaveValidationError()
    {
        // Arrange
        var command = CreateValidCommand() with { Name = "AB" }; // 2 characters

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Tên sản phẩm phải từ 3 đến 200 ký tự");
    }

    [Fact]
    public void Validate_NameTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('A', 201); // 201 characters
        var command = CreateValidCommand() with { Name = longName };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Tên sản phẩm phải từ 3 đến 200 ký tự");
    }

    [Fact]
    public void Validate_ShortDescriptionTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var longDescription = new string('A', 501); // 501 characters
        var command = CreateValidCommand() with { ShortDescription = longDescription };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ShortDescription)
            .WithErrorMessage("Mô tả ngắn không được vượt quá 500 ký tự");
    }

    [Fact]
    public void Validate_LongDescriptionTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var veryLongDescription = new string('A', 5001); // 5001 characters
        var command = CreateValidCommand() with { LongDescription = veryLongDescription };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LongDescription)
            .WithErrorMessage("Mô tả chi tiết không được vượt quá 5000 ký tự");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyOrNullSlug_ShouldHaveValidationError(string slug)
    {
        // Arrange
        var command = CreateValidCommand() with { Slug = slug };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Slug)
            .WithErrorMessage("Slug không được để trống");
    }

    [Theory]
    [InlineData("AB")] // Too short
    [InlineData("Invalid Slug")] // Contains spaces
    [InlineData("INVALID-SLUG")] // Contains uppercase
    [InlineData("invalid_slug")] // Contains underscore
    public void Validate_InvalidSlug_ShouldHaveValidationError(string slug)
    {
        // Arrange
        var command = CreateValidCommand() with { Slug = slug };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Theory]
    [InlineData("valid-slug")]
    [InlineData("another-valid-slug-123")]
    [InlineData("product-name-with-numbers-456")]
    public void Validate_ValidSlug_ShouldNotHaveValidationError(string slug)
    {
        // Arrange
        var command = CreateValidCommand() with { Slug = slug };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Slug);
    }

    [Theory]
    [InlineData((byte)0)] // Invalid
    [InlineData((byte)4)] // Invalid
    [InlineData((byte)255)] // Invalid
    public void Validate_InvalidProductType_ShouldHaveValidationError(byte productType)
    {
        // Arrange
        var command = CreateValidCommand() with { ProductType = productType };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductType)
            .WithErrorMessage("Loại sản phẩm không hợp lệ (1: Vật lý, 2: Số, 3: Dịch vụ)");
    }

    [Theory]
    [InlineData((byte)1)] // Physical
    [InlineData((byte)2)] // Digital
    [InlineData((byte)3)] // Service
    public void Validate_ValidProductType_ShouldNotHaveValidationError(byte productType)
    {
        // Arrange
        var command = CreateValidCommand() with { ProductType = productType };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProductType);
    }

    [Theory]
    [InlineData(6)] // Too few days
    [InlineData(31)] // Too many days
    public void Validate_InvalidReturnDaysForPhysicalProduct_ShouldHaveValidationError(int returnDays)
    {
        // Arrange
        var command = CreateValidCommand() with 
        { 
            ProductType = (byte)1, // Physical
            ReturnDays = returnDays 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReturnDays)
            .WithErrorMessage("Số ngày đổi trả phải từ 7 đến 30 ngày");
    }

    [Theory]
    [InlineData(7)]
    [InlineData(15)]
    [InlineData(30)]
    public void Validate_ValidReturnDaysForPhysicalProduct_ShouldNotHaveValidationError(int returnDays)
    {
        // Arrange
        var command = CreateValidCommand() with 
        { 
            ProductType = (byte)1, // Physical
            ReturnDays = returnDays 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ReturnDays);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidVendorId_ShouldHaveValidationError(long vendorId)
    {
        // Arrange
        var command = CreateValidCommand() with { VendorId = vendorId };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VendorId)
            .WithErrorMessage("ID nhà cung cấp phải lớn hơn 0");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("fr")]
    [InlineData("de")]
    public void Validate_InvalidPrimaryLanguage_ShouldHaveValidationError(string language)
    {
        // Arrange
        var command = CreateValidCommand() with { PrimaryLanguage = language };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PrimaryLanguage);
    }

    [Theory]
    [InlineData("vi")]
    [InlineData("en")]
    public void Validate_ValidPrimaryLanguage_ShouldNotHaveValidationError(string language)
    {
        // Arrange
        var command = CreateValidCommand() with { PrimaryLanguage = language };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PrimaryLanguage);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(0.21)]
    [InlineData(1.0)]
    public void Validate_InvalidVatRate_ShouldHaveValidationError(decimal vatRate)
    {
        // Arrange
        var command = CreateValidCommand() with { VatRate = vatRate };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VatRate)
            .WithErrorMessage("Thuế VAT phải từ 0% đến 20%");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.1)]
    [InlineData(0.2)]
    public void Validate_ValidVatRate_ShouldNotHaveValidationError(decimal vatRate)
    {
        // Arrange
        var command = CreateValidCommand() with { VatRate = vatRate };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.VatRate);
    }

    [Fact]
    public void Validate_EmptyPricing_ShouldHaveValidationError()
    {
        // Arrange
        var command = CreateValidCommand() with { Pricing = new List<CreateProductPricingRequest>() };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Pricing)
            .WithErrorMessage("Phải có ít nhất một thông tin giá");
    }

    [Fact]
    public void Validate_PricingWithoutVND_ShouldHaveValidationError()
    {
        // Arrange
        var pricing = new List<CreateProductPricingRequest>
        {
            new("USD", 50m, null, null, null, null, null)
        };
        var command = CreateValidCommand() with { Pricing = pricing };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Pricing)
            .WithErrorMessage("Phải có giá bằng VND cho thị trường Việt Nam");
    }

    [Theory]
    [InlineData("123456789")] // 9 digits
    [InlineData("12345678901")] // 11 digits
    [InlineData("1234567890-12")] // Wrong format
    [InlineData("abc1234567")] // Contains letters
    public void Validate_InvalidTaxCode_ShouldHaveValidationError(string taxCode)
    {
        // Arrange
        var command = CreateValidCommand() with { TaxCode = taxCode };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TaxCode)
            .WithErrorMessage("Mã số thuế phải có định dạng 10 số hoặc 10 số-3 số");
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("1234567890-123")]
    public void Validate_ValidTaxCode_ShouldNotHaveValidationError(string taxCode)
    {
        // Arrange
        var command = CreateValidCommand() with { TaxCode = taxCode };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TaxCode);
    }

    private static CreateProductCommand CreateValidCommand() =>
        new(
            Name: "Valid Product Name",
            ShortDescription: "Valid short description",
            LongDescription: "Valid long description",
            Slug: "valid-product-slug",
            ProductType: (byte)1, // Physical
            ReturnDays: 30,
            CategoryIds: new List<long> { 1L },
            VendorId: 1L,
            PrimaryLanguage: "vi",
            SecondaryLanguageContent: null,
            Pricing: new List<CreateProductPricingRequest>
            {
                new("VND", 100000m, null, null, null, null, null)
            },
            Variants: null,
            MetaTitle: null,
            MetaDescription: null,
            Tags: null,
            VatRate: 0.1m,
            IsVatIncluded: true,
            TaxCode: null,
            AdminNotes: null,
            RequestImmediateReview: false
        );
}