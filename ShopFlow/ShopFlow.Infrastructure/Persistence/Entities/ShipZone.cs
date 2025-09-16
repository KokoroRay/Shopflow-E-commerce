using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("ship_zone")]
[Index("Country", Name = "IX_ship_zone_country")]
[Index("Code", Name = "UQ__ship_zon__357D4CF956C7AC59", IsUnique = true)]
public partial class ShipZone
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("country")]
    [StringLength(2)]
    [Unicode(false)]
    public string Country { get; set; } = null!;

    [Column("region_pattern")]
    [StringLength(255)]
    public string? RegionPattern { get; set; }

    [Column("district_pattern")]
    [StringLength(255)]
    public string? DistrictPattern { get; set; }

    [InverseProperty("Zone")]
    public virtual ICollection<ShipRate> ShipRates { get; set; } = new List<ShipRate>();
}
