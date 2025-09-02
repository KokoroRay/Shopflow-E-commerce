// (N?u file ?ã t?n t?i thì gi?; n?u ch?a có thì dùng b?n này có thêm cart nav)
namespace ShopFlow.Domain.Entities;
public class sys_webhook_event : BaseEntity
{
    public long? product_id { get; set; }
    public cat_product? product { get; set; }
    public long? cart_id { get; set; }
    public cart? cart { get; set; }
}