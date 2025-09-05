using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class ShipZone
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string? RegionPattern { get; set; }

    public string? DistrictPattern { get; set; }

    public virtual ICollection<ShipRate> ShipRates { get; set; } = new List<ShipRate>();
}
