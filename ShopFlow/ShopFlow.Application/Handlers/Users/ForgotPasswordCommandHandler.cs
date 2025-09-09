using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Services;
using ShopFlow.Application.Abstractions.Messaging;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.DomainEvents;

namespace ShopFlow.Application.Handlers.Users;

/// <summary>
/// Handler for the ForgotPasswordCommand
/// </summary>
public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly IOtpService _otpService;
    private readonly IDomainEventPublisher _eventPublisher;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the ForgotPasswordCommandHandler
    /// </summary>
    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository,
        IEmailService emailService,
        IOtpService otpService,
        IDomainEventPublisher eventPublisher,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _otpService = otpService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the forgot password command
    /// </summary>
    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing forgot password request for email: {Email}", request.Email);

            // Validate email format
            if (!Email.IsValid(request.Email))
            {
                _logger.LogWarning("Invalid email format provided: {Email}", request.Email);
                return new ForgotPasswordResponse(false, "Invalid email format");
            }

            var email = new Email(request.Email);

            // Find user by email
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
                // Don't reveal that email doesn't exist for security reasons
                return new ForgotPasswordResponse(true, "If the email exists, an OTP has been sent to it", 15);
            }

            // For in-memory implementation, we don't have rate limiting
            // This functionality could be added using IMemoryCache if needed

            // For in-memory, we just proceed to create a new token
            // (existing tokens will be replaced automatically by the in-memory repository)

            // Generate new OTP token
            const int expirationMinutes = 15;
            var resetToken = PasswordResetToken.Create(user.Id, email, expirationMinutes);

            // Save token to database
            await _tokenRepository.AddAsync(resetToken, cancellationToken);

            // Send OTP email
            await _emailService.SendPasswordResetOtpAsync(
                email.Value,
                resetToken.OtpCode.Value,
                expirationMinutes,
                cancellationToken);

            // Publish domain event
            var domainEvent = new PasswordResetRequestedEvent(
                user.Id,
                email,
                resetToken.OtpCode.Value,
                resetToken.ExpiresAt);

            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

            _logger.LogInformation("Password reset OTP sent successfully for user: {UserId}", user.Id);

            return new ForgotPasswordResponse(true, "OTP has been sent to your email", expirationMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing forgot password request for email: {Email}", request.Email);
            return new ForgotPasswordResponse(false, "An error occurred while processing your request");
        }
    }
}
