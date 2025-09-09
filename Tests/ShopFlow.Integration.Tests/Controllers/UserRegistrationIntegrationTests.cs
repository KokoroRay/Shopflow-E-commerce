using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Enums;
using ShopFlow.Integration.Tests.TestFixtures;
using System.Net;
using System.Net.Http.Json;

namespace ShopFlow.Integration.Tests.Controllers;

/// <summary>
/// Integration tests for User Registration API
/// </summary>
public class UserRegistrationIntegrationTests : IntegrationTestBase
{
    public UserRegistrationIntegrationTests(ShopFlowWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!",
            FullName = "John Doe",
            Phone = "+84987654321",
            Gender = "male",
            DateOfBirth = new DateTime(1990, 1, 15, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var response = await PostJsonAsync("/api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var userResponse = await ReadResponseAsAsync<UserResponse>(response);
        userResponse.Should().NotBeNull();
        userResponse!.Email.Should().Be("test@example.com");
        userResponse.FullName.Should().Be("John Doe");
        userResponse.Phone.Should().Be("987654321"); // Normalized
        userResponse.Gender.Should().Be("male");
        userResponse.DateOfBirth.Should().Be(new DateTime(1990, 1, 15, 0, 0, 0, DateTimeKind.Utc));
        userResponse.Status.Should().Be((byte)UserStatus.Active);
        userResponse.EmailVerified.Should().BeFalse();
        userResponse.Id.Should().BeGreaterThan(0);

        // Verify user was saved to database
        var userInDb = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Email.Value == "john.doe@example.com");

        userInDb.Should().NotBeNull();
        userInDb!.Email.Value.Should().Be("john.doe@example.com");
        userInDb.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public async Task RegisterUser_WithMinimalData_ShouldReturnCreated()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request = new RegisterUserRequest
        {
            Email = "minimal@example.com",
            Password = "SecurePass123!"
        };

        // Act
        var response = await PostJsonAsync("/api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var userResponse = await ReadResponseAsAsync<UserResponse>(response);
        userResponse.Should().NotBeNull();
        userResponse!.Email.Should().Be("minimal@example.com");
        userResponse.FullName.Should().BeNull();
        userResponse.Phone.Should().BeNull();
        userResponse.Gender.Should().BeNull();
        userResponse.DateOfBirth.Should().BeNull();
        userResponse.Status.Should().Be((byte)UserStatus.Active);
        userResponse.EmailVerified.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterUser_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request = new RegisterUserRequest
        {
            Email = "invalid-email",
            Password = "SecurePass123!"
        };

        // Act
        var response = await PostJsonAsync("/api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithWeakPassword_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "weak"
        };

        // Act
        var response = await PostJsonAsync("/api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithDuplicateEmail_ShouldReturnConflict()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request1 = new RegisterUserRequest
        {
            Email = "duplicate@example.com",
            Password = "SecurePass123!"
        };

        var request2 = new RegisterUserRequest
        {
            Email = "duplicate@example.com",
            Password = "AnotherPass123!"
        };

        // Act
        var response1 = await PostJsonAsync("/api/users/register", request1);
        var response2 = await PostJsonAsync("/api/users/register", request2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task RegisterUser_WithInvalidPhoneNumber_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!",
            Phone = "invalid-phone"
        };

        // Act
        var response = await PostJsonAsync("/api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithInvalidGender_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!",
            Gender = "invalid-gender"
        };

        // Act
        var response = await PostJsonAsync("/api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithFutureDateOfBirth_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!",
            DateOfBirth = DateTime.UtcNow.AddYears(1) // Future date
        };

        // Act
        var response = await PostJsonAsync("/api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithTooYoungAge_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!",
            DateOfBirth = DateTime.UtcNow.AddYears(-10) // 10 years old
        };

        // Act
        var response = await PostJsonAsync("/api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_PasswordShouldBeHashed_DatabaseCheck()
    {
        // Arrange
        await CleanDatabaseAsync();

        var request = new RegisterUserRequest
        {
            Email = "hash-test@example.com",
            Password = "MySecretPassword123!"
        };

        // Act
        var response = await PostJsonAsync("/api/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify password is hashed in database
        var userInDb = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Email.Value == "hash-test@example.com");

        userInDb.Should().NotBeNull();
        userInDb!.PasswordHash.Should().NotBe("MySecretPassword123!");
        userInDb.PasswordHash.Should().StartWith("$2a$"); // BCrypt hash format
        userInDb.PasswordHash.Length.Should().Be(60); // BCrypt hash length
    }
}
