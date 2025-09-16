using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("inv_warehouse")]
[Index("Code", Name = "UQ__inv_ware__357D4CF95C6B894F", IsUnique = true)]
public partial class InvWarehouse
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

    [Column("address_json")]
    public string? AddressJson { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [InverseProperty("Warehouse")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [InverseProperty("Warehouse")]
    public virtual ICollection<InvAdjustment> InvAdjustments { get; set; } = new List<InvAdjustment>();

    [InverseProperty("Warehouse")]
    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    [InverseProperty("Warehouse")]
    public virtual ICollection<InvStockChangeLog> InvStockChangeLogs { get; set; } = new List<InvStockChangeLog>();

    [InverseProperty("Warehouse")]
    public virtual ICollection<InvStock> InvStocks { get; set; } = new List<InvStock>();

    [InverseProperty("Warehouse")]
    public virtual ICollection<OrdOrder> OrdOrders { get; set; } = new List<OrdOrder>();

    [InverseProperty("Warehouse")]
    public virtual ICollection<OrdShipment> OrdShipments { get; set; } = new List<OrdShipment>();

    [InverseProperty("Warehouse")]
    public virtual ICollection<RoleWarehouseStaff> RoleWarehouseStaffs { get; set; } = new List<RoleWarehouseStaff>();

    [InverseProperty("Warehouse")]
    public virtual ShipConfig? ShipConfig { get; set; }
}
