namespace ShopFlow.Domain.Entities;

public class cat_product_i18n : BaseEntity
{
    public long product_id { get; set; }
    public string? lang { get; set; }
    public string? name { get; set; }
    public string? short_desc { get; set; }
    public string? slug { get; set; }
    public string? meta_title { get; set; }
    public string? meta_desc { get; set; }

    // optional back-reference used in DbContext mapping
    public cat_product? product { get; set; }
}