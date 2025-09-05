using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class CoreAddress
{
    public long Id { get; set; }

    public long UserId { get; set; }

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

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual CoreUser User { get; set; } = null!;
}
