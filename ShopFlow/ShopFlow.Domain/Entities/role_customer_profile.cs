namespace ShopFlow.Domain.Entities;

public class role_customer_profile : BaseEntity
{
    public long user_id { get; set; }
    public string? full_name { get; set; }
    public string? gender { get; set; }
    public DateTime created_at { get; set; }
}