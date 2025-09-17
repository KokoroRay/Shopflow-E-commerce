using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("ship_config")]
[Index("WarehouseId", Name = "UQ__ship_con__734FE6BEBD30A8D8", IsUnique = true)]
public partial class ShipConfig
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("warehouse_id")]
    public long WarehouseId { get; set; }

    [Column("lead_time_days")]
    public int LeadTimeDays { get; set; }

    [Column("enabled")]
    public bool Enabled { get; set; }

    [ForeignKey("WarehouseId")]
    [InverseProperty("ShipConfig")]
    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
