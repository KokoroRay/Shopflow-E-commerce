// (N?u file ?� t?n t?i th� gi?; n?u ch?a c� th� d�ng b?n n�y c� th�m cart nav)
namespace ShopFlow.Domain.Entities;
public class sys_webhook_event : BaseEntity
{
    public long? product_id { get; set; }
    public cat_product? product { get; set; }
    public long? cart_id { get; set; }
    public cart? cart { get; set; }
}