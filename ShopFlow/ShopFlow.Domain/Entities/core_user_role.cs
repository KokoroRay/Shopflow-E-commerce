namespace ShopFlow.Domain.Entities;

public class core_user_role : BaseEntity
{
    public long user_id { get; set; }
    public long role_id { get; set; }

    // minimal navigations referenced by DbContext
    public core_user? user { get; set; }
    public core_role? role { get; set; }
    public ICollection<object> role_admin_profiles { get; set; } = new List<object>();
    public ICollection<object> role_customer_profiles { get; set; } = new List<object>();
    public ICollection<object> role_moderator_profiles { get; set; } = new List<object>();
    public ICollection<object> role_vendor_staffs { get; set; } = new List<object>();
    public ICollection<object> role_warehouse_staffs { get; set; } = new List<object>();
}