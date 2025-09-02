using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class sys_webhook_event
{
    public long id { get; set; }

    public string topic { get; set; } = null!;

    public string payload_json { get; set; } = null!;

    public int attempts { get; set; }

    public byte status { get; set; }

    public DateTime? last_attempt_at { get; set; }

    public DateTime created_at { get; set; }

    public long? vendor_id { get; set; }

    public long? integration_id { get; set; }

    public long? order_id { get; set; }

    public long? cart_id { get; set; }

    public long? product_id { get; set; }

    public virtual cart? cart { get; set; }

    public virtual sys_integration? integration { get; set; }

    public virtual ord_order? order { get; set; }

    public virtual cat_product? product { get; set; }

    public virtual mkt_vendor? vendor { get; set; }
}
