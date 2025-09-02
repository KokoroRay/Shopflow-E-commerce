using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ce_review
{
    public long id { get; set; }

    public long? order_id { get; set; }

    public long? user_id { get; set; }

    public long product_id { get; set; }

    public long? sku_id { get; set; }

    public long vendor_id { get; set; }

    public byte rating { get; set; }

    public string? content { get; set; }

    public string? images_json { get; set; }

    public byte status { get; set; }

    public long? moderated_by { get; set; }

    public DateTime? moderated_at { get; set; }

    public DateTime created_at { get; set; }

    public virtual core_user? moderated_byNavigation { get; set; }

    public virtual ord_order? order { get; set; }

    public virtual cat_product product { get; set; } = null!;

    public virtual cat_sku? sku { get; set; }

    public virtual core_user? user { get; set; }

    public virtual mkt_vendor vendor { get; set; } = null!;
}
