using System;
using System.Collections.Generic;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.DomainEvents;

namespace ShopFlow.Domain.Entities;

public class CoreUser : BaseEntity
{
    private readonly List<Cart> _carts = new();
    private readonly List<CeReview> _reviews = new();
    private readonly List<CoreAddress> _addresses = new();
    private readonly List<CoreUserRole> _userRoles = new();
    private readonly List<OrdOrder> _orders = new();

    public Email Email { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public string PasswordHash { get; private set; }
    public UserStatus Status { get; private set; }
    public bool EmailVerified { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public IReadOnlyCollection<Cart> Carts => _carts.AsReadOnly();
    public IReadOnlyCollection<CeReview> Reviews => _reviews.AsReadOnly();
    public IReadOnlyCollection<CoreAddress> Addresses => _addresses.AsReadOnly();
    public IReadOnlyCollection<CoreUserRole> UserRoles => _userRoles.AsReadOnly();
    public IReadOnlyCollection<OrdOrder> Orders => _orders.AsReadOnly();

    // Profile navigation properties
    public RoleAdminProfile? AdminProfile { get; private set; }
    public RoleCustomerProfile? CustomerProfile { get; private set; }
    public RoleModeratorProfile? ModeratorProfile { get; private set; }

    // Private constructor for EF
    private CoreUser() { }

    public CoreUser(Email email, string passwordHash, PhoneNumber? phone = null)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        Phone = phone;
        Status = UserStatus.Active;
        EmailVerified = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new UserCreatedEvent(Id, Email.Value));
    }

    public void UpdateEmail(Email newEmail)
    {
        if (newEmail == null) throw new ArgumentNullException(nameof(newEmail));
        
        Email = newEmail;
        EmailVerified = false; // Reset verification when email changes
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePhone(PhoneNumber? newPhone)
    {
        Phone = newPhone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));
            
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        EmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        if (Status == UserStatus.Banned)
            throw new InvalidOperationException("Cannot suspend a banned user");
            
        Status = UserStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == UserStatus.Banned)
            throw new InvalidOperationException("Cannot activate a banned user");
            
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Ban()
    {
        Status = UserStatus.Banned;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAddress(CoreAddress address)
    {
        if (address == null) throw new ArgumentNullException(nameof(address));
        
        _addresses.Add(address);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRole(CoreUserRole userRole)
    {
        if (userRole == null) throw new ArgumentNullException(nameof(userRole));
        
        _userRoles.Add(userRole);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasRole(string roleCode)
    {
        return _userRoles.Any(ur => ur.Role.Code == roleCode);
    }

    public bool IsActive()
    {
        return Status == UserStatus.Active;
    }
}
