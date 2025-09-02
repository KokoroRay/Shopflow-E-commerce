using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class inv_warehouse
{
    public long id { get; set; }

    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public string? address_json { get; set; }

    public bool is_active { get; set; }

    public virtual ICollection<cart> carts { get; set; } = new List<cart>();

    public virtual ICollection<inv_adjustment> inv_adjustments { get; set; } = new List<inv_adjustment>();

    public virtual ICollection<inv_reservation> inv_reservations { get; set; } = new List<inv_reservation>();

    public virtual ICollection<inv_stock> inv_stocks { get; set; } = new List<inv_stock>();

    public virtual ICollection<ord_order> ord_orders { get; set; } = new List<ord_order>();

    public virtual ICollection<ord_shipment> ord_shipments { get; set; } = new List<ord_shipment>();

    public virtual ICollection<role_warehouse_staff> role_warehouse_staffs { get; set; } = new List<role_warehouse_staff>();

    public virtual ship_config? ship_config { get; set; }
}
