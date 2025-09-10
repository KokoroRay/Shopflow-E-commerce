namespace ShopFlow.Application.Abstractions.Services;

/// <summary>
/// Service interface for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a password reset OTP email to the specified recipient
    /// </summary>
    /// <param name="toEmail">The recipient's email address</param>
    /// <param name="otpCode">The OTP code to include in the email</param>
    /// <param name="expirationMinutes">The expiration time in minutes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task SendPasswordResetOtpAsync(string toEmail, string otpCode, int expirationMinutes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a password reset confirmation email
    /// </summary>
    /// <param name="toEmail">The recipient's email address</param>
    /// <param name="resetDateTime">When the password was reset</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task SendPasswordResetConfirmationAsync(string toEmail, DateTime resetDateTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a generic email with subject and body
    /// </summary>
    /// <param name="toEmail">The recipient's email address</param>
    /// <param name="subject">The email subject</param>
    /// <param name="body">The email body</param>
    /// <param name="isHtml">Whether the body is HTML formatted</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
}
