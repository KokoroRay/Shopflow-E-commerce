namespace ShopFlow.Domain.Entities;

public class cat_category : BaseEntity
{
    public byte? is_active { get; set; }

    // minimal navigations referenced by DbContext
    public long? parent_id { get; set; }
    public cat_category? parent { get; set; }
    public ICollection<cat_category> Inverseparent { get; set; } = new List<cat_category>();
}