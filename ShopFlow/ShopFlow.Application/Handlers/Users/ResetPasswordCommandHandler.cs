using MediatR;
using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Services;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Abstractions.Messaging;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.DomainEvents;

namespace ShopFlow.Application.Handlers.Users;

/// <summary>
/// Handler for the ResetPasswordCommand
/// </summary>
public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IDomainEventPublisher _eventPublisher;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the ResetPasswordCommandHandler
    /// </summary>
    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IDomainEventPublisher eventPublisher,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the reset password command
    /// </summary>
    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing password reset request for email: {Email}", request.Email);

            // Validate inputs
            if (!Email.IsValid(request.Email))
            {
                _logger.LogWarning("Invalid email format provided: {Email}", request.Email);
                return new ResetPasswordResponse(false, "Invalid email format");
            }

            if (string.IsNullOrWhiteSpace(request.OtpCode))
            {
                return new ResetPasswordResponse(false, "OTP code is required");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new ResetPasswordResponse(false, "New password is required");
            }

            // Basic password validation
            if (request.NewPassword.Length < 8)
            {
                return new ResetPasswordResponse(false, "Password must be at least 8 characters long");
            }

            var email = new Email(request.Email);

            // Find user by email
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning("Password reset attempted for non-existent email: {Email}", request.Email);
                return new ResetPasswordResponse(false, "Invalid email or OTP code");
            }

            // Get the reset token by email (in-memory implementation)
            var resetToken = await _tokenRepository.GetByEmailAsync(email, cancellationToken);
            if (resetToken is null)
            {
                _logger.LogWarning("No active reset token found for user: {UserId}", user.Id);
                return new ResetPasswordResponse(false, "Invalid or expired OTP code");
            }

            // Verify OTP code
            if (!resetToken.VerifyOtp(request.OtpCode))
            {
                _logger.LogWarning("Invalid OTP provided for user: {UserId}, attempts: {AttemptCount}",
                    user.Id, resetToken.AttemptCount);

                // Update token with failed attempt
                await _tokenRepository.UpdateAsync(resetToken, cancellationToken);

                if (resetToken.IsLocked())
                {
                    return new ResetPasswordResponse(false, "Too many failed attempts. Please request a new OTP");
                }

                return new ResetPasswordResponse(false,
                    $"Invalid OTP code. {resetToken.GetRemainingAttempts()} attempts remaining");
            }

            // Check if token is still valid after verification
            if (!resetToken.IsValid())
            {
                return new ResetPasswordResponse(false, "OTP code has expired or is no longer valid");
            }

            // Hash the new password
            var hashedPassword = _passwordHasher.HashPassword(request.NewPassword);

            // Update user password
            user.ChangePassword(hashedPassword);

            // Mark token as used
            resetToken.MarkAsUsed();

            // Save changes
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _tokenRepository.UpdateAsync(resetToken, cancellationToken);

            // Send confirmation email
            await _emailService.SendPasswordResetConfirmationAsync(
                email.Value,
                DateTime.UtcNow,
                cancellationToken);

            // Publish domain event
            var domainEvent = new PasswordResetConfirmedEvent(
                user.Id,
                email.Value,
                DateTime.UtcNow);

            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);

            // Mark token as used in memory (for in-memory implementation, 
            // the token will be automatically cleaned up when expired)
            resetToken.MarkAsUsed();
            await _tokenRepository.UpdateAsync(resetToken, cancellationToken);

            _logger.LogInformation("Password reset completed successfully for user: {UserId}", user.Id);

            return new ResetPasswordResponse(true, "Password has been reset successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing password reset request for email: {Email}", request.Email);
            return new ResetPasswordResponse(false, "An error occurred while resetting your password");
        }
    }
}
