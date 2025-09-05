using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class InvWarehouse
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? AddressJson { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<InvAdjustment> InvAdjustments { get; set; } = new List<InvAdjustment>();

    public virtual ICollection<InvReservation> InvReservations { get; set; } = new List<InvReservation>();

    public virtual ICollection<InvStock> InvStocks { get; set; } = new List<InvStock>();

    public virtual ICollection<OrdOrder> OrdOrders { get; set; } = new List<OrdOrder>();

    public virtual ICollection<OrdShipment> OrdShipments { get; set; } = new List<OrdShipment>();

    public virtual ICollection<RoleWarehouseStaff> RoleWarehouseStaffs { get; set; } = new List<RoleWarehouseStaff>();

    public virtual ShipConfig? ShipConfig { get; set; }
}
