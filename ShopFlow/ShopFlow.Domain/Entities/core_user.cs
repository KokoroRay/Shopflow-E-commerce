namespace ShopFlow.Domain.Entities;

public class core_user : BaseEntity
{
    public string? email { get; set; }
    public string? phone { get; set; }
    public string? password_hash { get; set; }
    public byte status { get; set; }
    public bool email_verified { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }

    // minimal navigations referenced by DbContext
    public ICollection<cart> carts { get; set; } = new List<cart>();
    // omit explicit ce_review/core_address/inv_adjustment types to decouple domain from infra EF models
    public ICollection<object> ce_reviewmoderated_byNavigations { get; set; } = new List<object>();
    public ICollection<object> ce_reviewusers { get; set; } = new List<object>();
    public ICollection<object> core_addresses { get; set; } = new List<object>();
    public ICollection<object> inv_adjustments { get; set; } = new List<object>();
    public ICollection<ord_order> ord_orders { get; set; } = new List<ord_order>();
    public object? role_admin_profile { get; set; }
    public object? role_moderator_profile { get; set; }
    public object? role_customer_profile { get; set; }
    public ICollection<object> role_vendor_staffs { get; set; } = new List<object>();
    public ICollection<object> role_warehouse_staffs { get; set; } = new List<object>();
    public ICollection<core_user_role> core_user_roles { get; set; } = new List<core_user_role>();
}