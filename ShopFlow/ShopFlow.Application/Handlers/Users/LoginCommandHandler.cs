using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Exceptions;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Handlers.Users;

/// <summary>
/// Handler for LoginCommand
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the LoginCommandHandler class
    /// </summary>
    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the LoginCommand
    /// </summary>
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("Processing login request for email: {Email}", request.Email);

        try
        {
            // Validate email format and create Email value object
            var email = new Email(request.Email);

            // Find user by email
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email: {Email}", request.Email);
                throw new AuthenticationException("Invalid email or password");
            }

            // Verify password
            var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed: Invalid password for user: {UserId}", user.Id);
                throw new AuthenticationException("Invalid email or password");
            }

            // Check if user is active
            if (user.Status != Domain.Enums.UserStatus.Active)
            {
                _logger.LogWarning("Login failed: User account is not active for user: {UserId}", user.Id);
                throw new AuthenticationException("User account is not active");
            }

            // Get user roles (simplified - assuming Customer role for now)
            var roles = new List<string> { "Customer" };

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email.Value, roles);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var tokenExpiration = _jwtTokenService.GetTokenExpiration(accessToken);

            _logger.LogInformation("Login successful for user: {UserId}", user.Id);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = tokenExpiration,
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    FullName = GetUserFullName(user),
                    Roles = roles
                }
            };
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Login failed: Invalid email format: {Email}, Error: {Error}", request.Email, ex.Message);
            throw new AuthenticationException("Invalid email format");
        }
        catch (AuthenticationException)
        {
            throw; // Re-throw authentication exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for email: {Email}", request.Email);
            throw new DomainException("An error occurred during login. Please try again.");
        }
    }

    private static string? GetUserFullName(Domain.Entities.CoreUser user)
    {
        // Check if user has customer profile with full name
        if (user.CustomerProfile?.FullName != null)
            return user.CustomerProfile.FullName;

        return null;
    }
}
