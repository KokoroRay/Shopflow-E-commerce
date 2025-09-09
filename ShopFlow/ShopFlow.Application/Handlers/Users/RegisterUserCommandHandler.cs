using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Messaging;
using ShopFlow.Application.Exceptions;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.DomainEvents;

namespace ShopFlow.Application.Handlers.Users;

/// <summary>
/// Handler for RegisterUserCommand
/// </summary>
public sealed partial class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDomainEventPublisher _domainEventPublisher;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the RegisterUserCommandHandler class
    /// </summary>
    /// <param name="userRepository">User repository</param>
    /// <param name="passwordHasher">Password hasher service</param>
    /// <param name="domainEventPublisher">Domain event publisher</param>
    /// <param name="logger">Logger instance</param>
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDomainEventPublisher domainEventPublisher,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _domainEventPublisher = domainEventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the RegisterUserCommand
    /// </summary>
    /// <param name="request">The registration command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User response</returns>
    public async Task<UserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        LogUserRegistrationStarted(request.Email);

        // Create email value object for validation
        var email = new Email(request.Email);

        // Check if email already exists
        var emailExists = await _userRepository.ExistsByEmailAsync(email, cancellationToken).ConfigureAwait(false);
        if (emailExists)
        {
            LogEmailAlreadyExists(request.Email);
            throw new EmailAlreadyExistsException(request.Email);
        }

        // Create user entity
        var phone = !string.IsNullOrWhiteSpace(request.Phone) ? new PhoneNumber(request.Phone) : null;
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new CoreUser(email, passwordHash, phone);

        // Save user to database
        await _userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);

        LogUserRegisteredSuccessfully(user.Id);

        // Publish domain event
        var domainEvent = new UserRegisteredEvent(user.Id, user.Email.Value);
        await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken).ConfigureAwait(false);

        // Return response
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email.Value,
            Phone = user.Phone?.Value,
            Status = (byte)user.Status,
            EmailVerified = user.EmailVerified,
            CreatedAt = user.CreatedAt,
            FullName = request.FullName,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            Roles = new List<string>()
        };
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Starting user registration for email: {Email}")]
    private partial void LogUserRegistrationStarted(string email);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Registration failed: Email {Email} already exists")]
    private partial void LogEmailAlreadyExists(string email);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "User registered successfully with ID: {UserId}")]
    private partial void LogUserRegisteredSuccessfully(long userId);
}
