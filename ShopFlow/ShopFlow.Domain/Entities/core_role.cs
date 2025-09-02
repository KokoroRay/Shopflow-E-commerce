namespace ShopFlow.Domain.Entities;

public class core_role : BaseEntity
{
    public string? code { get; set; }
    public string? name { get; set; }

    // minimal navigations referenced by DbContext
    public ICollection<object> permissions { get; set; } = new List<object>();
    public ICollection<core_user_role> core_user_roles { get; set; } = new List<core_user_role>();
}