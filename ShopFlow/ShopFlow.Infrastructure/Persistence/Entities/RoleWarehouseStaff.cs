using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[PrimaryKey("WarehouseId", "UserId")]
[Table("role_warehouse_staff")]
public partial class RoleWarehouseStaff
{
    [Key]
    [Column("warehouse_id")]
    public long WarehouseId { get; set; }

    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("shift")]
    [StringLength(50)]
    public string? Shift { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId, RoleId")]
    [InverseProperty("RoleWarehouseStaffs")]
    public virtual CoreUserRole CoreUserRole { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("RoleWarehouseStaffs")]
    public virtual CoreUser User { get; set; } = null!;

    [ForeignKey("WarehouseId")]
    [InverseProperty("RoleWarehouseStaffs")]
    public virtual InvWarehouse Warehouse { get; set; } = null!;
}
