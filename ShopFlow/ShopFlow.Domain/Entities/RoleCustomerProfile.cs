using System;
using System.Collections.Generic;

namespace ShopFlow.Domain.Entities;

public partial class RoleCustomerProfile
{
    public long UserId { get; set; }

    public int RoleId { get; set; }

    public string? FullName { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Gender { get; set; }

    public int LoyaltyPoints { get; set; }

    public bool MarketingOptin { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CoreUserRole CoreUserRole { get; set; } = null!;

    public virtual CoreUser User { get; set; } = null!;
}
