using AutoFixture;
using FluentAssertions;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.DomainEvents;
using ShopFlow.Domain.Tests.TestFixtures;
using Xunit;

namespace ShopFlow.Domain.Tests.Entities;

/// <summary>
/// Comprehensive tests for CoreUser entity business logic and domain behavior
/// </summary>
public class CoreUserTests : DomainTestBase
{
    private readonly Email _validEmail;
    private readonly PhoneNumber _validPhone;
    private readonly string _validPasswordHash;

    public CoreUserTests()
    {
        _validEmail = new Email("test@example.com");
        _validPhone = new PhoneNumber("0901234567");
        _validPasswordHash = "hashed_password_123";
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ValidParameters_ShouldCreateUser()
    {
        // Act
        var user = new CoreUser(_validEmail, _validPasswordHash, _validPhone);

        // Assert
        user.Email.Should().Be(_validEmail);
        user.PasswordHash.Should().Be(_validPasswordHash);
        user.Phone.Should().Be(_validPhone);
        user.Status.Should().Be(UserStatus.Active);
        user.EmailVerified.Should().BeFalse();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.DomainEvents.Should().HaveCount(1);
        user.DomainEvents.First().Should().BeOfType<UserCreatedEvent>();
    }

    [Fact]
    public void Constructor_WithoutPhone_ShouldCreateUser()
    {
        // Act
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Assert
        user.Email.Should().Be(_validEmail);
        user.PasswordHash.Should().Be(_validPasswordHash);
        user.Phone.Should().BeNull();
        user.Status.Should().Be(UserStatus.Active);
        user.EmailVerified.Should().BeFalse();
    }

    [Fact]
    public void Constructor_NullEmail_ShouldThrowException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CoreUser(null!, _validPasswordHash, _validPhone));

        exception.ParamName.Should().Be("email");
    }

    [Fact]
    public void Constructor_NullPasswordHash_ShouldThrowException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CoreUser(_validEmail, null!, _validPhone));

        exception.ParamName.Should().Be("passwordHash");
    }

    [Fact]
    public void Constructor_EmptyPasswordHash_ShouldAcceptEmptyString()
    {
        // Arrange & Act
        var user = new CoreUser(_validEmail, "", _validPhone);

        // Assert - Constructor allows empty string, but it's business logic choice
        user.Should().NotBeNull();
        user.PasswordHash.Should().Be("");
    }

    #endregion

    #region Email Management Tests

    [Fact]
    public void UpdateEmail_ValidEmail_ShouldUpdateAndResetVerification()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        user.VerifyEmail(); // First verify email
        var newEmail = new Email("newemail@example.com");
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.UpdateEmail(newEmail);

        // Assert
        user.Email.Should().Be(newEmail);
        user.EmailVerified.Should().BeFalse();
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdateEmail_NullEmail_ShouldThrowException()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => user.UpdateEmail(null!));
        exception.ParamName.Should().Be("newEmail");
    }

    [Fact]
    public void VerifyEmail_ShouldSetEmailVerifiedToTrue()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.VerifyEmail();

        // Assert
        user.EmailVerified.Should().BeTrue();
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    #endregion

    #region Phone Management Tests

    [Fact]
    public void UpdatePhone_ValidPhone_ShouldUpdatePhone()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var newPhone = new PhoneNumber("0987654321"); // Sử dụng valid Vietnamese format
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.UpdatePhone(newPhone);

        // Assert
        user.Phone.Should().Be(newPhone);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdatePhone_NullPhone_ShouldSetPhoneToNull()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash, _validPhone);
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.UpdatePhone(null);

        // Assert
        user.Phone.Should().BeNull();
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    #endregion

    #region Password Management Tests

    [Fact]
    public void ChangePassword_ValidPasswordHash_ShouldUpdatePassword()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var newPasswordHash = "new_hashed_password_456";
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.ChangePassword(newPasswordHash);

        // Assert
        user.PasswordHash.Should().Be(newPasswordHash);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ChangePassword_InvalidPasswordHash_ShouldThrowException(string invalidPasswordHash)
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => user.ChangePassword(invalidPasswordHash));
        exception.ParamName.Should().Be("newPasswordHash");
        exception.Message.Should().Contain("Password hash cannot be empty");
    }

    #endregion

    #region Status Management Tests

    [Fact]
    public void Suspend_ActiveUser_ShouldSuspendUser()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Suspend();

        // Assert
        user.Status.Should().Be(UserStatus.Suspended);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Suspend_BannedUser_ShouldThrowException()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        user.Ban();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => user.Suspend());
        exception.Message.Should().Contain("Cannot suspend a banned user");
    }

    [Fact]
    public void Activate_SuspendedUser_ShouldActivateUser()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        user.Suspend();
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Activate();

        // Assert
        user.Status.Should().Be(UserStatus.Active);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Activate_InactiveUser_ShouldActivateUser()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        // Manually set status to Inactive using reflection for testing
        var statusProperty = typeof(CoreUser).GetProperty(nameof(CoreUser.Status));
        statusProperty!.SetValue(user, UserStatus.Inactive);
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Activate();

        // Assert
        user.Status.Should().Be(UserStatus.Active);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Activate_BannedUser_ShouldThrowException()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        user.Ban();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => user.Activate());
        exception.Message.Should().Contain("Cannot activate a banned user");
    }

    [Fact]
    public void Ban_AnyUser_ShouldBanUser()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Ban();

        // Assert
        user.Status.Should().Be(UserStatus.Banned);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void Ban_SuspendedUser_ShouldBanUser()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        user.Suspend();
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.Ban();

        // Assert
        user.Status.Should().Be(UserStatus.Banned);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void IsActive_ActiveUser_ShouldReturnTrue()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        user.IsActive().Should().BeTrue();
    }

    [Theory]
    [InlineData(UserStatus.Inactive)]
    [InlineData(UserStatus.Suspended)]
    [InlineData(UserStatus.Banned)]
    public void IsActive_NonActiveUser_ShouldReturnFalse(UserStatus status)
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var statusProperty = typeof(CoreUser).GetProperty(nameof(CoreUser.Status));
        statusProperty!.SetValue(user, status);

        // Act & Assert
        user.IsActive().Should().BeFalse();
    }

    #endregion

    #region Address Management Tests

    [Fact]
    public void AddAddress_ValidAddress_ShouldAddAddress()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var address = Fixture.Create<CoreAddress>();
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.AddAddress(address);

        // Assert
        user.Addresses.Should().Contain(address);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void AddAddress_NullAddress_ShouldThrowException()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => user.AddAddress(null!));
        exception.ParamName.Should().Be("address");
    }

    [Fact]
    public void AddAddress_MultipleAddresses_ShouldAddAllAddresses()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var address1 = Fixture.Create<CoreAddress>();
        var address2 = Fixture.Create<CoreAddress>();

        // Act
        user.AddAddress(address1);
        user.AddAddress(address2);

        // Assert
        user.Addresses.Should().HaveCount(2);
        user.Addresses.Should().Contain(address1);
        user.Addresses.Should().Contain(address2);
    }

    #endregion

    #region Role Management Tests

    [Fact]
    public void AddRole_ValidUserRole_ShouldAddRole()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var userRole = Fixture.Create<CoreUserRole>();
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        user.AddRole(userRole);

        // Assert
        user.UserRoles.Should().Contain(userRole);
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void AddRole_NullUserRole_ShouldThrowException()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => user.AddRole(null!));
        exception.ParamName.Should().Be("userRole");
    }

    [Fact]
    public void HasRole_ExistingRole_ShouldReturnTrue()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var role = new CoreRole { Id = 1, Code = "ADMIN", Name = "Administrator" };
        var userRole = new CoreUserRole { UserId = user.Id, RoleId = role.Id, Role = role };
        user.AddRole(userRole);

        // Act & Assert
        user.HasRole("ADMIN").Should().BeTrue();
    }

    [Fact]
    public void HasRole_NonExistingRole_ShouldReturnFalse()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);
        var role = new CoreRole { Id = 1, Code = "USER", Name = "User" };
        var userRole = new CoreUserRole { UserId = user.Id, RoleId = role.Id, Role = role };
        user.AddRole(userRole);

        // Act & Assert
        user.HasRole("ADMIN").Should().BeFalse();
    }

    [Fact]
    public void HasRole_EmptyRoles_ShouldReturnFalse()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        user.HasRole("ADMIN").Should().BeFalse();
    }

    [Fact]
    public void HasRole_MultipleRoles_ShouldReturnCorrectResult()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        var adminRole = new CoreRole { Id = 1, Code = "ADMIN", Name = "Administrator" };
        var userRole = new CoreRole { Id = 2, Code = "USER", Name = "User" };

        var adminUserRole = new CoreUserRole { UserId = user.Id, RoleId = adminRole.Id, Role = adminRole };
        var normalUserRole = new CoreUserRole { UserId = user.Id, RoleId = userRole.Id, Role = userRole };

        user.AddRole(adminUserRole);
        user.AddRole(normalUserRole);

        // Act & Assert
        user.HasRole("ADMIN").Should().BeTrue();
        user.HasRole("USER").Should().BeTrue();
        user.HasRole("MODERATOR").Should().BeFalse();
    }

    #endregion

    #region Domain Events Tests

    [Fact]
    public void Constructor_ShouldRaiseUserCreatedEvent()
    {
        // Act
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Assert
        user.DomainEvents.Should().HaveCount(1);
        var domainEvent = user.DomainEvents.First();
        domainEvent.Should().BeOfType<UserCreatedEvent>();

        var userCreatedEvent = (UserCreatedEvent)domainEvent;
        userCreatedEvent.UserId.Should().Be(user.Id);
        userCreatedEvent.Email.Should().Be(_validEmail.Value);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act
        user.ClearDomainEvents();

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region Navigation Properties Tests

    [Fact]
    public void Addresses_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        user.Addresses.Should().BeAssignableTo<IReadOnlyCollection<CoreAddress>>();
        user.Addresses.Should().BeEmpty();
    }

    [Fact]
    public void UserRoles_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        user.UserRoles.Should().BeAssignableTo<IReadOnlyCollection<CoreUserRole>>();
        user.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void Carts_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        user.Carts.Should().BeAssignableTo<IReadOnlyCollection<Cart>>();
        user.Carts.Should().BeEmpty();
    }

    [Fact]
    public void Reviews_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        user.Reviews.Should().BeAssignableTo<IReadOnlyCollection<CeReview>>();
        user.Reviews.Should().BeEmpty();
    }

    [Fact]
    public void Orders_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var user = new CoreUser(_validEmail, _validPasswordHash);

        // Act & Assert
        user.Orders.Should().BeAssignableTo<IReadOnlyCollection<OrdOrder>>();
        user.Orders.Should().BeEmpty();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void UserLifecycle_CompleteFlow_ShouldWorkCorrectly()
    {
        // Arrange & Act - Create user
        var user = new CoreUser(_validEmail, _validPasswordHash, _validPhone);
        var originalCreatedAt = user.CreatedAt;

        // Act - Update email
        var newEmail = new Email("updated@example.com");
        user.UpdateEmail(newEmail);

        // Act - Verify email
        user.VerifyEmail();

        // Act - Change password
        var newPasswordHash = "new_hashed_password";
        user.ChangePassword(newPasswordHash);

        // Act - Suspend and then activate
        user.Suspend();
        user.Activate();

        // Act - Add address and role
        var address = Fixture.Create<CoreAddress>();
        user.AddAddress(address);

        var role = new CoreRole { Id = 1, Code = "CUSTOMER", Name = "Customer" };
        var userRole = new CoreUserRole { UserId = user.Id, RoleId = role.Id, Role = role };
        user.AddRole(userRole);

        // Assert
        user.Email.Should().Be(newEmail);
        user.EmailVerified.Should().BeTrue();
        user.PasswordHash.Should().Be(newPasswordHash);
        user.Status.Should().Be(UserStatus.Active);
        user.Phone.Should().Be(_validPhone);
        user.Addresses.Should().Contain(address);
        user.HasRole("CUSTOMER").Should().BeTrue();
        user.IsActive().Should().BeTrue();
        user.CreatedAt.Should().Be(originalCreatedAt);
        user.UpdatedAt.Should().BeAfter(originalCreatedAt);
    }

    #endregion
}
