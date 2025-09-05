using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class OrdOrderAddress
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public byte AddrType { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Line1 { get; set; } = null!;

    public string? Line2 { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? Province { get; set; }

    public string Country { get; set; } = null!;

    public string? PostalCode { get; set; }

    public virtual OrdOrder Order { get; set; } = null!;
}
