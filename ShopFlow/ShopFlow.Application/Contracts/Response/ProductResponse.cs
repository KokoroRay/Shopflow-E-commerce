namespace ShopFlow.Application.Contracts.Response;

public class ProductResponse
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ShortDescription { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string Slug { get; set; } = string.Empty;

    public byte ProductType { get; set; }

    public byte Status { get; set; }

    public int? ReturnDays { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Language { get; set; } = string.Empty;

    public List<CategoryResponse> Categories { get; set; } = new();

    public List<SkuResponse> Skus { get; set; } = new();
}

public class SkuResponse
{
    public long Id { get; set; }

    public string SkuCode { get; set; } = string.Empty;

    public string? Barcode { get; set; }

    public bool IsActive { get; set; }

    public string? OptionsJson { get; set; }
}
