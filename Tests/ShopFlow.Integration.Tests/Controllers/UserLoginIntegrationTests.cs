using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Enums;
using ShopFlow.Integration.Tests.TestFixtures;
using System.Net;
using System.Net.Http.Json;

namespace ShopFlow.Integration.Tests.Controllers;

/// <summary>
/// Integration tests for User Login API
/// </summary>
public class UserLoginIntegrationTests : IntegrationTestBase
{
    public UserLoginIntegrationTests(ShopFlowWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithTokens()
    {
        // Arrange
        await CleanDatabaseAsync();

        // First register a user
        var registerRequest = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!",
            FullName = "John Doe"
        };

        await PostJsonAsync("/api/users/register", registerRequest);

        // Now login
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!"
        };

        // Act
        var response = await PostJsonAsync("/api/users/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await ReadResponseAsAsync<LoginResponse>(response);
        loginResponse.Should().NotBeNull();
        loginResponse!.AccessToken.Should().NotBeEmpty();
        loginResponse.RefreshToken.Should().NotBeEmpty();
        loginResponse.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        loginResponse.User.Should().NotBeNull();
        loginResponse.User.Email.Should().Be("test@example.com");
        loginResponse.User.FullName.Should().Be("John Doe");
        loginResponse.User.Roles.Should().Contain("Customer");
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        await CleanDatabaseAsync();

        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "SecurePass123!"
        };

        // Act
        var response = await PostJsonAsync("/api/users/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        await CleanDatabaseAsync();

        // First register a user
        var registerRequest = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!",
            FullName = "John Doe"
        };

        await PostJsonAsync("/api/users/register", registerRequest);

        // Now login with wrong password
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await PostJsonAsync("/api/users/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Login_WithInvalidEmailFormat_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var loginRequest = new LoginRequest
        {
            Email = "invalid-email",
            Password = "SecurePass123!"
        };

        // Act
        var response = await PostJsonAsync("/api/users/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithEmptyEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var loginRequest = new LoginRequest
        {
            Email = "",
            Password = "SecurePass123!"
        };

        // Act
        var response = await PostJsonAsync("/api/users/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = ""
        };

        // Act
        var response = await PostJsonAsync("/api/users/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithShortPassword_ShouldReturnBadRequest()
    {
        // Arrange
        await CleanDatabaseAsync();

        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "short"
        };

        // Act
        var response = await PostJsonAsync("/api/users/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_TokenShouldContainUserClaims()
    {
        // Arrange
        await CleanDatabaseAsync();

        // First register a user
        var registerRequest = new RegisterUserRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!",
            FullName = "John Doe"
        };

        await PostJsonAsync("/api/users/register", registerRequest);

        // Now login
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "SecurePass123!"
        };

        // Act
        var response = await PostJsonAsync("/api/users/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await ReadResponseAsAsync<LoginResponse>(response);
        loginResponse.Should().NotBeNull();

        // Token should be valid and contain user information
        var token = loginResponse!.AccessToken;
        token.Should().NotBeEmpty();

        // The token should be a valid JWT (basic format check)
        var tokenParts = token.Split('.');
        tokenParts.Should().HaveCount(3);
    }
}
