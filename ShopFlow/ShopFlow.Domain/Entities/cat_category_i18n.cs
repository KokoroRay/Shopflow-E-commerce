namespace ShopFlow.Domain.Entities;

public class cat_category_i18n : BaseEntity
{
    public long category_id { get; set; }
    public string? lang { get; set; }
    public string? name { get; set; }
    public string? meta_desc { get; set; }
    public string? meta_title { get; set; }
    public string? slug { get; set; }

    public cat_category? category { get; set; }
}