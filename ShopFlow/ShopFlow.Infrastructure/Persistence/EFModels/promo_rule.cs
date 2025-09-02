using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class promo_rule
{
    public long id { get; set; }

    public long campaign_id { get; set; }

    public string condition_json { get; set; } = null!;

    public string action_json { get; set; } = null!;

    public bool stop_further_rules { get; set; }

    public int sort { get; set; }

    public byte status { get; set; }

    public virtual promo_campaign campaign { get; set; } = null!;

    public virtual ICollection<cart_applied_promo> cart_applied_promos { get; set; } = new List<cart_applied_promo>();

    public virtual ICollection<promo_application_log> promo_application_logs { get; set; } = new List<promo_application_log>();
}
